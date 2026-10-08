namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// The native side of a <see cref="TrayIcon"/>: the notification area icon and the hidden window
/// that receives its callback messages.
/// </summary>
internal interface ITrayIconHost : IDisposable
{
    /// <summary>
    /// Gets or sets the callback for a user action on the icon.
    /// </summary>
    Action<TrayIconAction>? ActionReceived { get; set; }

    /// <summary>
    /// Gets or sets the callback for an Explorer restart, after which the icon must be added again.
    /// </summary>
    Action? TaskbarCreated { get; set; }

    /// <summary>
    /// Adds the icon to the notification area.
    /// </summary>
    /// <returns><see langword="true"/> if the shell accepted the icon.</returns>
    bool Add(
        ImageSource? icon,
        string toolTip);

    /// <summary>
    /// Updates the image and tooltip of the added icon.
    /// </summary>
    void Update(
        ImageSource? icon,
        string toolTip);

    /// <summary>
    /// Brings the given window to the foreground, so an open context menu closes when the user clicks elsewhere.
    /// </summary>
    void SetForegroundWindow(nint handle);
}