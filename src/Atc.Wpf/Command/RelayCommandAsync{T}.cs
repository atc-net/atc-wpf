// ReSharper disable SuggestVarOrType_SimpleTypes
namespace Atc.Wpf.Command;

public class RelayCommandAsync<T> : IRelayCommandAsync<T>
{
    private readonly Func<T, Task>? execute;
    private readonly WeakFunc<T, bool>? wfCanExecute;
    private EventHandler requerySuggestedLocal = null!;

    public RelayCommandAsync(Func<T, Task> execute, Func<T, bool>? canExecute = null, bool keepTargetAlive = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));

        if (canExecute is not null)
        {
            this.wfCanExecute = new WeakFunc<T, bool>(canExecute, keepTargetAlive);
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

    public void Execute(object? parameter)
    {
        var val = parameter;

        if (parameter is not null &&
            parameter.GetType() != typeof(T) &&
            parameter is IConvertible)
        {
            val = Convert.ChangeType(parameter, typeof(T), provider: null);
        }

        if (!CanExecute(val) ||
            this.execute is null)
        {
            return;
        }

        if (val is null)
        {
            if (typeof(T).IsValueType)
            {
                this.ExecuteAsync(default!);
            }
            else
            {
                this.execute((T)val!);
            }
        }
        else
        {
            this.execute((T)val);
        }
    }

    public Task ExecuteAsync(T parameter)
    {
        if (this.CanExecute(parameter)
            && this.execute is not null)
        {
            return Task.Run(() => this.execute(parameter), CancellationToken.None);
        }

        return Task.CompletedTask;
    }
}
