// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedTypeParameter
namespace Atc.Wpf.Command;

public interface IRelayCommand<T> : ICommand
{
    /// <summary>
    /// Raises the <see cref="RelayCommand.CanExecuteChanged" /> event.
    /// </summary>
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    void RaiseCanExecuteChanged();
}