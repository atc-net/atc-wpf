namespace Atc.Wpf.Components.Dialogs.Helpers;

internal static class DialogBoxHelper
{
    /// <summary>
    /// Makes <paramref name="owningWindow"/> the owner of <paramref name="dialog"/> and centers the dialog on it,
    /// when the owner has already been shown. A window that has no handle yet (or has been closed) cannot own
    /// another window, so in that case the dialog keeps its current startup location.
    /// </summary>
    public static void ApplyOwner(
        Window dialog,
        Window? owningWindow)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        if (owningWindow is null ||
            ReferenceEquals(owningWindow, dialog) ||
            !HasWindowHandle(owningWindow))
        {
            return;
        }

        dialog.Owner = owningWindow;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    /// <summary>
    /// Whether the window has been shown and not yet closed. <see cref="FrameworkElement.IsLoaded"/> is not
    /// used because it stays <see langword="true"/> until the deferred Unloaded broadcast after <see cref="Window.Close"/>.
    /// </summary>
    private static bool HasWindowHandle(Window window)
        => new WindowInteropHelper(window).Handle != IntPtr.Zero &&
           PresentationSource.FromVisual(window) is not null;

    public static ContentControl CreateHeaderControl(string headerText)
        => new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new TextBlock
            {
                Text = TextHelper.NormalizeLineBreaks(headerText),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 24,
            },
        };

    public static ContentControl CreateContentControl(
        string contentText,
        FrameworkElement? contentImage)
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        if (contentImage is not null)
        {
            stackPanel.Children.Add(contentImage);
        }

        var textPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center,
        };

        textPanel.Children.Add(
            new TextBlock
            {
                Text = TextHelper.NormalizeLineBreaks(contentText),
                TextWrapping = TextWrapping.Wrap,
            });

        stackPanel.Children.Add(textPanel);

        return new ContentControl
        {
            Content = stackPanel,
        };
    }
}