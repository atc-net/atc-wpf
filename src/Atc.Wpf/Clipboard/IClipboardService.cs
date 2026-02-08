namespace Atc.Wpf.Clipboard;

/// <summary>
/// Provides an MVVM-friendly abstraction over system clipboard operations
/// with support for text, image, and file data, history tracking,
/// and real-time change notifications via Win32 clipboard listener.
/// </summary>
public interface IClipboardService : IDisposable
{
    // Text
    void SetText(string text);

    string? GetText();

    bool ContainsText();

    // Image
    void SetImage(BitmapSource image);

    BitmapSource? GetImage();

    bool ContainsImage();

    // File drop list
    void SetFileDropList(StringCollection fileDropList);

    StringCollection? GetFileDropList();

    bool ContainsFileDropList();

    // Generic
    void SetData(
        string format,
        object data);

    object? GetData(string format);

    bool ContainsData(string format);

    // Clear
    void Clear();

    // History
    IReadOnlyList<ClipboardEntry> History { get; }

    int MaxHistorySize { get; set; }

    void ClearHistory();

    // Monitoring
    void StartMonitoring();

    void StopMonitoring();

    bool IsMonitoring { get; }

    event EventHandler<ClipboardChangedEventArgs>? ClipboardChanged;
}