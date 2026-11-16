namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// The display mirror sample: receives display mirroring packets over TCP, UDP or a serial port (or from the in-process loopback demo),
/// creates a card for each display id from its Describe and shows live statistics.
/// </summary>
public sealed partial class DisplayMirrorDemoViewModel : ViewModelBase, IDisposable
{
    private const int StatisticsPerSecond = 4;

    private readonly Dispatcher dispatcher;
    private readonly DisplayMirrorReceiver receiver = new();
    private readonly DispatcherTimer statisticsTimer;
    private readonly DisplayMirrorLoopback loopback;
    private readonly ConcurrentDictionary<byte, DisplayMirrorRejectReason> lastRejections = new();
    private CancellationTokenSource? transportCancellation;
    private Task? transportTask;
    private int lastStreamRejection = -1;
    private DateTimeOffset clearedAt = DateTimeOffset.UtcNow;
    private bool isLoaded;

    [PropertyDisplay("Transport", "Connection", 1)]
    [ObservableProperty]
    private DisplayMirrorTransport transport = DisplayMirrorTransport.Tcp;

    [PropertyDisplay("TCP Port", "Connection", 2)]
    [PropertyRange(1, 65535, 1)]
    [ObservableProperty]
    private int tcpPort = 5555;

    [PropertyDisplay("UDP Port", "Connection", 3)]
    [PropertyRange(1, 65535, 1)]
    [ObservableProperty]
    private int udpPort = 5556;

    [PropertyDisplay("Baud Rate", "Connection", 4)]
    [PropertyRange(1200, 4000000, 1200)]
    [ObservableProperty]
    private int baudRate = 921600;

    [ObservableProperty]
    private SerialPortInfo? selectedSerialPort;

    [ObservableProperty]
    private string status = "Stopped. Start a transport, or press Loopback demo to see it without hardware.";

    [ObservableProperty]
    private string streamStatisticsText = string.Empty;

    [ObservableProperty]
    private int manualDisplayId;

    [ObservableProperty]
    private DisplayMirrorDisplayPreset selectedPreset = DisplayMirrorDisplayPreset.All[0];

