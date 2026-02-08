namespace Atc.Wpf.Clipboard;

/// <summary>
/// Represents a single clipboard history entry with type, data, and metadata.
/// </summary>
public sealed class ClipboardEntry
{
    public ClipboardDataType DataType { get; init; }

    public object? Data { get; init; }

    public DateTime Timestamp { get; init; }

    public string Summary { get; init; } = string.Empty;

    public override string ToString()
        => $"[{Timestamp:HH:mm:ss}] {DataType}: {Summary}";
}