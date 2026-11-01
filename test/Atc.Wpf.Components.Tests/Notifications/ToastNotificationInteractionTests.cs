namespace Atc.Wpf.Components.Tests.Notifications;

/// <summary>
/// <see cref="ToastNotification.CloseOnClick"/> closes the toast when it is clicked, and a toast does not
/// expire while the mouse is over it.
/// </summary>
public sealed class ToastNotificationInteractionTests
{
    [StaFact]
    public void CloseOnClick_True_ClickClosesTheNotification()
    {
        // Arrange
        var toast = new ToastNotification { CloseOnClick = true };
        var closed = false;
        toast.NotificationClosed += (_, _) => closed = true;

        // Act
        Click(toast);
        DispatcherPump.Until(() => closed, TimeSpan.FromSeconds(2));

        // Assert
        Assert.True(closed);
    }

    [StaFact]
    public void CloseOnClick_False_ClickKeepsTheNotificationOpen()
    {
        // Arrange
        var toast = new ToastNotification();

        // Act
        Click(toast);

        // Assert
        Assert.False(toast.IsClosing);
    }

    [StaFact]
    public void CloseOnClick_ClickAlreadyHandled_KeepsTheNotificationOpen()
    {
        // Arrange
        var toast = new ToastNotification { CloseOnClick = true };

        // Act
        Click(toast, handled: true);

        // Assert
        Assert.False(toast.IsClosing);
    }

    [StaFact]
    public void WaitForExpiry_NoHover_CompletesAfterExpirationTime()
    {
        // Arrange
        var toast = new ToastNotification();

        // Act
        var expiry = toast.WaitForExpiryAsync(TimeSpan.FromMilliseconds(50), pauseOnHover: true);
        DispatcherPump.Until(() => expiry.IsCompleted, TimeSpan.FromSeconds(2));

        // Assert
        Assert.True(expiry.IsCompleted);
    }

    [StaFact]
    public void WaitForExpiry_MouseOver_DoesNotExpireUntilTheMouseLeaves()
    {
        // Arrange
        var toast = new ToastNotification();
        var expiry = toast.WaitForExpiryAsync(TimeSpan.FromMilliseconds(100), pauseOnHover: true);

        // Act
        RaiseMouse(toast, UIElement.MouseEnterEvent);
        DispatcherPump.Until(() => expiry.IsCompleted, TimeSpan.FromMilliseconds(400));
        var expiredWhileHovered = expiry.IsCompleted;

        RaiseMouse(toast, UIElement.MouseLeaveEvent);
        DispatcherPump.Until(() => expiry.IsCompleted, TimeSpan.FromSeconds(2));

        // Assert
        Assert.False(expiredWhileHovered);
        Assert.True(expiry.IsCompleted);
    }

    [StaFact]
    public void WaitForExpiry_MouseOverWithPauseOnHoverOff_ExpiresAnyway()
    {
        // Arrange
        var toast = new ToastNotification();
        var expiry = toast.WaitForExpiryAsync(TimeSpan.FromMilliseconds(50), pauseOnHover: false);

        // Act
        RaiseMouse(toast, UIElement.MouseEnterEvent);
        DispatcherPump.Until(() => expiry.IsCompleted, TimeSpan.FromSeconds(2));

        // Assert
        Assert.True(expiry.IsCompleted);
    }

    [StaFact]
    public void WaitForExpiry_NotificationClosedEarly_Completes()
    {
        // Arrange
        var toast = new ToastNotification();
        var expiry = toast.WaitForExpiryAsync(TimeSpan.FromHours(1), pauseOnHover: true);

        // Act
        toast.Close();
        DispatcherPump.Until(() => expiry.IsCompleted, TimeSpan.FromSeconds(2));

        // Assert
        Assert.True(expiry.IsCompleted);
    }

    private static void Click(
        ToastNotification toast,
        bool handled = false)
    {
        // A handled click is one a button inside the toast has already taken.
        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseLeftButtonDownEvent,
            Handled = handled,
        };

        toast.RaiseEvent(args);
    }

    private static void RaiseMouse(
        UIElement element,
        RoutedEvent routedEvent)
        => element.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = routedEvent });
}