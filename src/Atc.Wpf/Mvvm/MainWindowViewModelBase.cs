namespace Atc.Wpf.Mvvm;

public class MainWindowViewModelBase : ViewModelBase, IMainWindowViewModelBase
{
    private WindowState windowState;

    /// <inheritdoc />
    public WindowState WindowState
    {
        get => windowState;
        set
        {
            windowState = value;
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public ICommand ApplicationExitCommand => new RelayCommand(ApplicationExitCommandHandler);

    /// <inheritdoc />
    public void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        // Method intentionally left empty.
    }

    /// <inheritdoc />
    public void OnClosing(
        object sender,
        CancelEventArgs e)
    {
        Application.Current.Shutdown(-1);
    }

    /// <inheritdoc />
    public void OnKeyDown(
        object sender,
        KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Key != Key.F11)
        {
            return;
        }

        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
        e.Handled = true;
    }

    /// <inheritdoc />
    public void OnKeyUp(
        object sender,
        KeyEventArgs e)
    {
        // Method intentionally left empty.
    }

    private void ApplicationExitCommandHandler()
    {
        OnClosing(this, new CancelEventArgs());
    }
}