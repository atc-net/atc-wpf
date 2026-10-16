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
    private int maxHistorySize = DefaultMaxHistorySize;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardService"/> class.
    /// </summary>
    public ClipboardService(Dispatcher? dispatcher = null)
    {
        this.dispatcher = dispatcher
            ?? Application.Current?.Dispatcher
            ?? Dispatcher.CurrentDispatcher;
    }

    /// <inheritdoc />
    public event EventHandler<ClipboardChangedEventArgs>? ClipboardChanged;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public int MaxHistorySize
    {
        get => maxHistorySize;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            lock (historyLock)
            {
                maxHistorySize = value;
                TrimHistory();
            }
        }
    }

    /// <inheritdoc />
    public bool IsMonitoring => hwndSource is not null;

    /// <inheritdoc />
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (ExecuteClipboardOperation(() => System.Windows.Clipboard.SetText(text)))
        {
            AddHistoryEntry(ClipboardDataType.Text, text, TruncateText(text));
        }
    }

    /// <inheritdoc />
    public string? GetText()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetText);

    /// <inheritdoc />
    public bool ContainsText()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsText);

    /// <inheritdoc />
    public void SetImage(BitmapSource image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (ExecuteClipboardOperation(() => System.Windows.Clipboard.SetImage(image)))
        {
            var summary = $"Image {(int)image.Width}x{(int)image.Height}";
            AddHistoryEntry(ClipboardDataType.Image, image, summary);
        }
    }

    /// <inheritdoc />
    public BitmapSource? GetImage()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetImage);

    /// <inheritdoc />
    public bool ContainsImage()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsImage);

    /// <inheritdoc />
    public void SetFileDropList(StringCollection fileDropList)
    {
        ArgumentNullException.ThrowIfNull(fileDropList);

        if (ExecuteClipboardOperation(() => System.Windows.Clipboard.SetFileDropList(fileDropList)))
        {
            var summary = $"{fileDropList.Count} file{(fileDropList.Count == 1 ? string.Empty : "s")}";
            AddHistoryEntry(ClipboardDataType.FileDropList, fileDropList, summary);
        }
    }

    /// <inheritdoc />
    public StringCollection? GetFileDropList()
        => ExecuteClipboardOperation(System.Windows.Clipboard.GetFileDropList);

    /// <inheritdoc />
    public bool ContainsFileDropList()
        => ExecuteClipboardOperation(System.Windows.Clipboard.ContainsFileDropList);

    /// <inheritdoc />
    public void SetData(
        string format,
        object data)
    {
        ArgumentNullException.ThrowIfNull(format);
        ArgumentNullException.ThrowIfNull(data);

        if (ExecuteClipboardOperation(() => System.Windows.Clipboard.SetData(format, data)))
        {
            AddHistoryEntry(ClipboardDataType.Other, data, $"Format: {format}");
        }
    }

    /// <inheritdoc />
    public object? GetData(string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        return ExecuteClipboardOperation(() => System.Windows.Clipboard.GetData(format));
    }

    /// <inheritdoc />
    public bool ContainsData(string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        return ExecuteClipboardOperation(() => System.Windows.Clipboard.ContainsData(format));
    }

    /// <inheritdoc />
    public void Clear()
    {
        ExecuteClipboardOperation(System.Windows.Clipboard.Clear);
    }

    /// <inheritdoc />
    public void ClearHistory()
    {
        lock (historyLock)
        {
            history.Clear();
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <summary>
    /// Runs a clipboard operation on the dispatcher thread (the WPF clipboard needs an STA thread).
    /// </summary>
    /// <returns><see langword="true"/> when the operation ran; <see langword="false"/> when the clipboard was unavailable.</returns>
    private bool ExecuteClipboardOperation(Action operation)
    {
        try
        {
            if (dispatcher.CheckAccess())
            {
                operation();
            }
            else
            {
                dispatcher.Invoke(operation);
            }

            return true;
        }
        catch (Exception ex) when (ex is ExternalException or ThreadStateException)
        {
            // Clipboard is locked by another process or unavailable — silently ignore.
            return false;
        }
    }

    private T? ExecuteClipboardOperation<T>(Func<T> operation)
    {
        try
        {
            return dispatcher.CheckAccess()
                ? operation()
                : dispatcher.Invoke(operation);
        }
        catch (Exception ex) when (ex is ExternalException or ThreadStateException)
        {
            return default;
        }
    }

    private void AddHistoryEntry(
        ClipboardDataType dataType,
        object? data,
        string summary)
    {
        // While monitoring, the clipboard listener records the change (and raises ClipboardChanged),
        // so adding it here as well would duplicate the entry.
        if (IsMonitoring)
        {
            return;
        }

        TryAddHistoryEntry(new ClipboardEntry
        {
            DataType = dataType,
            Data = data,
            Timestamp = DateTime.Now,
            Summary = summary,
        });
    }

    /// <summary>
    /// Adds the entry to the front of the history unless it carries the same content as the newest entry.
    /// Windows raises more than one clipboard update for a single OLE write (set followed by flush),
    /// so an identical consecutive snapshot is treated as an echo of the previous one.
    /// </summary>
    /// <returns><see langword="true"/> when the entry was added; <see langword="false"/> when it duplicated the newest entry.</returns>
    private bool TryAddHistoryEntry(ClipboardEntry entry)
    {
        lock (historyLock)
        {
            if (history.Count > 0 && HasSameContent(history[0], entry))
            {
                return false;
            }

            history.Insert(0, entry);
            TrimHistory();
            return true;
        }
    }

    private static bool HasSameContent(
        ClipboardEntry existing,
        ClipboardEntry candidate)
    {
        if (existing.DataType != candidate.DataType)
        {
            return false;
        }

        return candidate.DataType switch
        {
            ClipboardDataType.Text => string.Equals(existing.Data as string, candidate.Data as string, StringComparison.Ordinal),
            ClipboardDataType.FileDropList => existing.Data is StringCollection existingFiles &&
                                              candidate.Data is StringCollection candidateFiles &&
                                              existingFiles.Cast<string>().SequenceEqual(candidateFiles.Cast<string>(), StringComparer.OrdinalIgnoreCase),
            _ => false,
        };
    }

    /// <summary>
    /// Removes the oldest entries until the history fits <see cref="MaxHistorySize"/>. Call under <see cref="historyLock"/>.
    /// </summary>
    private void TrimHistory()
    {
        if (history.Count > maxHistorySize)
        {
            history.RemoveRange(maxHistorySize, history.Count - maxHistorySize);
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
        if (entry is not null && TryAddHistoryEntry(entry))
        {
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