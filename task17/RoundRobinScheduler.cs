using System.Collections.Generic;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _queue = new();

    public bool HasCommand()
    {
        lock (_queue) return _queue.Count > 0;
    }

    public ICommand Select()
    {
        lock (_queue) return _queue.Dequeue();
    }

    public void Add(ICommand cmd)
    {
        lock (_queue) _queue.Enqueue(cmd);
    }
}