namespace Atc.Wpf.Sample.SamplesWpfComponents.Notifications;

public partial class TrayIconView : INotifyPropertyChanged
{
    private string status = "Ready";

    public TrayIconView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Status
    {
        get => status;
        private set
        {
            status = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    private void OnTrayIconClick(
        object? sender,
        EventArgs e)
        => Status = $"Icon clicked at {DateTime.Now:T}";

    private void OnTrayIconDoubleClick(
        object? sender,
        EventArgs e)
    {
        Status = $"Icon double-clicked at {DateTime.Now:T}";
        BringWindowToFront();
    }

    private void OnShowNotificationClick(
        object sender,
        RoutedEventArgs e)
        => ShowNotification(ToastNotificationType.Information);

    private void OnBringToFrontClick(
        object sender,
        RoutedEventArgs e)
        => BringWindowToFront();

    private void OnHideIconClick(
        object sender,
        RoutedEventArgs e)
        => CbShowIcon.IsChecked = false;

    private void OnNotifyClick(
        object sender,
        RoutedEventArgs e)
        => ShowNotification((ToastNotificationType)((FrameworkElement)sender).Tag);

    private void ShowNotification(ToastNotificationType type)
    {
        SampleTrayIcon.ShowNotification(
            "Tray icon",
            $"A {type.ToString().ToLowerInvariant()} notification from the tray icon. Click it to bring the window to the front.",
            type,
            onClick: BringWindowToFront);
        Status = $"{type} notification shown";
    }

    private void BringWindowToFront()
    {
        var window = Window.GetWindow(this);
        if (window is null)
        {
            return;
        }

        if (window.WindowState == WindowState.Minimized)
        {
            window.WindowState = WindowState.Normal;
        }

        window.Activate();
    }
}