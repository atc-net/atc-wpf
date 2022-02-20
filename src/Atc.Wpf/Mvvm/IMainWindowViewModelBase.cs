// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Atc.Wpf.Mvvm;

[SuppressMessage("Security", "CA2109:Review visible event handlers", Justification = "OK.")]
public interface IMainWindowViewModelBase : IViewModelBase
{
    /// <summary>
    /// Gets or sets the state of the window.
    /// </summary>
    /// <value>
    /// The state of the window.
    /// </value>
    WindowState WindowState { get; set; }

    /// <summary>
    /// Gets the application exit command.
    /// </summary>
    /// <value>
    /// The application exit command.
    /// </value>
    ICommand ApplicationExitCommand { get; }

    /// <summary>
    /// Called when loaded.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    void OnLoaded(object sender, RoutedEventArgs args);

    /// <summary>
    /// Called when closing.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
    void OnClosing(object sender, CancelEventArgs args);

    /// <summary>
    /// Called when key down.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    void OnKeyDown(object sender, KeyEventArgs args);

    /// <summary>
    /// Called when key up.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    void OnKeyUp(object sender, KeyEventArgs args);
}