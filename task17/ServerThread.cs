using System;
using System.Collections.Concurrent;
using System.Threading;

public class ServerThread
{
    private readonly Thread _thread;
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IScheduler? _scheduler;
    private readonly CancellationTokenSource _cts = new();
    private volatile bool _softStopRequested = false;

    public Thread Thread => _thread;
    public Action<Exception, ICommand>? ExceptionHandler { get; set; }

    public ServerThread()
    {
        _thread = new Thread(RunWithoutScheduler);
    }

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(RunWithScheduler);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand command)
    {
        if (_queue.IsAddingCompleted) return;
        _queue.Add(command, _cts.Token);
    }

    private void RunWithoutScheduler()
    {
        try
        {
            foreach (var cmd in _queue.GetConsumingEnumerable(_cts.Token))
            {
                try { cmd.Execute(); }
                catch (Exception ex) { ExceptionHandler?.Invoke(ex, cmd); }

                if (_softStopRequested && _queue.Count == 0)
                {
                    _queue.CompleteAdding();
                    break;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (InvalidOperationException) { }
    }

    private void RunWithScheduler()
    {
        try
        {
            while (true)
            {
                _cts.Token.ThrowIfCancellationRequested();

                // Проверяем очередь с коротким таймаутом
                if (_queue.TryTake(out var cmdq, 10, _cts.Token))
                {
                    if (cmdq is HardStop) { _cts.Cancel(); _queue.CompleteAdding(); break; }
                    ExecuteCommand(cmdq);
                    continue;
                }

                // Планировщик
                if (_scheduler!.HasCommand())
                {
                    var cmd = _scheduler.Select();
                    _cts.Token.ThrowIfCancellationRequested();
                    
                    if (cmd is HardStop) { _cts.Cancel(); _queue.CompleteAdding(); break; }
                    ExecuteCommand(cmd);
                    continue;
                }

                // SoftStop
                if (_softStopRequested && !_scheduler.HasCommand()) break;
            }
        }
        catch (OperationCanceledException) { }
        catch (InvalidOperationException) { }
    }

    private void ExecuteCommand(ICommand cmd)
    {
        if (_cts.IsCancellationRequested) return;
        
        try { cmd.Execute(); }
        catch (Exception ex) { ExceptionHandler?.Invoke(ex, cmd); }

        if (_scheduler != null && cmd is ILongRunningCommand longCmd && !longCmd.IsComplete)
            _scheduler.Add(cmd);
    }
    internal void RequestHardStop() { _cts.Cancel(); _queue.CompleteAdding(); }
    internal void RequestSoftStop()
    {
        _softStopRequested = true;
        if (_queue.Count == 0 && (_scheduler == null || !_scheduler.HasCommand()))
            _queue.CompleteAdding();
    }
}