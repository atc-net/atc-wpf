// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed partial class LiveAudioOutputTester : UserControl, IDisposable
{
    private const int RingCapacity = 200;
    private const int RenderIntervalMilliseconds = 33;
    private const int SineFrequencyHz = 1000;
    private const int FadeMilliseconds = 30;
    private const int SegmentMilliseconds = 1000;
    private const int DrainMilliseconds = 500;
    private const int InputSampleRate = 48000;
    private const int InputChannelCount = 2;
    private const int InputBitsPerSample = 32;

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

    private Windows.Media.Audio.AudioGraph? graph;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "AudioGraph.Dispose disposes its child nodes transitively.")]
    private Windows.Media.Audio.AudioDeviceOutputNode? outputNode;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "AudioGraph.Dispose disposes its child nodes transitively.")]
    private Windows.Media.Audio.AudioFrameInputNode? frameInputNode;

    private uint sampleRate;
    private uint channelCount;
    private long samplePosition;
    private long totalSamples;
    private long fadeSamples;
    private long perSegmentSamples;
    private CancellationTokenSource? autoStopCts;
    private int quantumCount;
    private int addFrameCount;
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
            // Stop any ongoing playback when the bound camera changes.
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
        catch (Exception)
        {
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

            if (!await TryBuildGraphAsync(deviceId))
            {
                return;
            }

            graph!.Start();
            renderTimer.Start();

            isPlaying = true;
            UpdateButtonLabel();

            // Wall-clock auto-stop. Stopping based on samplePosition inside the
            // quantum handler would dispose the graph before the engine has finished
            // playing the queued frames — symptom: a single click then silence.
            // Wait the full tone duration plus a drain margin instead.
            var totalMs = (totalSamples * 1000 / (long)sampleRate) + DrainMilliseconds;
            autoStopCts = new CancellationTokenSource();
            _ = ScheduleAutoStopAsync(totalMs, autoStopCts.Token);
        }
        catch (Exception)
        {
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync();
        }
    }

    private async Task<bool> TryBuildGraphAsync(string deviceId)
    {
        var deviceInfo = await DeviceInformation.CreateFromIdAsync(deviceId);

        var settings = new Windows.Media.Audio.AudioGraphSettings(
            Windows.Media.Render.AudioRenderCategory.Media)
        {
            PrimaryRenderDevice = deviceInfo,
        };

        var graphResult = await Windows.Media.Audio.AudioGraph.CreateAsync(settings);
        if (graphResult.Status is not Windows.Media.Audio.AudioGraphCreationStatus.Success ||
            graphResult.Graph is null)
        {
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            return false;
        }

        graph = graphResult.Graph;

        var outputResult = await graph.CreateDeviceOutputNodeAsync();
        if (outputResult.Status is not Windows.Media.Audio.AudioDeviceNodeCreationStatus.Success ||
            outputResult.DeviceOutputNode is null)
        {
            ShowError(Miscellaneous.AudioPreviewUnavailable);
            await StopAsync();
            return false;
        }

        outputNode = outputResult.DeviceOutputNode;

        var inputEncoding = BuildStereoFloatEncoding();

        sampleRate = inputEncoding.SampleRate;
        channelCount = inputEncoding.ChannelCount;

        Debug.WriteLine(
            $"[LiveAudioOutputTester] device encoding: subtype={graph.EncodingProperties.Subtype} " +
            $"sampleRate={graph.EncodingProperties.SampleRate} " +
            $"channels={graph.EncodingProperties.ChannelCount} | " +
            $"input encoding: subtype={inputEncoding.Subtype} sampleRate={inputEncoding.SampleRate} " +
            $"channels={inputEncoding.ChannelCount} bitsPerSample={inputEncoding.BitsPerSample}");

        perSegmentSamples = sampleRate * SegmentMilliseconds / 1000;
        totalSamples = perSegmentSamples * 3;
        fadeSamples = sampleRate * FadeMilliseconds / 1000;
        samplePosition = 0;

        frameInputNode = graph.CreateFrameInputNode(inputEncoding);
        frameInputNode.AddOutgoingConnection(outputNode);
        frameInputNode.QuantumStarted += OnFrameInputQuantumStarted;
        frameInputNode.Start();

        Debug.WriteLine(
            $"[LiveAudioOutputTester] primaryDevice={graph.PrimaryRenderDevice?.Name ?? "<null>"}");

        return true;
    }

    /// <summary>
    /// Use explicit stereo float encoding for the FrameInputNode regardless of the
    /// device's native format. <c>graph.EncodingProperties</c> on multichannel hardware
    /// (e.g. 7.1 Realtek) reports 8 channels, but writing Left=ch0/Right=ch1 with a
    /// 7.1 channel mask routes those to the rear/side speakers — silent on stereo
    /// setups. Stereo input lets the engine handle the up-mix correctly.
    /// </summary>
    private static Windows.Media.MediaProperties.AudioEncodingProperties BuildStereoFloatEncoding()
    {
        var encoding = Windows.Media.MediaProperties.AudioEncodingProperties.CreatePcm(
            sampleRate: InputSampleRate,
            channelCount: InputChannelCount,
            bitsPerSample: InputBitsPerSample);
        encoding.Subtype = Windows.Media.MediaProperties.MediaEncodingSubtypes.Float;
        return encoding;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Auto-stop must not bubble exceptions to the caller.")]
    private async Task ScheduleAutoStopAsync(
        long delayMilliseconds,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(delayMilliseconds), cancellationToken);
            await Dispatcher.InvokeAsync(() => _ = StopAsync());
        }
        catch (Exception)
        {
            // Cancelled (user clicked Stop) or shutdown.
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Stop must not throw on shutdown.")]
    private async Task StopAsync()
    {
        renderTimer.Stop();
        isPlaying = false;

        Debug.WriteLine(
            $"[LiveAudioOutputTester] stopping: quanta={quantumCount} frames={addFrameCount}");
        quantumCount = 0;
        addFrameCount = 0;

        var existingCts = autoStopCts;
        autoStopCts = null;
        if (existingCts is not null)
        {
            try
            {
                await existingCts.CancelAsync();
                existingCts.Dispose();
            }
            catch (Exception)
            {
                // Ignore.
            }
        }

        var existingGraph = graph;
        graph = null;

        if (existingGraph is not null)
        {
            try
            {
                if (frameInputNode is not null)
                {
                    frameInputNode.QuantumStarted -= OnFrameInputQuantumStarted;
                }

                existingGraph.Stop();
                existingGraph.Dispose();
            }
            catch (Exception)
            {
                // Best-effort cleanup.
            }
        }

        outputNode = null;
        frameInputNode = null;

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Quantum-tick handler must not crash the audio graph.")]
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "AddFrame queues the frame asynchronously; the engine takes ownership via the WinRT ABI. The canonical Windows-universal-samples / WPFAudioGraph pattern lets the local go out of scope without disposal.")]
    private void OnFrameInputQuantumStarted(
        Windows.Media.Audio.AudioFrameInputNode sender,
        Windows.Media.Audio.FrameInputNodeQuantumStartedEventArgs args)
    {
        try
        {
            var requiredSamples = args.RequiredSamples;
            quantumCount++;

            if (quantumCount <= 5 || quantumCount % 50 == 0)
            {
                Debug.WriteLine(
                    $"[LiveAudioOutputTester] quantum #{quantumCount} required={requiredSamples} " +
                    $"samplePosition={samplePosition}/{totalSamples} added={addFrameCount}");
            }

            if (requiredSamples <= 0)
            {
                return;
            }

            if (samplePosition >= totalSamples)
            {
                // Tone is fully queued. Don't stop the graph here — the wall-clock
                // auto-stop scheduled in StartInternalAsync waits for the engine to
                // finish playing the buffered frames before disposing.
                return;
            }

            // Allocate the AudioFrame at exactly the byte size we'll write — that
            // allocation size IS the buffer length the engine reads. Setting
            // AudioBuffer.Length after-the-fact is unreliable under CsWinRT.
            var floatCount = requiredSamples * (int)channelCount;

            Span<float> samples = stackalloc float[floatCount];
            FillToneSamples(samples);

            UpdateVisualisationRing(samples);

            var bufferBytes = (uint)(floatCount * sizeof(float));
            var frame = new Windows.Media.AudioFrame(bufferBytes);
            AudioBufferAccess.WriteFloatSamples(frame, samples);
            sender.AddFrame(frame);
            addFrameCount++;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LiveAudioOutputTester] quantum exception: {ex}");
        }
    }

    private void UpdateVisualisationRing(ReadOnlySpan<float> samples)
    {
        var peak = 0f;
        for (var i = 0; i < samples.Length; i++)
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

    private void FillToneSamples(Span<float> destination)
    {
        var channels = (int)channelCount;
        var samplesNeeded = destination.Length / channels;
        var twoPi = 2.0 * System.Math.PI;
        var omega = twoPi * SineFrequencyHz / sampleRate;

        for (var i = 0; i < samplesNeeded; i++)
        {
            var pos = samplePosition + i;
            var amplitude = ComputeFadeAmplitude(pos);
            var segment = pos / perSegmentSamples;  // 0=Left, 1=Right, 2=Both
            var sample = (float)(amplitude * System.Math.Sin(omega * pos));

            for (var ch = 0; ch < channels; ch++)
            {
                var channelSample = segment switch
                {
                    0 => ch == 0 ? sample : 0f,                          // Left only
                    1 => channels > 1 && ch == 1 ? sample : 0f,          // Right only (mono → silence)
                    _ => sample,                                         // Both
                };
                destination[(i * channels) + ch] = channelSample;
            }
        }

        samplePosition += samplesNeeded;
    }

    private double ComputeFadeAmplitude(long pos)
        => SineToneEnvelope.ComputeAmplitude(
            position: pos,
            perSegmentSamples: perSegmentSamples,
            totalSamples: totalSamples,
            fadeSamples: fadeSamples,
            targetAmplitude: 0.3);

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