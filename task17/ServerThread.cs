using System;
using System.Collections.Concurrent;
using System.Threading;

public class ServerThread
{
    private readonly Thread _thread;
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly CancellationTokenSource _cts = new();
    private volatile bool _softStopRequested = false;

    public Thread Thread => _thread;
    public Action<Exception, ICommand>? ExceptionHandler { get; set; }

    public ServerThread()
    {
        _thread = new Thread(Run);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand command)
    {
        if (_queue.IsAddingCompleted) return;
        _queue.Add(command, _cts.Token);
    }

    private void Run()
    {
        try
        {
            foreach (var cmd in _queue.GetConsumingEnumerable(_cts.Token))
            {
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke(ex, cmd);
                }

                if (_softStopRequested && _queue.Count == 0)
                {
                    _queue.CompleteAdding();
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // HardStop — нормальное завершение
        }
        catch (InvalidOperationException)
        {
            // CompleteAdding вызван — нормальное завершение
        }
    }

    internal void RequestHardStop()
    {
        _cts.Cancel();
        _queue.CompleteAdding();
    }

    internal void RequestSoftStop()
    {
        _softStopRequested = true;
        if (_queue.Count == 0)
        {
            _queue.CompleteAdding();
        }
    }
}