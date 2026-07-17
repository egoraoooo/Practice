using System;
using System.Threading;

public class SoftStop : ICommand
{
    private readonly ServerThread _serverThread;

    public SoftStop(ServerThread serverThread)
    {
        _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.Thread)
            throw new InvalidOperationException("SoftStop может выполняться только в том потоке, который останавливает");

        _serverThread.RequestSoftStop();
    }
}