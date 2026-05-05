// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed partial class LiveCameraPreview : UserControl, IDisposable
{
    public static readonly DependencyProperty DeviceIdProperty = DependencyProperty.Register(
        nameof(DeviceId),
        typeof(string),
        typeof(LiveCameraPreview),
        new PropertyMetadata(defaultValue: null, OnDeviceIdChanged));

    public string? DeviceId
    {
        get => (string?)GetValue(DeviceIdProperty);
        set => SetValue(DeviceIdProperty, value);
    }

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(LiveCameraPreview),
        new PropertyMetadata(defaultValue: false, OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly DependencyProperty PreferredFormatProperty = DependencyProperty.Register(
        nameof(PreferredFormat),
        typeof(UsbCameraFormat),
        typeof(LiveCameraPreview),
        new PropertyMetadata(defaultValue: null, OnPreferredFormatChanged));

    public UsbCameraFormat? PreferredFormat
    {
        get => (UsbCameraFormat?)GetValue(PreferredFormatProperty);
        set => SetValue(PreferredFormatProperty, value);
    }

    public event EventHandler<CameraFormatsAvailableEventArgs>? FormatsAvailable;

    private Windows.Media.Capture.MediaCapture? mediaCapture;
    private Windows.Media.Capture.Frames.MediaFrameReader? frameReader;
    private WriteableBitmap? renderTarget;
    private bool startInProgress;
    private bool disposed;

    public LiveCameraPreview()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        StopAsync().GetAwaiter().GetResult();
    }

    private static void OnDeviceIdChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveCameraPreview p)
        {
            _ = p.RestartAsync();
        }
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveCameraPreview p)
        {
            _ = p.RestartAsync();
        }
    }

    private static void OnPreferredFormatChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveCameraPreview p)
        {
            _ = p.RestartAsync();
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => _ = RestartAsync();

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => _ = StopAsync();

    private async Task RestartAsync()
    {
        await StopAsync();

        if (!IsActive ||
            string.IsNullOrEmpty(DeviceId) ||
            !IsLoaded ||
            disposed)
        {
            return;
        }

        if (startInProgress)
        {
            return;
        }

        startInProgress = true;
        try
        {
            await StartInternalAsync(DeviceId);
        }
        finally
        {
            startInProgress = false;
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Preview must not crash the picker on permission / device errors.")]
    private async Task StartInternalAsync(string deviceId)
    {
        try
        {
            ClearError();

            var capture = new Windows.Media.Capture.MediaCapture();
            await capture.InitializeAsync(new Windows.Media.Capture.MediaCaptureInitializationSettings
            {
                VideoDeviceId = deviceId,
                StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
                MemoryPreference = Windows.Media.Capture.MediaCaptureMemoryPreference.Cpu,
            });

            mediaCapture = capture;

            var colorSource = capture.FrameSources.Values
                .FirstOrDefault(s => s.Info.SourceKind is Windows.Media.Capture.Frames.MediaFrameSourceKind.Color);

            if (colorSource is null)
            {
                ShowError(Miscellaneous.PreviewUnavailable);
                return;
            }

            var formats = ToCameraFormats(colorSource.SupportedFormats);
            if (formats.Count > 0)
            {
                FormatsAvailable?.Invoke(this, new CameraFormatsAvailableEventArgs(formats));
            }

            await TryApplyPreferredFormatAsync(colorSource);

            var reader = await capture.CreateFrameReaderAsync(colorSource);
            reader.FrameArrived += OnFrameArrived;
            var status = await reader.StartAsync();
            if (status is not Windows.Media.Capture.Frames.MediaFrameReaderStartStatus.Success)
            {
                reader.FrameArrived -= OnFrameArrived;
                reader.Dispose();
                ShowError(Miscellaneous.PreviewUnavailable);
                return;
            }

            frameReader = reader;
        }
        catch (UnauthorizedAccessException)
        {
            ShowError(Miscellaneous.PreviewPermissionDenied);
            await DisposeMediaAsync();
        }
        catch (Exception)
        {
            ShowError(Miscellaneous.PreviewUnavailable);
            await DisposeMediaAsync();
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Stop must not throw on shutdown.")]
    private async Task StopAsync()
    {
        var reader = frameReader;
        frameReader = null;

        if (reader is not null)
        {
            try
            {
                reader.FrameArrived -= OnFrameArrived;
                await reader.StopAsync();
            }
            catch (Exception)
            {
                // Already stopped or device gone.
            }
            finally
            {
                reader.Dispose();
            }
        }

        await DisposeMediaAsync();

        await Dispatcher.InvokeAsync(() =>
        {
            renderTarget = null;
            PartImage.Source = null;
        });
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Dispose must not throw on shutdown.")]
    private Task DisposeMediaAsync()
    {
        var capture = mediaCapture;
        mediaCapture = null;

        if (capture is not null)
        {
            try
            {
                capture.Dispose();
            }
            catch (Exception)
            {
                // Best-effort cleanup.
            }
        }

        return Task.CompletedTask;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Frame copy must not crash on transient errors.")]
    private void OnFrameArrived(
        Windows.Media.Capture.Frames.MediaFrameReader sender,
        Windows.Media.Capture.Frames.MediaFrameArrivedEventArgs args)
    {
        try
        {
            using var frame = sender.TryAcquireLatestFrame();
            var softwareBitmap = frame?.VideoMediaFrame?.SoftwareBitmap;
            if (softwareBitmap is null)
            {
                return;
            }

            var converted = NormalizeBitmap(softwareBitmap);
            try
            {
                var width = converted.PixelWidth;
                var height = converted.PixelHeight;
                var bytes = new byte[width * height * 4];

                converted.CopyToBuffer(System.Runtime.InteropServices.WindowsRuntime
                    .WindowsRuntimeBufferExtensions.AsBuffer(bytes));

                _ = Dispatcher.BeginInvoke(new Action(() => RenderFrame(bytes, width, height)));
            }
            finally
            {
                if (!ReferenceEquals(converted, softwareBitmap))
                {
                    converted.Dispose();
                }
            }
        }
        catch (Exception)
        {
            // Skip this frame.
        }
    }

    private static Windows.Graphics.Imaging.SoftwareBitmap NormalizeBitmap(
        Windows.Graphics.Imaging.SoftwareBitmap source)
    {
        if (source.BitmapPixelFormat is Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8 &&
            source.BitmapAlphaMode is Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied)
        {
            return source;
        }

        return Windows.Graphics.Imaging.SoftwareBitmap.Convert(
            source,
            Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
            Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
    }

    private void RenderFrame(
        byte[] bytes,
        int width,
        int height)
    {
        if (disposed)
        {
            return;
        }

        if (renderTarget is null ||
            renderTarget.PixelWidth != width ||
            renderTarget.PixelHeight != height)
        {
            renderTarget = new WriteableBitmap(
                width,
                height,
                dpiX: 96,
                dpiY: 96,
                pixelFormat: PixelFormats.Bgra32,
                palette: null);

            PartImage.Source = renderTarget;
        }

        renderTarget.Lock();
        try
        {
            Marshal.Copy(bytes, 0, renderTarget.BackBuffer, bytes.Length);
            renderTarget.AddDirtyRect(new Int32Rect(0, 0, width, height));
        }
        finally
        {
            renderTarget.Unlock();
        }
    }

    private void ClearError()
        => Dispatcher.Invoke(() =>
        {
            PartError.Visibility = Visibility.Collapsed;
            PartError.Text = string.Empty;
        });

    private void ShowError(string text)
        => Dispatcher.Invoke(() =>
        {
            PartError.Text = text;
            PartError.Visibility = Visibility.Visible;
        });

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Format enumeration must not crash the preview on transient errors.")]
    private static IReadOnlyList<UsbCameraFormat> ToCameraFormats(
        IReadOnlyList<Windows.Media.Capture.Frames.MediaFrameFormat> source)
    {
        var formats = new List<UsbCameraFormat>(source.Count);
        foreach (var format in source)
        {
            try
            {
                if (format.VideoFormat is null)
                {
                    continue;
                }

                var fps = format.FrameRate.Denominator == 0
                    ? 0
                    : format.FrameRate.Numerator / (double)format.FrameRate.Denominator;

                formats.Add(new UsbCameraFormat(
                    Width: format.VideoFormat.Width,
                    Height: format.VideoFormat.Height,
                    FrameRate: fps,
                    Subtype: format.Subtype ?? string.Empty));
            }
            catch (Exception)
            {
                // Skip malformed format entries.
            }
        }

        return formats
            .Distinct()
            .OrderByDescending(f => f.Width)
            .ThenByDescending(f => f.Height)
            .ThenByDescending(f => f.FrameRate)
            .ToArray();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "SetFormatAsync may legitimately fail; the picker should keep working at the device default.")]
    private async Task TryApplyPreferredFormatAsync(
        Windows.Media.Capture.Frames.MediaFrameSource colorSource)
    {
        var preferred = PreferredFormat;
        if (preferred is null)
        {
            return;
        }

        var match = colorSource.SupportedFormats.FirstOrDefault(f =>
            f.VideoFormat is not null &&
            f.VideoFormat.Width == preferred.Width &&
            f.VideoFormat.Height == preferred.Height &&
            f.FrameRate.Denominator != 0 &&
            System.Math.Abs((f.FrameRate.Numerator / (double)f.FrameRate.Denominator) - preferred.FrameRate) < 0.5);

        if (match is null)
        {
            return;
        }

        try
        {
            await colorSource.SetFormatAsync(match);
        }
        catch (Exception)
        {
            // Falling back to the device default is acceptable.
        }
    }
}