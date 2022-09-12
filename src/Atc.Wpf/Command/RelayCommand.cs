// ReSharper disable SuggestVarOrType_SimpleTypes
namespace Atc.Wpf.Command;

/// <summary>
/// A command whose sole purpose is to relay its functionality to other
/// objects by invoking delegates. The default return value for the CanExecute
/// method is 'true'. This class does not allow you to accept command parameters in the
/// Execute and CanExecute callback methods.
/// </summary>
public sealed class RelayCommand : IRelayCommand
{
    private readonly WeakAction? waExecute;
    private readonly WeakFunc<bool>? wfCanExecute;
    private EventHandler requerySuggestedLocal = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is causing a closures.</param>
    /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
    public RelayCommand(Action execute, Func<bool>? canExecute = null, bool keepTargetAlive = false)
    {
        if (execute is null)
        {
            throw new ArgumentNullException(nameof(execute));
        }

        waExecute = new WeakAction(execute, keepTargetAlive);

        if (canExecute is not null)
        {
            wfCanExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
        }
    }

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            if (wfCanExecute is null)
            {
                return;
            }

            EventHandler handler2;
            EventHandler canExecuteChanged = requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                canExecuteChanged = Interlocked.CompareExchange(
                    ref requerySuggestedLocal,
                    handler3,
                    handler2);
            }
            while (canExecuteChanged != handler2);

            CommandManager.RequerySuggested += value;
        }

        remove
        {
            if (wfCanExecute is null)
            {
                return;
            }

            EventHandler handler2;
            EventHandler canExecuteChanged = requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value)!;
                canExecuteChanged = Interlocked.CompareExchange(
                    ref requerySuggestedLocal,
                    handler3,
                    handler2);
            }
            while (canExecuteChanged != handler2);

            CommandManager.RequerySuggested -= value;
        }
    }

    /// <inheritdoc />
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    /// <inheritdoc />
    public bool CanExecute(object? parameter)
    {
        return wfCanExecute is null ||
               ((wfCanExecute.IsStatic || wfCanExecute.IsAlive) && wfCanExecute.Execute());
    }

    /// <inheritdoc />
    public void Execute(object? parameter)
    {
        if (CanExecute(parameter)
            && waExecute is not null
            && (waExecute.IsStatic || waExecute.IsAlive))
        {
            waExecute.Execute();
        }
    }
}