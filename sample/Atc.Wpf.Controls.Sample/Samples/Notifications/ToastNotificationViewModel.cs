namespace Atc.Wpf.Controls.Sample.Samples.Notifications;

public class ToastNotificationViewModel : ViewModelBase
{
    private readonly ToastNotificationManager notificationManager = new();

    public ICommand WindowInformationCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: false, toastNotificationType: ToastNotificationType.Information));

    public ICommand WindowSuccessCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: false, toastNotificationType: ToastNotificationType.Success));

    public ICommand WindowWarningCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: false, toastNotificationType: ToastNotificationType.Warning));

    public ICommand WindowErrorCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: false, toastNotificationType: ToastNotificationType.Error));

    public ICommand DesktopInformationCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: true, toastNotificationType: ToastNotificationType.Information));

    public ICommand DesktopSuccessCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: true, toastNotificationType: ToastNotificationType.Success));

    public ICommand DesktopWarningCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: true, toastNotificationType: ToastNotificationType.Warning));

    public ICommand DesktopErrorCommand => new RelayCommand(execute: () => ToastNotificationHandler(useDesktop: true, toastNotificationType: ToastNotificationType.Error));

    private void ToastNotificationHandler(
        bool useDesktop,
        ToastNotificationType toastNotificationType)
    {
        var title = useDesktop
            ? $"ToastNotification in Desktop - {DateTime.Now:T}"
            : $"ToastNotification in Window - {DateTime.Now:T}";

        var content = new ToastNotificationContent(
            toastNotificationType,
            title,
            "Hello world - Click me!");

        var clickContentMessage = useDesktop
            ? "Desktop notification was clicked!"
            : "Window notification was clicked!";

        var clickContent = new ToastNotificationContent(
            ToastNotificationType.Information,
            "Clicked!",
            clickContentMessage);

        if (useDesktop)
        {
            notificationManager.Show(
                useDesktop,
                content);
        }
        else
        {
            notificationManager.Show(
                useDesktop,
                content,
                "WindowArea",
                onClick: () => notificationManager.Show(useDesktop, clickContent));
        }
    }
}