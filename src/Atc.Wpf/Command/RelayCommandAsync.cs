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
            this.wfCanExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
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
            if (this.wfCanExecute is null)
            {
                return;
            }

            EventHandler handler2;
            EventHandler canExecuteChanged = this.requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                canExecuteChanged = Interlocked.CompareExchange(
                    ref this.requerySuggestedLocal,
                    handler3,
                    handler2);
            }
            while (canExecuteChanged != handler2);

            CommandManager.RequerySuggested += value;
        }

        remove
        {
            if (this.wfCanExecute is null)
            {
                return;
            }

            EventHandler handler2;
            EventHandler canExecuteChanged = this.requerySuggestedLocal;

            do
            {
                handler2 = canExecuteChanged;
                EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value)!;
                canExecuteChanged = Interlocked.CompareExchange(
                    ref this.requerySuggestedLocal,
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
        return this.wfCanExecute is null ||
               ((this.wfCanExecute.IsStatic || this.wfCanExecute.IsAlive) && this.wfCanExecute.Execute());
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK - ICommand signature")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK - errorHandler will handle it")]
    public async void Execute(object? parameter)
    {
        if (this.isExecuting ||
            !this.CanExecute(parameter) ||
            this.execute is null)
        {
            return;
        }

        this.isExecuting = true;

        if (this.errorHandler is null)
        {
            await this.ExecuteAsync(parameter);
            this.RaiseCanExecuteChanged();
            this.isExecuting = false;
        }
        else
        {
            try
            {
                await this.ExecuteAsync(parameter);
                this.RaiseCanExecuteChanged();
                this.isExecuting = false;
            }
            catch (Exception ex)
            {
                this.errorHandler?.HandleError(ex);
                this.isExecuting = false;
            }
        }
    }

    public Task ExecuteAsync(object? parameter)
    {
        if (this.CanExecute(parameter) &&
            this.execute is not null)
        {
            return this.execute();
        }

        return Task.CompletedTask;
    }
}