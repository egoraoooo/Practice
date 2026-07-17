using System;

// Адаптер, позволяющий выполнить обычную команду заданное количество раз
// как длительную (ILongRunningCommand).
public class StepCommandAdapter : ILongRunningCommand
{
    private readonly ICommand _command;
    private int _executedCount;
    private readonly int _requiredExecutions;

    public bool IsComplete => _executedCount >= _requiredExecutions;

    public int ExecutedCount => _executedCount;
    public int RequiredExecutions => _requiredExecutions;

    public StepCommandAdapter(ICommand command, int requiredExecutions)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _requiredExecutions = requiredExecutions;
    }

    public void Execute()
    {
        if (!IsComplete)
        {
            _command.Execute();
            _executedCount++;
        }
    }
}
