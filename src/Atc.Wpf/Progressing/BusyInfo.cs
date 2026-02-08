namespace Atc.Wpf.Progressing;

/// <summary>
/// Carries progress information for a busy indicator operation.
/// </summary>
public sealed class BusyInfo
{
    /// <summary>
    /// Gets or sets the message to display.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the progress percentage (0-100), or null for message-only updates.
    /// </summary>
    public int? ProgressPercentage { get; set; }

    /// <summary>
    /// Creates a <see cref="BusyInfo"/> with just a message.
    /// </summary>
    public static BusyInfo FromMessage(string message)
        => new() { Message = message };

    /// <summary>
    /// Creates a <see cref="BusyInfo"/> with a message and progress percentage.
    /// </summary>
    public static BusyInfo FromProgress(
        string message,
        int percentage)
        => new() { Message = message, ProgressPercentage = percentage };

    /// <summary>
    /// Gets the formatted display text, appending the percentage if present.
    /// </summary>
    public string DisplayText
        => ProgressPercentage.HasValue
            ? $"{Message} {ProgressPercentage}%"
            : Message;
}