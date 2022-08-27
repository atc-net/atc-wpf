// ReSharper disable SuggestVarOrType_SimpleTypes
namespace Atc.Wpf.Command;

public class RelayCommandAsync : IRelayCommandAsync
{
    private readonly Func<Task>? execute;
    private readonly WeakFunc<bool>? wfCanExecute;
    private EventHandler requerySuggestedLocal = null!;

    public RelayCommandAsync(Func<Task> execute, Func<bool>? canExecute = null, bool keepTargetAlive = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));

        if (canExecute is not null)
        {
            this.wfCanExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
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
        if (this.CanExecute(parameter)
            && this.execute is not null)
        {
            this.ExecuteAsync(parameter);
        }
    }

    public Task ExecuteAsync(object? parameter)
    {
        if (this.CanExecute(parameter)
            && this.execute is not null)
        {
            return Task.Run(() => this.execute(), CancellationToken.None);
        }

        return Task.CompletedTask;
    }
}