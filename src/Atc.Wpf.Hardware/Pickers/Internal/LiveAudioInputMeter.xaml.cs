// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed partial class LiveAudioInputMeter : UserControl, IDisposable
{
    private const int RingCapacity = 200;
    private const int RenderIntervalMilliseconds = 33;

    public static readonly DependencyProperty DeviceIdProperty = DependencyProperty.Register(
        nameof(DeviceId),
        typeof(string),
        typeof(LiveAudioInputMeter),
        new PropertyMetadata(defaultValue: null, OnDeviceIdChanged));

    public string? DeviceId
    {
        get => (string?)GetValue(DeviceIdProperty);
        set => SetValue(DeviceIdProperty, value);
    }

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(LiveAudioInputMeter),
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

    private Windows.Media.Audio.AudioGraph? graph;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "AudioGraph.Dispose disposes its child nodes transitively.")]
    private Windows.Media.Audio.AudioDeviceInputNode? inputNode;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "AudioGraph.Dispose disposes its child nodes transitively.")]
    private Windows.Media.Audio.AudioFrameOutputNode? frameOutputNode;
    private bool startInProgress;
    private bool disposed;

    public LiveAudioInputMeter()
    {
        InitializeComponent();
        renderTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(RenderIntervalMilliseconds),
        };
        renderTimer.Tick += OnRenderTick;
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
        if (d is LiveAudioInputMeter m)
        {
            _ = m.RestartAsync();
        }
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveAudioInputMeter m)
        {
            _ = m.RestartAsync();
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

            if (!await TryCreateGraphAsync().ConfigureAwait(true))
            {
                return;
            }

            var captureEncoding = CreateStereoFloatEncoding();

            if (!await TryCreateNodesAsync(deviceId, captureEncoding).ConfigureAwait(true))
            {
                return;
            }

            graph!.QuantumStarted += OnQuantumStarted;
            graph.Start();

            renderTimer.Start();
        }
        catch (UnauthorizedAccessException)
        {
            ShowError(Miscellaneous.AudioPermissionDenied);
            await StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LiveAudioInputMeter] start failed: {ex}");
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync().ConfigureAwait(false);
        }
    }

    private async Task<bool> TryCreateGraphAsync()
    {
        var settings = new Windows.Media.Audio.AudioGraphSettings(
            Windows.Media.Render.AudioRenderCategory.Speech)
        {
            // We're not actually rendering — but AudioGraph still requires
            // a render category. Speech minimises latency and is a sensible
            // pairing for a microphone capture graph.
            DesiredSamplesPerQuantum = 480,
            QuantumSizeSelectionMode =
                Windows.Media.Audio.QuantumSizeSelectionMode.ClosestToDesired,
        };

        var graphResult = await Windows.Media.Audio.AudioGraph.CreateAsync(settings);
        if (graphResult.Status is not Windows.Media.Audio.AudioGraphCreationStatus.Success ||
            graphResult.Graph is null)
        {
            Debug.WriteLine($"[LiveAudioInputMeter] AudioGraph.CreateAsync failed: {graphResult.Status}");
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            return false;
        }

        graph = graphResult.Graph;
        return true;
    }

    /// <summary>
    /// Force the frame output node to deliver float stereo at 48 kHz —
    /// independent of whatever multichannel format the graph picks for the
    /// render side. Without this the frame buffers come out in the device's
    /// native multichannel layout and the <c>Span&lt;float&gt;</c> reinterpret
    /// is mis-aligned.
    /// </summary>
    private static Windows.Media.MediaProperties.AudioEncodingProperties CreateStereoFloatEncoding()
    {
        var encoding = Windows.Media.MediaProperties.AudioEncodingProperties.CreatePcm(
            sampleRate: 48000,
            channelCount: 2,
            bitsPerSample: 32);
        encoding.Subtype = Windows.Media.MediaProperties.MediaEncodingSubtypes.Float;
        return encoding;
    }

    private async Task<bool> TryCreateNodesAsync(
        string deviceId,
        Windows.Media.MediaProperties.AudioEncodingProperties captureEncoding)
    {
        var deviceInfo = await DeviceInformation.CreateFromIdAsync(deviceId);

        var inputResult = await graph!.CreateDeviceInputNodeAsync(
            Windows.Media.Capture.MediaCategory.Speech,
            captureEncoding,
            deviceInfo);
        if (inputResult.Status is not Windows.Media.Audio.AudioDeviceNodeCreationStatus.Success ||
            inputResult.DeviceInputNode is null)
        {
            Debug.WriteLine(
                $"[LiveAudioInputMeter] CreateDeviceInputNodeAsync failed: " +
                $"status={inputResult.Status} hr=0x{inputResult.ExtendedError?.HResult:X8}");
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync();
            return false;
        }

        inputNode = inputResult.DeviceInputNode;
        frameOutputNode = graph.CreateFrameOutputNode(captureEncoding);
        inputNode.AddOutgoingConnection(frameOutputNode);
        return true;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Stop must not throw on shutdown.")]
    private async Task StopAsync()
    {
        renderTimer.Stop();

        var existingGraph = graph;
        graph = null;

        if (existingGraph is not null)
        {
            try
            {
                existingGraph.QuantumStarted -= OnQuantumStarted;
                existingGraph.Stop();
                existingGraph.Dispose();
            }
            catch (Exception)
            {
                // Best-effort cleanup.
            }
        }

        inputNode = null;
        frameOutputNode = null;

        await Dispatcher.InvokeAsync(() =>
        {
            ringHead = 0;
            currentPeak = 0;
            Array.Clear(ring, 0, ring.Length);
            PartTopLine.Points.Clear();
            PartBottomLine.Points.Clear();
            PartPeakBar.Height = 0;
        });
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Quantum-tick handler must not crash the audio graph.")]
    private void OnQuantumStarted(
        Windows.Media.Audio.AudioGraph sender,
        object args)
    {
        if (frameOutputNode is null)
        {
            return;
        }

        try
        {
            using var frame = frameOutputNode.GetFrame();
            Span<float> samples = stackalloc float[2048];
            var copied = AudioBufferAccess.ReadFloatSamples(
                frame,
                Windows.Media.AudioBufferAccessMode.Read,
                samples);

            if (copied == 0)
            {
                return;
            }

            var peak = 0f;
            for (var i = 0; i < copied; i++)
            {
                var v = samples[i];
                if (v < 0)
                {
                    v = -v;
                }

                if (v > peak)
                {
                    peak = v;
                }
            }

            ring[ringHead] = peak;
            ringHead = (ringHead + 1) % RingCapacity;
            currentPeak = peak;
        }
        catch (Exception)
        {
            // Best-effort; skip this quantum.
        }
    }

    private void OnRenderTick(
        object? sender,
        EventArgs e)
        => RenderWaveform();

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
            // Read ring oldest-to-newest so the trace scrolls left-to-right.
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