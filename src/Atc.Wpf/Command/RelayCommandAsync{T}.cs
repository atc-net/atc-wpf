// ReSharper disable SuggestVarOrType_SimpleTypes
namespace Atc.Wpf.Command;

public class RelayCommandAsync<T> : IRelayCommandAsync<T>
{
    private readonly Func<T, Task>? execute;
    private readonly WeakFunc<T, bool>? wfCanExecute;
    private readonly IErrorHandler? errorHandler;
    private EventHandler requerySuggestedLocal = null!;
    private bool isExecuting;

    public RelayCommandAsync(
        Func<T, Task> execute,
        Func<T, bool>? canExecute = null,
        IErrorHandler? errorHandler = null,
        bool keepTargetAlive = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));

        if (canExecute is not null)
        {
            this.wfCanExecute = new WeakFunc<T, bool>(canExecute, keepTargetAlive);
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
        var val = parameter;

        if (parameter is not null &&
            parameter.GetType() != typeof(T) &&
            parameter is IConvertible)
        {
            val = Convert.ChangeType(parameter, typeof(T), provider: null);
        }

        if (this.isExecuting ||
            !this.CanExecute(val) ||
            this.execute is null)
        {
            return;
        }

        this.isExecuting = true;

        if (this.errorHandler is null)
        {
            await DoExecute(val);
            this.RaiseCanExecuteChanged();
            this.isExecuting = false;
        }
        else
        {
            try
            {
                await DoExecute(val);
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

    public Task ExecuteAsync(T parameter)
    {
        if (this.CanExecute(parameter) &&
            this.execute is not null)
        {
            return this.execute(parameter);
        }

        return Task.CompletedTask;
    }

    private async Task DoExecute(object? val)
    {
        if (this.execute is null)
        {
            return;
        }

        if (val is null)
        {
            if (typeof(T).IsValueType)
            {
                await this.ExecuteAsync(default!);
            }
            else
            {
                await this.execute((T)val!);
            }
        }
        else
        {
            await this.execute((T)val);
        }
    }
}
