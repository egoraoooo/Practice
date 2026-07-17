using System;

public class TestCommand : ICommand
{
    private readonly int _id;
    private int _counter = 0;

    public TestCommand(int id)
    {
        _id = id;
    }

    public void Execute()
    {
        Console.WriteLine($"Поток {_id} вызов {++_counter}");
    }
}