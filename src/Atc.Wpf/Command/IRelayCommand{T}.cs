// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedTypeParameter
namespace Atc.Wpf.Command;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "OK.")]
public interface IRelayCommand<T> : ICommand
{
    /// <summary>
    /// Raises the <see cref="RelayCommand.CanExecuteChanged" /> event.
    /// </summary>
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    void RaiseCanExecuteChanged();
}