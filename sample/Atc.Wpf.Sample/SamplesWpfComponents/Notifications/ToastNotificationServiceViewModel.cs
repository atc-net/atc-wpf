namespace Atc.Wpf.Sample.SamplesWpfComponents.Notifications;

public sealed partial class ToastNotificationServiceViewModel : ViewModelBase
{
    private readonly IToastNotificationService notificationService = new ToastNotificationService();

    [RelayCommand]
    private void ShowInformation()
        => notificationService.ShowInformation(
            $"Information - {DateTime.Now:T}",
            "This is an information notification via IToastNotificationService.",
            "ServiceArea");

    [RelayCommand]
    private void ShowSuccess()
        => notificationService.ShowSuccess(
            $"Success - {DateTime.Now:T}",
            "Operation completed successfully!",
            "ServiceArea");

    [RelayCommand]
    private void ShowWarning()
        => notificationService.ShowWarning(
            $"Warning - {DateTime.Now:T}",
            "This action may have consequences.",
            "ServiceArea");

    [RelayCommand]
    private void ShowError()
        => notificationService.ShowError(
            $"Error - {DateTime.Now:T}",
            "Something went wrong!",
            "ServiceArea");

    [RelayCommand]
    private void ShowDesktop()
        => notificationService.ShowInformation(
            $"Desktop - {DateTime.Now:T}",
            "This notification appears on the desktop overlay.",
            useDesktop: true);

    [RelayCommand]
    private void ShowCustomDuration()
        => notificationService.ShowInformation(
            $"Custom Duration - {DateTime.Now:T}",
            "This notification stays for 10 seconds.",
            "ServiceArea",
            expirationTime: TimeSpan.FromSeconds(10));

    [RelayCommand]
    private void ShowWithClickAction()
        => notificationService.ShowSuccess(
            $"Clickable - {DateTime.Now:T}",
            "Click me for a follow-up notification!",
            "ServiceArea",
            onClick: () => notificationService.ShowInformation(
                "Clicked!",
                "You clicked the notification.",
                "ServiceArea"));
}