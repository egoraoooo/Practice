using System;

public interface ILongRunningCommand : ICommand
{
    bool IsComplete { get; }
}