    public DisplayMirrorDemoViewModel()
    {
        dispatcher = Dispatcher.CurrentDispatcher;
        loopback = new DisplayMirrorLoopback(
            receiver,
            ex => _ = dispatcher.BeginInvoke(() => Status = $"Loopback demo stopped: {ex.Message}"));

        statisticsTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / StatisticsPerSecond),
        };

        statisticsTimer.Tick += OnStatisticsTimerTick;

        // Both events are raised on the thread that fed the packet: hand them to the UI thread without waiting.
        receiver.DescribeReceived += OnDescribeReceived;
        receiver.PacketRejected += OnPacketRejected;

        StartCommand = new RelayCommand(Start);
        StopCommand = new RelayCommand(StopSource);
        LoopbackCommand = new RelayCommand(StartLoopback);
        AddDisplayCommand = new RelayCommand(AddDisplay);
        ClearDisplaysCommand = new RelayCommand(ClearDisplays);
    }

    /// <summary>
    /// Gets the displays, one card per display id, ordered by id.
    /// </summary>
    public ObservableCollection<DisplayMirrorCard> Cards { get; } = [];

    /// <summary>
    /// Gets the displays that can be added by hand.
    /// </summary>
    public IReadOnlyList<DisplayMirrorDisplayPreset> Presets
        => DisplayMirrorDisplayPreset.All;

    /// <summary>
    /// Gets the receiver.
    /// </summary>
    public DisplayMirrorReceiver Receiver => receiver;

    public ICommand StartCommand { get; }

    public ICommand StopCommand { get; }

    public ICommand LoopbackCommand { get; }

    public ICommand AddDisplayCommand { get; }

    public ICommand ClearDisplaysCommand { get; }

    /// <summary>
    /// Starts the statistics; the view calls it when it is loaded.
    /// </summary>
    public void Resume()
    {
        isLoaded = true;
        statisticsTimer.Start();
    }

    /// <summary>
    /// Stops the transport, the loopback demo and the statistics; the view calls it when it unloads.
    /// </summary>
    public void Stop()
    {
        isLoaded = false;
        statisticsTimer.Stop();
        StopSource();
    }

    /// <summary>
    /// Creates, replaces or keeps the card of a display id for a description; called on the UI thread.
    /// An identical description keeps the card, so a device that describes itself every second does not reset it.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description.</param>
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The card is owned by Cards and disposed when it is replaced or cleared.")]
    public void ApplyDescribe(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        var index = IndexOf(displayId);
        if (index >= 0 && Cards[index].Describe == describe)
        {
            return;
        }

        ReplaceCard(index, DisplayMirrorCardFactory.Create(displayId, describe));
    }

    /// <summary>
    /// Shows the latest statistics; the timer calls it a few times a second.
    /// </summary>
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The card is owned by Cards and disposed when it is replaced or cleared.")]
    public void RefreshStatistics()
    {
        var now = DateTimeOffset.UtcNow;
        var all = receiver.GetAllStatistics();

        // A display id that sends packets without a Describe gets a card that shows its statistics
        // (counted since the cards were last cleared, as the receiver's counters keep running).
        foreach (var (displayId, statistics) in all)
        {
            if (IndexOf(displayId) < 0 &&
                statistics.LastPacket > clearedAt)
            {
                ReplaceCard(
                    -1,
                    new DisplayMirrorPlaceholderCard(
                        displayId,
                        describe: null,
                        "No Describe has arrived for this display id; add it by hand to show it."));
            }
        }

        foreach (var card in Cards)
        {
            card.UpdateStatistics(receiver.GetStatistics(card.DisplayId), now);
            if (lastRejections.TryRemove(card.DisplayId, out var reason))
            {
                card.ShowRejection(reason);
            }
        }

        var stream = receiver.GetStreamStatistics();
        var streamRejection = Interlocked.Exchange(ref lastStreamRejection, -1);
        StreamStatisticsText = string.Create(
            CultureInfo.InvariantCulture,
            $"Stream: {stream.CrcErrors} CRC errors, {stream.Resyncs} resyncs, {stream.Rejected} rejected headers");
        if (streamRejection >= 0)
        {
            StreamStatisticsText += string.Create(CultureInfo.InvariantCulture, $" (last: {(DisplayMirrorRejectReason)streamRejection})");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Stop();
        receiver.DescribeReceived -= OnDescribeReceived;
        receiver.PacketRejected -= OnPacketRejected;
        loopback.Dispose();
        ClearDisplays();
        receiver.Dispose();
    }

    private void Start()
    {
        StopSource();

        Func<CancellationToken, Task> run;
        switch (Transport)
        {
            case DisplayMirrorTransport.Tcp:
                var tcpPortValue = TcpPort;
                run = token => RunTcpAsync(tcpPortValue, token);
                break;
            case DisplayMirrorTransport.Udp:
                var udpPortValue = UdpPort;
                run = token => RunUdpAsync(udpPortValue, token);
                break;
            default:
                if (SelectedSerialPort is not { } port)
                {
                    Status = "Choose a serial port first.";
                    return;
                }

                var portName = port.PortName;
                var baudRateValue = BaudRate;
                run = token => RunSerialAsync(portName, baudRateValue, token);
                break;
        }

        var cancellation = new CancellationTokenSource();
        transportCancellation = cancellation;
        var runToken = cancellation.Token;
        transportTask = Task.Run(() => RunTransportAsync(run, runToken), CancellationToken.None);
    }

    // The loopback and the stream transports feed the same stream parser, so only one source runs at a time.
    private void StartLoopback()
    {
        StopSource();
        receiver.ResetPartial();
        loopback.Start();
        Status = "Loopback demo: an OLED frame stream (#1), a colour frame stream (#2), LCD I2C traffic (#3), an e-paper refresh every 5 s (#4), "
                 + "MAX7219 scrolling text (#5) and ST7920 serial text and graphics (#6) at 20 fps, with some noise.";
    }

    private void StopSource()
    {
        if (!loopback.Stop())
        {
            Status = "The loopback demo did not stop within a second.";
        }

        if (transportCancellation is not null)
        {
            // Cancelling closes the listener, socket or port of the running transport. The transport is awaited (briefly) so its
            // port or socket is released before a new one opens; it never waits for the UI thread, so this cannot deadlock.
            transportCancellation.Cancel();
            transportTask?.Wait(TimeSpan.FromSeconds(2), CancellationToken.None);
            transportTask = null;
            transportCancellation.Dispose();
            transportCancellation = null;
        }

        // A partial packet of the old source must not be completed by the bytes of the next one.
        receiver.ResetPartial();
        Status = "Stopped.";
    }

    // Never faults: every error of a transport ends on the status line.
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A transport error must never end the sample.")]
    private async Task RunTransportAsync(
        Func<CancellationToken, Task> run,
        CancellationToken cancellationToken)
    {
        try
        {
            await run(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            SetStatus($"Transport error: {ex.Message}", cancellationToken);
        }
    }

    private async Task RunTcpAsync(
        int port,
        CancellationToken cancellationToken)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            using var stopListener = cancellationToken.Register(listener.Stop);

            while (!cancellationToken.IsCancellationRequested)
            {
                SetStatus(string.Create(CultureInfo.InvariantCulture, $"Listening on TCP port {port}; waiting for a client."), cancellationToken);

                // One client at a time: the next is accepted when this one disconnects.
                using var client = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                using var closeClient = cancellationToken.Register(client.Dispose);
                receiver.ResetPartial();
                SetStatus(string.Create(CultureInfo.InvariantCulture, $"TCP client {client.Client.RemoteEndPoint} connected on port {port}."), cancellationToken);

                try
                {
                    await receiver.RunAsync(client.GetStream(), cancellationToken).ConfigureAwait(false);
                }
                catch (IOException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    SetStatus($"TCP client lost: {ex.Message}", cancellationToken);
                }
            }
        }
        catch (Exception ex) when (cancellationToken.IsCancellationRequested && ex is OperationCanceledException or ObjectDisposedException or SocketException or IOException)
        {
            // Stopped.
        }
        catch (Exception ex) when (ex is SocketException or IOException or ObjectDisposedException or InvalidOperationException)
        {
            SetStatus($"TCP error: {ex.Message}", cancellationToken);
        }
    }

    private async Task RunUdpAsync(
        int port,
        CancellationToken cancellationToken)
    {
        try
        {
            SetStatus(string.Create(CultureInfo.InvariantCulture, $"Listening on UDP port {port}; one packet per datagram."), cancellationToken);
            await receiver.RunUdpAsync(port, cancellationToken).ConfigureAwait(false);
        }
        catch (SocketException ex)
        {
            SetStatus($"UDP error: {ex.Message}", cancellationToken);
        }
    }

    private async Task RunSerialAsync(
        string portName,
        int baudRateValue,
        CancellationToken cancellationToken)
    {
        try
        {
            using var port = new SerialPort(portName, baudRateValue);
            port.Open();
            SetStatus(string.Create(CultureInfo.InvariantCulture, $"Reading {portName} at {baudRateValue} baud."), cancellationToken);

            // After a cancellation a read may still be pending on the stream, so the port is closed (disposed) rather than read again.
            await receiver.RunAsync(port.BaseStream, cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                SetStatus($"{portName} closed.", cancellationToken);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                SetStatus($"Serial error on {portName}: {ex.Message}", cancellationToken);
            }
        }
    }

    // A status from a run that has been stopped (it arrives after "Stopped.") is dropped.
    private void SetStatus(
        string text,
        CancellationToken runToken)
        => _ = dispatcher.BeginInvoke(() =>
        {
            if (!runToken.IsCancellationRequested)
            {
                Status = text;
            }
        });

    private void AddDisplay()
    {
        var displayId = (byte)System.Math.Clamp(ManualDisplayId, 0, 255);
        ApplyDescribe(displayId, SelectedPreset.ToDescribe());
    }

    private void ClearDisplays()
    {
        foreach (var card in Cards)
        {
            receiver.Unregister(card.DisplayId);
            card.Dispose();
        }

        Cards.Clear();
        lastRejections.Clear();
        clearedAt = DateTimeOffset.UtcNow;
    }

    private int IndexOf(byte displayId)
    {
        for (var i = 0; i < Cards.Count; i++)
        {
            if (Cards[i].DisplayId == displayId)
            {
                return i;
            }
        }

        return -1;
    }

    // Replaces the card at the index, or inserts the card in display id order when the index is -1.
    // The old target is unregistered before its emulators are disposed, so no packet reaches a half-released card.
    private void ReplaceCard(
        int index,
        DisplayMirrorCard card)
    {
        // A rejection of the old card's packets is not shown on the new card.
        lastRejections.TryRemove(card.DisplayId, out _);

        if (index >= 0)
        {
            receiver.Unregister(card.DisplayId);
            Cards[index].Dispose();
            Cards[index] = card;
        }
        else
        {
            var position = 0;
            while (position < Cards.Count && Cards[position].DisplayId < card.DisplayId)
            {
                position++;
            }

            Cards.Insert(position, card);
        }

        if (card.Target is { } target)
        {
            receiver.Register(card.DisplayId, target);
        }
        else
        {
            receiver.Unregister(card.DisplayId);
        }
    }

    private void OnDescribeReceived(
        object? sender,
        DisplayMirrorDescribeEventArgs e)
    {
        var displayId = e.DisplayId;
        var describe = e.Describe;
        _ = dispatcher.BeginInvoke(() => ApplyDescribe(displayId, describe));
    }

    // Kept, not dispatched: at 20 fps a mismatched stream would flood the dispatcher. The statistics timer shows the latest.
    private void OnPacketRejected(
        object? sender,
        DisplayMirrorRejectedEventArgs e)
    {
        if (e.DisplayId is { } displayId)
        {
            lastRejections[displayId] = e.Reason;
        }
        else
        {
            Interlocked.Exchange(ref lastStreamRejection, (int)e.Reason);
        }
    }

    private void OnStatisticsTimerTick(
        object? sender,
        EventArgs e)
    {
        if (isLoaded)
        {
            RefreshStatistics();
        }
    }
}