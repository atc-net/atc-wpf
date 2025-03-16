namespace Atc.Wpf.Controls.Notifications;

public interface IToastNotificationManager
{
    void Show(
        bool useDesktop,
        UserControl content,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null);
}