// ReSharper disable SuggestVarOrType_SimpleTypes
namespace Atc.Wpf.Command;

public class RelayCommandAsync : IRelayCommandAsync
{
    private readonly Func<Task>? execute;
    private readonly WeakFunc<bool>? wfCanExecute;
    private readonly IErrorHandler? errorHandler;
    private EventHandler requerySuggestedLocal = null!;
    private bool isExecuting;

    public RelayCommandAsync(
        Func<Task> execute,
        Func<bool>? canExecute = null,
        IErrorHandler? errorHandler = null,
        bool keepTargetAlive = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));

        if (canExecute is not null)
        {
            wfCanExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
        }

        if (errorHandler is not null)
        {
            this.errorHandler = errorHandler;
        }
    }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            if (wfCanExecute is null)
            {
                return;
            }

            EventHandler handler2;
            var canExecuteChanged = requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                var handler3 = (EventHandler)Delegate.Combine(handler2, value);
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
            var canExecuteChanged = requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                var handler3 = (EventHandler)Delegate.Remove(handler2, value)!;
                canExecuteChanged = Interlocked.CompareExchange(
                    ref requerySuggestedLocal,
                    handler3,
                    handler2);
            }
            while (canExecuteChanged != handler2);

            CommandManager.RequerySuggested -= value;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    public bool CanExecute(object? parameter)
    {
        return wfCanExecute is null ||
               ((wfCanExecute.IsStatic || wfCanExecute.IsAlive) && wfCanExecute.Execute());
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK - ICommand signature")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK - errorHandler will handle it")]
    public async void Execute(object? parameter)
    {
        if (isExecuting ||
            !CanExecute(parameter) ||
            execute is null)
        {
            return;
        }

        isExecuting = true;

        if (errorHandler is null)
        {
            await ExecuteAsync(parameter);
            RaiseCanExecuteChanged();
            isExecuting = false;
        }
        else
        {
            try
            {
                await ExecuteAsync(parameter);
                RaiseCanExecuteChanged();
                isExecuting = false;
            }
            catch (Exception ex)
            {
                errorHandler?.HandleError(ex);
                isExecuting = false;
            }
        }
    }

    public Task ExecuteAsync(object? parameter)
    {
        if (CanExecute(parameter) &&
            execute is not null)
        {
            return execute();
        }

        return Task.CompletedTask;
    }
}