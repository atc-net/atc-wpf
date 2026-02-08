namespace Atc.Wpf.Clipboard;

/// <summary>
/// Provides clipboard operations with history tracking and
/// real-time change notifications via a Win32 clipboard format listener.
/// </summary>
public sealed class ClipboardService : IClipboardService
{
    private const int WmClipboardUpdate = 0x031D;
    private const int DefaultMaxHistorySize = 25;
    private const int SummaryMaxLength = 100;

    private readonly Dispatcher dispatcher;
    private readonly Lock historyLock = new();
    private readonly List<ClipboardEntry> history = [];

    private HwndSource? hwndSource;
    private bool disposed;

    public ClipboardService(Dispatcher? dispatcher = null)
    {
        this.dispatcher = dispatcher
            ?? Application.Current?.Dispatcher
            ?? Dispatcher.CurrentDispatcher;
    }

    public event EventHandler<ClipboardChangedEventArgs>? ClipboardChanged;

    public IReadOnlyList<ClipboardEntry> History
    {
        get
        {
            lock (historyLock)
            {
                return history.ToList().AsReadOnly();
            }
        }
    }

    public int MaxHistorySize { get; set; } = DefaultMaxHistorySize;

    public bool IsMonitoring => hwndSource is not null;

    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        ExecuteClipboardOperation(() => System.Windows.Clipboard.SetText(text));
        AddHistoryEntry(ClipboardDataType.Text, text, TruncateText(text));
    }

    public string? GetText()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetText);

    public bool ContainsText()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsText);

    public void SetImage(BitmapSource image)
    {
        ArgumentNullException.ThrowIfNull(image);

        ExecuteClipboardOperation(() => System.Windows.Clipboard.SetImage(image));

        var summary = $"Image {(int)image.Width}x{(int)image.Height}";
        AddHistoryEntry(ClipboardDataType.Image, image, summary);
    }

    public BitmapSource? GetImage()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetImage);

    public bool ContainsImage()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsImage);

    public void SetFileDropList(StringCollection fileDropList)
    {
        ArgumentNullException.ThrowIfNull(fileDropList);

        ExecuteClipboardOperation(() => System.Windows.Clipboard.SetFileDropList(fileDropList));

        var summary = $"{fileDropList.Count} file{(fileDropList.Count == 1 ? string.Empty : "s")}";
        AddHistoryEntry(ClipboardDataType.FileDropList, fileDropList, summary);
    }

    public StringCollection? GetFileDropList()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetFileDropList);

    public bool ContainsFileDropList()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsFileDropList);

    public void SetData(
        string format,
        object data)
    {
        ArgumentNullException.ThrowIfNull(format);
        ArgumentNullException.ThrowIfNull(data);

        ExecuteClipboardOperation(() => System.Windows.Clipboard.SetData(format, data));
        AddHistoryEntry(ClipboardDataType.Other, data, $"Format: {format}");
    }

    public object? GetData(string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        return ExecuteClipboardOperation(() => System.Windows.Clipboard.GetData(format));
    }

    public bool ContainsData(string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        return ExecuteClipboardOperation(() => System.Windows.Clipboard.ContainsData(format));
    }

    public void Clear()
        => ExecuteClipboardOperation(System.Windows.Clipboard.Clear);

    public void ClearHistory()
    {
        lock (historyLock)
        {
            history.Clear();
        }
    }

    public void StartMonitoring()
    {
        if (hwndSource is not null)
        {
            return;
        }

        if (dispatcher.CheckAccess())
        {
            CreateListener();
        }
        else
        {
            dispatcher.Invoke(CreateListener);
        }
    }

    public void StopMonitoring()
    {
        if (hwndSource is null)
        {
            return;
        }

        if (dispatcher.CheckAccess())
        {
            DestroyListener();
        }
        else
        {
            dispatcher.Invoke(DestroyListener);
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        StopMonitoring();
    }

    private static string TruncateText(string text)
    {
        var singleLine = text.ReplaceLineEndings(" ");
        return singleLine.Length <= SummaryMaxLength
            ? singleLine
            : string.Concat(singleLine.AsSpan(0, SummaryMaxLength), "...");
    }

    private static void ExecuteClipboardOperation(Action operation)
    {
        try
        {
            operation();
        }
        catch (ExternalException)
        {
            // Clipboard is locked by another process — silently ignore.
        }
    }

    private static T? ExecuteClipboardOperation<T>(Func<T> operation)
    {
        try
        {
            return operation();
        }
        catch (ExternalException)
        {
            return default;
        }
    }

    private void AddHistoryEntry(
        ClipboardDataType dataType,
        object? data,
        string summary)
    {
        var entry = new ClipboardEntry
        {
            DataType = dataType,
            Data = data,
            Timestamp = DateTime.Now,
            Summary = summary,
        };

        lock (historyLock)
        {
            history.Insert(0, entry);

            while (history.Count > MaxHistorySize)
            {
                history.RemoveAt(history.Count - 1);
            }
        }
    }

    private void CreateListener()
    {
        var parameters = new HwndSourceParameters("ClipboardListener")
        {
            ParentWindow = new IntPtr(-3), // HWND_MESSAGE
            Width = 0,
            Height = 0,
        };

        hwndSource = new HwndSource(parameters);
        hwndSource.AddHook(WndProc);

        NativeMethods.AddClipboardFormatListener(hwndSource.Handle);
    }

    private void DestroyListener()
    {
        if (hwndSource is null)
        {
            return;
        }

        NativeMethods.RemoveClipboardFormatListener(hwndSource.Handle);
        hwndSource.RemoveHook(WndProc);
        hwndSource.Dispose();
        hwndSource = null;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Clipboard snapshot requires checking multiple data types.")]
    private IntPtr WndProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg != WmClipboardUpdate)
        {
            return IntPtr.Zero;
        }

        handled = true;

        var entry = CreateSnapshotEntry();
        if (entry is not null)
        {
            lock (historyLock)
            {
                history.Insert(0, entry);

                while (history.Count > MaxHistorySize)
                {
                    history.RemoveAt(history.Count - 1);
                }
            }

            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(entry));
        }

        return IntPtr.Zero;
    }

    private static ClipboardEntry? CreateSnapshotEntry()
    {
        try
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                var text = System.Windows.Clipboard.GetText();
                return new ClipboardEntry
                {
                    DataType = ClipboardDataType.Text,
                    Data = text,
                    Timestamp = DateTime.Now,
                    Summary = TruncateText(text),
                };
            }

            if (System.Windows.Clipboard.ContainsImage())
            {
                var image = System.Windows.Clipboard.GetImage();
                return new ClipboardEntry
                {
                    DataType = ClipboardDataType.Image,
                    Data = image,
                    Timestamp = DateTime.Now,
                    Summary = image is not null
                        ? $"Image {(int)image.Width}x{(int)image.Height}"
                        : "Image",
                };
            }

            if (System.Windows.Clipboard.ContainsFileDropList())
            {
                var files = System.Windows.Clipboard.GetFileDropList();
                return new ClipboardEntry
                {
                    DataType = ClipboardDataType.FileDropList,
                    Data = files,
                    Timestamp = DateTime.Now,
                    Summary = $"{files.Count} file{(files.Count == 1 ? string.Empty : "s")}",
                };
            }

            return new ClipboardEntry
            {
                DataType = ClipboardDataType.Other,
                Timestamp = DateTime.Now,
                Summary = "Other data",
            };
        }
        catch (ExternalException)
        {
            return null;
        }
    }
}