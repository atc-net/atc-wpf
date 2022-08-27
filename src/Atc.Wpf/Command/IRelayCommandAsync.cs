namespace Atc.Wpf.Command;

public interface IRelayCommandAsync : IRelayCommand
{
    Task ExecuteAsync(object? parameter);
}