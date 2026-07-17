using System;
using System.Threading;

public class HardStop : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStop(ServerThread serverThread)
    {
        _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.Thread)
            throw new InvalidOperationException("HardStop может выполняться только в том потоке, который останавливает");

        _serverThread.RequestHardStop();
    }
}