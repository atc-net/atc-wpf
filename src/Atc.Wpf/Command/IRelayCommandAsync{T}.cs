namespace Atc.Wpf.Command;

public interface IRelayCommandAsync<in T> : IRelayCommand
{
    Task ExecuteAsync(T parameter);
}