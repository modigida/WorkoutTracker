﻿using System.Windows.Input;

namespace WorkoutTracker.Commands;
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
    public event EventHandler? CanExecuteChanged;
    public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (x => true);

        CommandManager.RequerySuggested += (sender, e) => RaiseCanExecuteChanged();
    }
    public bool CanExecute(object? parameter) => _canExecute(parameter!);
    public void Execute(object? parameter) => _execute(parameter!);
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (x => true);

        CommandManager.RequerySuggested += (sender, e) => RaiseCanExecuteChanged();
    }
    public bool CanExecute(object? parameter) => _canExecute((T?)parameter!);

    public void Execute(object? parameter) => _execute((T?)parameter!);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
