// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed partial class LiveAudioOutputTester : UserControl, IDisposable
{
    private const int RingCapacity = 200;
    private const int RenderIntervalMilliseconds = 33;
    private const int SineFrequencyHz = 1000;
    private const int FadeMilliseconds = 30;
    private const int SegmentMilliseconds = 1000;
    private const int WavSampleRate = 44100;
    private const double TestAmplitude = 0.4;

    public static readonly DependencyProperty DeviceIdProperty = DependencyProperty.Register(
        nameof(DeviceId),
        typeof(string),
        typeof(LiveAudioOutputTester),
        new PropertyMetadata(defaultValue: null, OnDeviceIdChanged));

    public string? DeviceId
    {
        get => (string?)GetValue(DeviceIdProperty);
        set => SetValue(DeviceIdProperty, value);
    }

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(LiveAudioOutputTester),
        new PropertyMetadata(defaultValue: false, OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private readonly float[] ring = new float[RingCapacity];
    private readonly DispatcherTimer renderTimer;

    private int ringHead;
    private float currentPeak;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed manually in StopAsync.")]
    private Windows.Media.Playback.MediaPlayer? player;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed manually in StopAsync.")]
    private Windows.Storage.Streams.InMemoryRandomAccessStream? wavStream;

    private DateTime playbackStartUtc;
    private bool isPlaying;
    private bool startInProgress;
    private bool disposed;

    public LiveAudioOutputTester()
    {
        InitializeComponent();
        renderTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(RenderIntervalMilliseconds),
        };
        renderTimer.Tick += OnRenderTick;
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
        if (d is LiveAudioOutputTester t)
        {
            _ = t.StopAsync();
        }
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveAudioOutputTester t && !t.IsActive)
        {
            _ = t.StopAsync();
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => _ = StopAsync();

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Test must not crash the picker on permission / device errors.")]
    private async void OnTestButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (isPlaying)
            {
                await StopAsync();
                return;
            }

            if (string.IsNullOrEmpty(DeviceId))
            {
                ShowError(Miscellaneous.AudioPreviewUnavailable);
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
        catch (Exception ex)
        {
            Debug.WriteLine($"[LiveAudioOutputTester] test click: {ex}");
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync();
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Tester must not crash the picker on transient errors.")]
    private async Task StartInternalAsync(string deviceId)
    {
        try
        {
            ClearError();

            var deviceInfo = await Windows.Devices.Enumeration.DeviceInformation
                .CreateFromIdAsync(deviceId);

            var wavBytes = WavGenerator.CreateStereoTestTone(
                sampleRate: WavSampleRate,
                frequencyHz: SineFrequencyHz,
                segmentMilliseconds: SegmentMilliseconds,
                fadeMilliseconds: FadeMilliseconds,
                amplitude: TestAmplitude);

            wavStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            await wavStream.WriteAsync(System.Runtime.InteropServices.WindowsRuntime
                .WindowsRuntimeBufferExtensions.AsBuffer(wavBytes));
            wavStream.Seek(0);

            var source = Windows.Media.Core.MediaSource.CreateFromStream(wavStream, "audio/wav");

            player = new Windows.Media.Playback.MediaPlayer
            {
                AudioDevice = deviceInfo,
                Source = source,
                Volume = 1.0,
            };
            player.MediaFailed += OnMediaFailed;
            player.MediaEnded += OnMediaEnded;

            Debug.WriteLine(
                $"[LiveAudioOutputTester] starting MediaPlayer on '{deviceInfo.Name}', " +
                $"wav bytes={wavBytes.Length}");

            player.Play();

            playbackStartUtc = DateTime.UtcNow;
            renderTimer.Start();

            isPlaying = true;
            UpdateButtonLabel();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LiveAudioOutputTester] start failed: {ex}");
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync();
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Stop must not throw on shutdown.")]
    private async Task StopAsync()
    {
        renderTimer.Stop();
        isPlaying = false;

        var existingPlayer = player;
        player = null;
        if (existingPlayer is not null)
        {
            try
            {
                existingPlayer.MediaFailed -= OnMediaFailed;
                existingPlayer.MediaEnded -= OnMediaEnded;
                existingPlayer.Pause();
                existingPlayer.Source = null;
                existingPlayer.Dispose();
            }
            catch (Exception)
            {
                // Best-effort cleanup.
            }
        }

        var existingStream = wavStream;
        wavStream = null;
        if (existingStream is not null)
        {
            try
            {
                existingStream.Dispose();
            }
            catch (Exception)
            {
                // Best-effort cleanup.
            }
        }

        await Dispatcher.InvokeAsync(() =>
        {
            ringHead = 0;
            currentPeak = 0;
            Array.Clear(ring, 0, ring.Length);
            PartTopLine.Points.Clear();
            PartBottomLine.Points.Clear();
            PartPeakBar.Height = 0;
            UpdateButtonLabel();
        });
    }

    private void OnMediaFailed(
        Windows.Media.Playback.MediaPlayer sender,
        Windows.Media.Playback.MediaPlayerFailedEventArgs args)
    {
        Debug.WriteLine(
            $"[LiveAudioOutputTester] MediaPlayer failed: error={args.Error} " +
            $"hr=0x{args.ExtendedErrorCode?.HResult:X8} message={args.ErrorMessage}");
        _ = Dispatcher.BeginInvoke(new Action(() =>
        {
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            _ = StopAsync();
        }));
    }

    private void OnMediaEnded(
        Windows.Media.Playback.MediaPlayer sender,
        object args)
    {
        Debug.WriteLine("[LiveAudioOutputTester] MediaEnded");
        _ = Dispatcher.BeginInvoke(new Action(() => _ = StopAsync()));
    }

    private void OnRenderTick(
        object? sender,
        EventArgs e)
    {
        // Synthesize a visualisation that mirrors what the WAV is playing in real time.
        // We can't tap into MediaPlayer's render pipeline, so we recompute the same
        // envelope locally based on wall-clock elapsed since play started.
        if (!isPlaying)
        {
            return;
        }

        var elapsedMs = (DateTime.UtcNow - playbackStartUtc).TotalMilliseconds;
        var samplesPerSegment = WavSampleRate * SegmentMilliseconds / 1000;
        var totalDurationMs = SegmentMilliseconds * 3;
        if (elapsedMs >= totalDurationMs)
        {
            // Tone is over; let the engine drain naturally and OnMediaEnded will stop.
            ringHead = (ringHead + 1) % RingCapacity;
            ring[ringHead] = 0;
            currentPeak = 0;
            RenderWaveform();
            return;
        }

        var samplePosition = (long)(elapsedMs * WavSampleRate / 1000);
        var segment = samplePosition / samplesPerSegment;
        var posInSegment = samplePosition - (segment * samplesPerSegment);
        var fadeSamples = WavSampleRate * FadeMilliseconds / 1000;

        var envelope = posInSegment < fadeSamples
            ? posInSegment / (double)fadeSamples
            : (samplesPerSegment - posInSegment) < fadeSamples
                ? (samplesPerSegment - posInSegment) / (double)fadeSamples
                : 1.0;

        var peak = (float)(TestAmplitude * envelope);
        ring[ringHead] = peak;
        ringHead = (ringHead + 1) % RingCapacity;
        currentPeak = peak;

        RenderWaveform();
    }

    private void OnCanvasSizeChanged(
        object sender,
        SizeChangedEventArgs e)
        => RenderWaveform();

    private void RenderWaveform()
    {
        var width = PartCanvas.ActualWidth;
        var height = PartCanvas.ActualHeight;
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var midY = height / 2;
        var halfHeight = (height / 2) - 2;

        var topPoints = new PointCollection(RingCapacity);
        var bottomPoints = new PointCollection(RingCapacity);

        for (var i = 0; i < RingCapacity; i++)
        {
            var index = (ringHead + i) % RingCapacity;
            var sample = ring[index];
            if (sample > 1f)
            {
                sample = 1f;
            }

            var x = (i / (double)(RingCapacity - 1)) * (width - 12);
            topPoints.Add(new System.Windows.Point(x, midY - (sample * halfHeight)));
            bottomPoints.Add(new System.Windows.Point(x, midY + (sample * halfHeight)));
        }

        PartTopLine.Points = topPoints;
        PartBottomLine.Points = bottomPoints;

        var peakHeight = currentPeak * height;
        PartPeakBar.Height = peakHeight;
        Canvas.SetLeft(PartPeakBar, width - 8);
        Canvas.SetTop(PartPeakBar, height - peakHeight);
    }

    private void UpdateButtonLabel()
        => PartTestButton.Content = isPlaying ? Miscellaneous.Stop : Miscellaneous.Test;

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
}