// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed partial class LiveCameraPreview : UserControl, IDisposable, IAsyncDisposable
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

    private readonly DeviceSessionCoordinator session;
    private readonly LatestFrameSlot<CameraPreviewFrame> frameSlot = new();
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by the teardown Dispose starts (StopAsync / DisposeMediaAsync).")]
    private Windows.Media.Capture.MediaCapture? mediaCapture;
    private Windows.Media.Capture.Frames.MediaFrameReader? frameReader;
    private WriteableBitmap? renderTarget;
    private bool isInVisualTree;
    private bool disposed;
    private Task? teardown;

    public LiveCameraPreview()
    {
        InitializeComponent();
        session = new DeviceSessionCoordinator(StartInternalAsync, StopAsync, GetDesiredDeviceId);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Stops the device without blocking. The stop awaits work that resumes on the UI thread, so waiting for it
    /// here would deadlock when disposed from that thread; use <see cref="DisposeAsync"/> to await it.
    /// </summary>
    public void Dispose()
        => _ = BeginTeardown();

    /// <summary>
    /// Stops the device and completes once it has been released.
    /// </summary>
    public ValueTask DisposeAsync()
        => new(BeginTeardown());

    private Task BeginTeardown()
    {
        if (teardown is not null)
        {
            return teardown;
        }

        disposed = true;
        teardown = LiveDeviceTeardown.Start(Dispatcher, session.SyncAsync, StopAsync);
        return teardown;
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
    {
        isInVisualTree = true;
        _ = RestartAsync();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        isInVisualTree = false;
        _ = RestartAsync();
    }

    /// <summary>
    /// Re-evaluates whether the camera should run. Routed through <see cref="DeviceSessionCoordinator"/> so an
    /// unload or device change that happens while the camera is still initializing is not lost.
    /// </summary>
    private Task RestartAsync()
        => session.SyncAsync();

    private string? GetDesiredDeviceId()
        => IsActive && !string.IsNullOrEmpty(DeviceId) && isInVisualTree && !disposed
            ? DeviceId
            : null;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Preview must not crash the picker on permission / device errors.")]
    private async Task StartInternalAsync(string deviceId)
    {
        try
        {
            ClearError();

            var capture = new Windows.Media.Capture.MediaCapture();

            // Own it before initialising, so a failed InitializeAsync (access denied, camera busy) is disposed
            // by DisposeMediaAsync instead of holding the camera until finalisation.
            mediaCapture = capture;
            await capture.InitializeAsync(new Windows.Media.Capture.MediaCaptureInitializationSettings
            {
                VideoDeviceId = deviceId,
                StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video,
                MemoryPreference = Windows.Media.Capture.MediaCaptureMemoryPreference.Cpu,
            });

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

            var reader = await CreateFrameReaderAsync(capture, colorSource);
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

                // Not on the UI thread: the rest of the stop is not UI work, so it must not wait for a dispatcher
                // that may be blocked or shutting down (the coordinator resumes on the UI thread itself).
                await reader.StopAsync().AsTask().ConfigureAwait(false);
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

        // Release a frame still waiting for the UI before the capture device goes away.
        frameSlot.Clear();

        await DisposeMediaAsync().ConfigureAwait(false);

        LiveDeviceTeardown.OnUiThread(Dispatcher, () =>
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
        // The UI has not shown the previous frame yet: leave this one to the reader, which drops it, instead of
        // acquiring and holding a frame that would only replace the pending one.
        if (frameSlot.HasPendingFrame)
        {
            return;
        }

        CameraPreviewFrame? preview = null;
        try
        {
            var frame = sender.TryAcquireLatestFrame();
            preview = frame is null ? null : CameraPreviewFrame.From(frame);
            if (preview is null)
            {
                return;
            }

            // Only one render is queued at a time; frames arriving meanwhile replace the pending one.
            var scheduleRender = frameSlot.Publish(preview);
            preview = null; // Owned by the slot now.
            if (scheduleRender)
            {
                _ = Dispatcher.BeginInvoke(new Action(RenderLatestFrame));
            }
        }
        catch (Exception)
        {
            // Skip this frame.
        }
        finally
        {
            preview?.Dispose();
        }
    }

    /// <summary>
    /// Asks the capture pipeline for BGRA8 frames at the preview's size, so neither the pixel conversion nor
    /// the downscale of every full-resolution frame runs on the CPU here. Falls back to the camera's own
    /// format when the device refuses.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Fall back to the native format on any device error.")]
    private async Task<Windows.Media.Capture.Frames.MediaFrameReader> CreateFrameReaderAsync(
        Windows.Media.Capture.MediaCapture capture,
        Windows.Media.Capture.Frames.MediaFrameSource colorSource)
    {
        try
        {
            var format = colorSource.CurrentFormat?.VideoFormat;
            var size = format is null
                ? null
                : CameraPreviewFormat.GetReaderSize(format.Width, format.Height, GetPreviewPixelHeight());

            return size is { } readerSize
                ? await capture.CreateFrameReaderAsync(
                    colorSource,
                    Windows.Media.MediaProperties.MediaEncodingSubtypes.Bgra8,
                    new Windows.Graphics.Imaging.BitmapSize { Width = readerSize.Width, Height = readerSize.Height })
                : await capture.CreateFrameReaderAsync(
                    colorSource,
                    Windows.Media.MediaProperties.MediaEncodingSubtypes.Bgra8);
        }
        catch (Exception)
        {
            return await capture.CreateFrameReaderAsync(colorSource);
        }
    }

    private double GetPreviewPixelHeight()
        => Dispatcher.Invoke(() =>
        {
            var height = ActualHeight > 0
                ? ActualHeight
                : double.IsNaN(Height) ? 0 : Height;

            return height > 0
                ? height * VisualTreeHelper.GetDpi(this).DpiScaleY
                : CameraPreviewFormat.FallbackPreviewPixelHeight;
        });

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Frame copy must not crash on transient errors.")]
    private void RenderLatestFrame()
    {
        if (!frameSlot.TryTake(out var frame))
        {
            return;
        }

        using (frame)
        {
            try
            {
                RenderFrame(frame);
            }
            catch (Exception)
            {
                // Skip this frame.
            }
        }
    }

    private void RenderFrame(CameraPreviewFrame frame)
    {
        if (disposed)
        {
            return;
        }

        var width = frame.Bitmap.PixelWidth;
        var height = frame.Bitmap.PixelHeight;
        var pixelFormat = CameraPreviewFormat.ToWpfPixelFormat(frame.Bitmap.BitmapAlphaMode);

        if (renderTarget is null ||
            renderTarget.PixelWidth != width ||
            renderTarget.PixelHeight != height ||
            renderTarget.Format != pixelFormat)
        {
            renderTarget = new WriteableBitmap(
                width,
                height,
                dpiX: 96,
                dpiY: 96,
                pixelFormat: pixelFormat,
                palette: null);

            PartImage.Source = renderTarget;
        }

        renderTarget.Lock();
        try
        {
            // The only copy of the frame: from the capture buffer straight into the back buffer.
            if (frame.CopyTo(renderTarget))
            {
                renderTarget.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
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