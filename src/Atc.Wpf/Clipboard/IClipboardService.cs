namespace Atc.Wpf.Clipboard;

/// <summary>
/// Provides an MVVM-friendly abstraction over system clipboard operations
/// with support for text, image, and file data, history tracking,
/// and real-time change notifications via Win32 clipboard listener.
/// </summary>
public interface IClipboardService : IDisposable
{
    // Text

    /// <summary>
    /// Places the specified text on the clipboard and adds it to the history.
    /// </summary>
    void SetText(string text);

    /// <summary>
    /// Gets the text currently on the clipboard.
    /// </summary>
    string? GetText();

    /// <summary>
    /// Determines whether the clipboard contains text.
    /// </summary>
    bool ContainsText();

    // Image

    /// <summary>
    /// Places the specified image on the clipboard and adds it to the history.
    /// </summary>
    void SetImage(BitmapSource image);

    /// <summary>
    /// Gets the image currently on the clipboard.
    /// </summary>
    BitmapSource? GetImage();

    /// <summary>
    /// Determines whether the clipboard contains an image.
    /// </summary>
    bool ContainsImage();

    // File drop list

    /// <summary>
    /// Places the specified file drop list on the clipboard and adds it to the history.
    /// </summary>
    void SetFileDropList(StringCollection fileDropList);

    /// <summary>
    /// Gets the file drop list currently on the clipboard.
    /// </summary>
    StringCollection? GetFileDropList();

    /// <summary>
    /// Determines whether the clipboard contains a file drop list.
    /// </summary>
    bool ContainsFileDropList();

    // Generic

    /// <summary>
    /// Places data in the specified format on the clipboard and adds it to the history.
    /// </summary>
    void SetData(
        string format,
        object data);

    /// <summary>
    /// Gets the clipboard data in the specified format.
    /// </summary>
    object? GetData(string format);

    /// <summary>
    /// Determines whether the clipboard contains data in the specified format.
    /// </summary>
    bool ContainsData(string format);

    // Clear

    /// <summary>
    /// Clears the system clipboard.
    /// </summary>
    void Clear();

    // History

    /// <summary>
    /// Gets a snapshot of the captured clipboard history entries.
    /// </summary>
    IReadOnlyList<ClipboardEntry> History { get; }

    /// <summary>
    /// Gets or sets the maximum number of entries kept in the history.
    /// </summary>
    int MaxHistorySize { get; set; }

    /// <summary>
    /// Removes all entries from the clipboard history.
    /// </summary>
    void ClearHistory();

    // Monitoring

    /// <summary>
    /// Starts listening for system clipboard changes.
    /// </summary>
    void StartMonitoring();

    /// <summary>
    /// Stops listening for system clipboard changes.
    /// </summary>
    void StopMonitoring();

    /// <summary>
    /// Gets a value indicating whether the service is listening for clipboard changes.
    /// </summary>
    bool IsMonitoring { get; }

    /// <summary>
    /// Occurs when the system clipboard content changes while monitoring.
    /// </summary>
    event EventHandler<ClipboardChangedEventArgs>? ClipboardChanged;
}