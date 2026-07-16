namespace ServerThreadTests;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

class CounterCommand : ICommand
{
    public int Count { get; private set; }
    public void Execute() => Count++;
}

class ThrowCommand : ICommand
{
    public void Execute() => throw new InvalidOperationException("Тестовое исключение");
}

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsThreadImmediately()
    {
        var server = new ServerThread();
        var counter = new CounterCommand();

        server.AddCommand(counter);
        server.AddCommand(counter);
        server.AddCommand(new HardStop(server));
        server.AddCommand(counter); // не выполнится
        server.AddCommand(counter); // не выполнится

        server.Start();
        server.Thread.Join();

        Assert.Equal(2, counter.Count);
    }

    [Fact]
    public void SoftStop_StopsThreadAfterQueueEmpty()
    {
        var server = new ServerThread();
        var counter = new CounterCommand();

        server.AddCommand(counter);
        server.AddCommand(counter);
        server.AddCommand(new SoftStop(server));
        server.AddCommand(counter);

        server.Start();
        server.Thread.Join();

        Assert.Equal(3, counter.Count);
    }

    [Fact]
    public void HardStop_FromAnotherThread_ThrowsException()
    {
        var server = new ServerThread();
        var hardStop = new HardStop(server);

        var ex = Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Contains("HardStop", ex.Message);
    }

    [Fact]
    public void SoftStop_FromAnotherThread_ThrowsException()
    {
        var server = new ServerThread();
        var softStop = new SoftStop(server);

        var ex = Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        Assert.Contains("SoftStop", ex.Message);
    }

    [Fact]
    public void ExceptionInCommand_IsCaughtAndThreadContinues()
    {
        var server = new ServerThread();
        var exceptions = new List<(Exception, ICommand)>();
        server.ExceptionHandler = (ex, cmd) => exceptions.Add((ex, cmd));

        var goodCmd = new CounterCommand();
        var badCmd = new ThrowCommand();

        server.AddCommand(goodCmd);
        server.AddCommand(badCmd);
        server.AddCommand(goodCmd);
        server.AddCommand(new SoftStop(server));

        server.Start();
        server.Thread.Join();

        Assert.Single(exceptions);
        Assert.Equal(2, goodCmd.Count);
    }
}