namespace Atc.Wpf.Clipboard;

/// <summary>
/// Represents a single clipboard history entry with type, data, and metadata.
/// </summary>
public sealed class ClipboardEntry
{
    /// <summary>
    /// Gets the type of data stored in the entry.
    /// </summary>
    public ClipboardDataType DataType { get; init; }

    /// <summary>
    /// Gets the clipboard data.
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Gets the time the entry was captured.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets a short human-readable summary of the data.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
        => $"[{Timestamp:HH:mm:ss}] {DataType}: {Summary}";
}