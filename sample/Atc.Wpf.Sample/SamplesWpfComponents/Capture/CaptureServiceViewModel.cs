namespace Atc.Wpf.Sample.SamplesWpfComponents.Capture;

public sealed partial class CaptureServiceViewModel : ViewModelBase
{
    private readonly ICaptureService captureService = new CaptureService();

    [ObservableProperty]
    private string resultLog = string.Empty;

    [ObservableProperty]
    private ImageFormatType selectedFormat = ImageFormatType.Png;

    [ObservableProperty]
    private double dpi = 96;

    public IReadOnlyList<ImageFormatType> Formats { get; } =
    [
        ImageFormatType.Png,
        ImageFormatType.Jpeg,
        ImageFormatType.Bmp,
        ImageFormatType.Gif,
        ImageFormatType.Tiff,
    ];

    [RelayCommand]
    private void CaptureToFile(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = GetFileFilter(),
            DefaultExt = GetDefaultExtension(),
            FileName = $"capture.{GetDefaultExtension()}",
        };

        if (dialog.ShowDialog() != true)
        {
            AppendLog("Capture to file: Cancelled");
            return;
        }

        var settings = CreateSettings();
        var bitmap = captureService.CaptureVisual(visual, settings);
        var result = captureService.SaveToFile(bitmap, dialog.FileName, SelectedFormat);
        AppendLog($"Capture to file: {result}");
    }

    [RelayCommand]
    private void CaptureToClipboard(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        var bitmap = captureService.CaptureVisual(visual, settings);
        var result = captureService.CopyToClipboard(bitmap);
        AppendLog($"Capture to clipboard: {result}");
    }

    [RelayCommand]
    private void CaptureRegionToFile(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = GetFileFilter(),
            DefaultExt = GetDefaultExtension(),
            FileName = $"region.{GetDefaultExtension()}",
        };

        if (dialog.ShowDialog() != true)
        {
            AppendLog("Capture region to file: Cancelled");
            return;
        }

        var settings = CreateSettings();
        var region = new Int32Rect(0, 0, 200, 100);
        var bitmap = captureService.CaptureRegion(visual, region, settings);
        var result = captureService.SaveToFile(bitmap, dialog.FileName, SelectedFormat);
        AppendLog($"Capture region (200x100): {result}");
    }

    [RelayCommand]
    private void CaptureRegionToClipboard(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        var region = new Int32Rect(0, 0, 200, 100);
        var bitmap = captureService.CaptureRegion(visual, region, settings);
        var result = captureService.CopyToClipboard(bitmap);
        AppendLog($"Capture region (200x100) to clipboard: {result}");
    }

    [RelayCommand]
    private void CaptureWindowToFile()
    {
        var dialog = new SaveFileDialog
        {
            Filter = GetFileFilter(),
            DefaultExt = GetDefaultExtension(),
            FileName = $"window.{GetDefaultExtension()}",
        };

        if (dialog.ShowDialog() != true)
        {
            AppendLog("Capture window to file: Cancelled");
            return;
        }

        var window = Application.Current.MainWindow;
        if (window is null)
        {
            AppendLog("Capture window to file: No main window");
            return;
        }

        var settings = CreateSettings();
        var bitmap = captureService.CaptureWindow(window, settings);
        var result = captureService.SaveToFile(bitmap, dialog.FileName, SelectedFormat);
        AppendLog($"Capture window: {result}");
    }

    [RelayCommand]
    private void CaptureWindowToClipboard()
    {
        var window = Application.Current.MainWindow;
        if (window is null)
        {
            AppendLog("Capture window to clipboard: No main window");
            return;
        }

        var settings = CreateSettings();
        var bitmap = captureService.CaptureWindow(window, settings);
        var result = captureService.CopyToClipboard(bitmap);
        AppendLog($"Capture window to clipboard: {result}");
    }

    private CaptureSettings CreateSettings()
        => new()
        {
            DpiX = Dpi,
            DpiY = Dpi,
        };

    private string GetDefaultExtension()
        => SelectedFormat switch
        {
            ImageFormatType.Jpeg => "jpg",
            ImageFormatType.Bmp => "bmp",
            ImageFormatType.Gif => "gif",
            ImageFormatType.Tiff => "tiff",
            _ => "png",
        };

    private string GetFileFilter()
        => SelectedFormat switch
        {
            ImageFormatType.Jpeg => "JPEG files (*.jpg)|*.jpg",
            ImageFormatType.Bmp => "Bitmap files (*.bmp)|*.bmp",
            ImageFormatType.Gif => "GIF files (*.gif)|*.gif",
            ImageFormatType.Tiff => "TIFF files (*.tiff)|*.tiff",
            _ => "PNG files (*.png)|*.png",
        };

    private void AppendLog(string entry)
        => ResultLog = $"[{DateTime.Now:T}] {entry}{Environment.NewLine}{ResultLog}";
}