namespace Atc.Wpf.Sample.SamplesWpf.Clipboard;

public sealed partial class ClipboardServiceViewModel : ViewModelBase, IDisposable
{
    private readonly IClipboardService clipboardService = new ClipboardService();

    private string statusText = "(none)";
    private string? pastedText;
    private BitmapSource? pastedImage;
    private ObservableCollection<Atc.Wpf.Clipboard.ClipboardEntry> historyEntries = [];
    private ObservableCollection<string> monitorLog = [];

    public ClipboardServiceViewModel()
    {
        clipboardService.ClipboardChanged += OnClipboardChanged;
    }

    public string StatusText
    {
        get => statusText;
        set
        {
            statusText = value;
            RaisePropertyChanged();
        }
    }

    public string? PastedText
    {
        get => pastedText;
        set
        {
            pastedText = value;
            RaisePropertyChanged();
        }
    }

    public BitmapSource? PastedImage
    {
        get => pastedImage;
        set
        {
            pastedImage = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<Atc.Wpf.Clipboard.ClipboardEntry> HistoryEntries
    {
        get => historyEntries;
        set
        {
            historyEntries = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<string> MonitorLog
    {
        get => monitorLog;
        set
        {
            monitorLog = value;
            RaisePropertyChanged();
        }
    }

    public bool IsMonitoring => clipboardService.IsMonitoring;

    [RelayCommand]
    private void CopyText()
    {
        clipboardService.SetText("Hello from ClipboardService! This is a sample text copied at " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
        StatusText = "Text copied to clipboard";
        RefreshHistory();
    }

    [RelayCommand]
    private void PasteText()
    {
        if (clipboardService.ContainsText())
        {
            PastedText = clipboardService.GetText();
            StatusText = "Text pasted from clipboard";
        }
        else
        {
            PastedText = null;
            StatusText = "No text on clipboard";
        }
    }

    [RelayCommand]
    private void CopyImage()
    {
        var bitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            context.DrawRectangle(Brushes.CornflowerBlue, pen: null, new Rect(0, 0, 200, 200));
            context.DrawEllipse(Brushes.White, pen: null, new Point(100, 100), 60, 60);
            var formattedText = new FormattedText(
                "Sample",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                24,
                Brushes.CornflowerBlue,
                96);
            context.DrawText(formattedText, new Point(100 - (formattedText.Width / 2), 100 - (formattedText.Height / 2)));
        }

        bitmap.Render(visual);
        bitmap.Freeze();

        clipboardService.SetImage(bitmap);
        StatusText = "Image copied to clipboard";
        RefreshHistory();
    }

    [RelayCommand]
    private void PasteImage()
    {
        if (clipboardService.ContainsImage())
        {
            PastedImage = clipboardService.GetImage();
            StatusText = "Image pasted from clipboard";
        }
        else
        {
            PastedImage = null;
            StatusText = "No image on clipboard";
        }
    }

    [RelayCommand]
    private void ClearClipboard()
    {
        clipboardService.Clear();
        PastedText = null;
        PastedImage = null;
        StatusText = "Clipboard cleared";
    }

    [RelayCommand]
    private void ToggleMonitoring()
    {
        if (clipboardService.IsMonitoring)
        {
            clipboardService.StopMonitoring();
            MonitorLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Monitoring stopped");
            StatusText = "Monitoring stopped";
        }
        else
        {
            clipboardService.StartMonitoring();
            MonitorLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Monitoring started");
            StatusText = "Monitoring started — copy something to see events";
        }

        RaisePropertyChanged(nameof(IsMonitoring));
    }

    [RelayCommand]
    private void ClearHistory()
    {
        clipboardService.ClearHistory();
        HistoryEntries.Clear();
        StatusText = "History cleared";
    }

    [RelayCommand]
    private void ClearLog()
    {
        MonitorLog.Clear();
    }

    public void Dispose()
    {
        clipboardService.ClipboardChanged -= OnClipboardChanged;
        clipboardService.Dispose();
    }

    private void OnClipboardChanged(
        object? sender,
        ClipboardChangedEventArgs e)
    {
        var message = $"[{e.Entry.Timestamp:HH:mm:ss}] {e.Entry.DataType}: {e.Entry.Summary}";
        MonitorLog.Insert(0, message);
        RefreshHistory();
    }

    private void RefreshHistory()
    {
        HistoryEntries = new ObservableCollection<Atc.Wpf.Clipboard.ClipboardEntry>(clipboardService.History);
    }
}