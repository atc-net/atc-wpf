// ReSharper disable UnusedMemberInSuper.Global
namespace Atc.Wpf.Command;

public interface IRelayCommand : ICommand
{
    /// <summary>
    /// Raises the <see cref="RelayCommand.CanExecuteChanged" /> event.
    /// </summary>
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    void RaiseCanExecuteChanged();
}