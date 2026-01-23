namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the online status for an Avatar control.
/// </summary>
public enum AvatarStatus
{
    /// <summary>
    /// No status indicator is shown.
    /// </summary>
    None,

    /// <summary>
    /// User is online and available.
    /// </summary>
    Online,

    /// <summary>
    /// User is offline.
    /// </summary>
    Offline,

    /// <summary>
    /// User is away from their device.
    /// </summary>
    Away,

    /// <summary>
    /// User is busy.
    /// </summary>
    Busy,

    /// <summary>
    /// User does not want to be disturbed.
    /// </summary>
    DoNotDisturb,
}
