using System;
using System.Windows.Input;

namespace TICLab5;

public class ActionCommand : ICommand
{
    private readonly Action<object>? _action;

    public ActionCommand(Action<object> exec)
    {
        _action = exec;
    }

    public bool CanExecute(object? parameter)
    {
        return _action != null;
    }

    public void Execute(object? parameter)
    {
        if(_action != null)
            _action(parameter?? "Null");
    }

    public event EventHandler? CanExecuteChanged;
}