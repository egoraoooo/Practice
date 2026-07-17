using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Task17Tests
{
    class CounterCommand : ICommand
    {
        public int Count { get; private set; }
        public void Execute() => Count++;
    }

    class ThrowCommand : ICommand
    {
        public void Execute() => throw new InvalidOperationException("Тестовое исключение");
    }

    class StepCommand : ILongRunningCommand
    {
        private readonly int _totalSteps;
        public int CurrentStep { get; private set; }
        public bool IsComplete => CurrentStep >= _totalSteps;
        public StepCommand(int totalSteps) => _totalSteps = totalSteps;
        public void Execute() { if (!IsComplete) CurrentStep++; }
    }

    class TrackedStepCommand : ILongRunningCommand
    {
        private readonly int _totalSteps;
        private readonly string _name;
        private readonly List<string> _tracker;
        public int CurrentStep { get; private set; }
        public bool IsComplete => CurrentStep >= _totalSteps;
        public TrackedStepCommand(string name, int totalSteps, List<string> tracker)
        {
            _name = name; _totalSteps = totalSteps; _tracker = tracker;
        }
        public void Execute()
        {
            if (!IsComplete) { CurrentStep++; _tracker.Add(_name); }
        }
    }

    public class ServerThreadTests
    {
        // ========== Тесты задания 17 ==========

        [Fact]
        public void HardStop_StopsThreadImmediately()
        {
            var server = new ServerThread();
            var counter = new CounterCommand();
            server.AddCommand(counter);
            server.AddCommand(counter);
            server.AddCommand(new HardStop(server));
            server.AddCommand(counter);
            server.AddCommand(counter);
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
            server.AddCommand(goodCmd);
            server.AddCommand(new ThrowCommand());
            server.AddCommand(goodCmd);
            server.AddCommand(new SoftStop(server));
            server.Start();
            server.Thread.Join();
            Assert.Single(exceptions);
            Assert.Equal(2, goodCmd.Count);
        }

        // ========== Тесты задания 18 ==========

        [Fact]
        public void LongRunningCommand_CompletesAfterMultipleExecutes()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            var longCmd = new StepCommand(5);
            server.AddCommand(longCmd);
            server.AddCommand(new SoftStop(server));
            server.Start();
            server.Thread.Join();
            Assert.Equal(5, longCmd.CurrentStep);
            Assert.True(longCmd.IsComplete);
        }

        [Fact]
        public void RoundRobin_BothCommandsExecute()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            var tracker = new List<string>();
            var cmd1 = new TrackedStepCommand("A", 4, tracker);
            var cmd2 = new TrackedStepCommand("B", 4, tracker);
            server.AddCommand(cmd1);
            server.AddCommand(cmd2);
            server.AddCommand(new SoftStop(server));
            server.Start();
            server.Thread.Join();
            Assert.Equal(4, cmd1.CurrentStep);
            Assert.Equal(4, cmd2.CurrentStep);
            Assert.Equal(8, tracker.Count);
        }

        [Fact]
        public void Interleaving_ShortCommandsRunBetweenLongSteps()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            var longCmd = new StepCommand(20);
            var counters = Enumerable.Range(0, 5).Select(_ => new CounterCommand()).ToArray();
            server.AddCommand(longCmd);
            foreach (var c in counters) server.AddCommand(c);
            server.AddCommand(new SoftStop(server));
            server.Start();
            server.Thread.Join();
            Assert.True(longCmd.IsComplete);
            foreach (var c in counters) Assert.Equal(1, c.Count);
        }

        [Fact]
        public void HardStop_StopsThread_WithLongRunningCommand()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            var longCmd = new StepCommand(100000);
            server.AddCommand(longCmd);
            server.AddCommand(new HardStop(server));
            server.Start();
            var stopped = server.Thread.Join(500);
            Assert.True(stopped);
            Assert.False(longCmd.IsComplete);
        }

        [Fact]
        public void ExceptionInSchedulerCommand_IsCaughtAndThreadContinues()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            var exceptions = new List<Exception>();
            server.ExceptionHandler = (ex, cmd) => exceptions.Add(ex);
            var goodCmd = new CounterCommand();
            server.AddCommand(goodCmd);
            server.AddCommand(new ThrowCommand());
            server.AddCommand(goodCmd);
            server.AddCommand(new SoftStop(server));
            server.Start();
            server.Thread.Join();
            Assert.Single(exceptions);
            Assert.Equal(2, goodCmd.Count);
        }
    }
}