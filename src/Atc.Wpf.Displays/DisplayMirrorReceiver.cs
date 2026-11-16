namespace Atc.Wpf.Displays;

/// <summary>
/// Receives display mirroring packets (protocol version 1) from a byte stream or as single messages
/// and applies them to the <see cref="IDisplayMirrorTarget"/> registered for their display id.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Feed"/> takes a byte stream in chunks of any size (a serial port, a TCP connection) and keeps a partial packet between calls.
/// It scans for the magic, waits for the 10-byte header, checks the version, the type and the payload cap of the type, then waits for the payload and the CRC.
/// A header that fails a check, or a packet with a bad CRC, makes the parser skip one byte and scan again, so a real packet inside the rejected bytes is not lost.
/// A header over its cap is noise and never makes the parser wait for (or allocate) its payload.
/// For a frame header the parser also checks the first payload bytes as soon as they arrive (width and height 1 to 65535, the plane count 1 or 2
/// for an e-paper frame) and that the declared length is exactly what those dimensions need; a mismatch is noise
/// (<see cref="DisplayMirrorRejectReason.Malformed"/>, skip one byte, rescan), so a false frame header does not hold up the good packets behind it.
/// <see cref="ProcessMessage"/> takes exactly one packet per call (a UDP datagram, an MQTT payload).
/// </para>
/// <para>
/// Statistics: <see cref="GetStatistics"/> counts per display id the applied packets and their bytes (Describe included), the unsupported packets and the rejected ones.
/// <see cref="GetStreamStatistics"/> counts what has no trusted display id: bad CRCs, resyncs (the times the stream parser lost sync and discarded bytes; one per
/// loss of sync until the next good packet) and the headers or messages rejected before the CRC was checked.
/// </para>
/// <para>
/// Describe is advisory: it raises <see cref="DescribeReceived"/> and is offered to the target of its display id, if any, without counting a target that
/// ignores it as unsupported or rejected. The receiver never creates or resizes targets.
/// </para>
/// <para>
/// Thread-safety: <see cref="Feed"/> and <see cref="ProcessMessage"/> may be called from any thread; one private lock serialises parsing and applying, so one
/// packet is applied at a time. The events are raised on the calling thread after the lock is released. Exceptions thrown by a target are caught and counted
/// as <see cref="DisplayMirrorRejectReason.TargetFailed"/>. Payload buffers are rented from <see cref="System.Buffers.ArrayPool{T}"/>; a target must not keep them.
/// Targets are applied while the receiver's lock is held, so the <c>Changed</c> events of the buffers and emulators they feed are raised under it:
/// such a handler must not block on another thread (for example with <c>Dispatcher.Invoke</c>) that also calls into the receiver.
/// A target (or a handler it raises) must not call <see cref="Feed"/>, <see cref="ProcessMessage"/> or <see cref="ResetPartial"/> on the receiver
/// that is applying to it: that re-entry throws <see cref="InvalidOperationException"/> (which the receiver then counts as
/// <see cref="DisplayMirrorRejectReason.TargetFailed"/> unless the target catches it).
/// </para>
/// </remarks>
public sealed class DisplayMirrorReceiver : IDisposable
{
    private const int InitialCapacity = 4096;
    private const int ReadBufferLength = 64 * 1024;
    private const int LargeCapacity = 128 * 1024;
    private const int PacketOverhead = DisplayMirrorProtocol.HeaderLength + DisplayMirrorProtocol.CrcLength;
    private const int MaxPayloadLimit = 0x7FFFFFC7 - PacketOverhead; // Array.MaxLength minus the header and CRC.

    private readonly Lock sync = new();
    private readonly TimeProvider timeProvider;
    private readonly Dictionary<byte, IDisplayMirrorTarget> targets = [];
    private readonly Dictionary<byte, Counters> counters = [];
    private readonly Counters streamCounters = new();
    private byte[]? buffer;
    private int count;
    private bool isResyncing;
    private DateTimeOffset lastFeed;
    private int maxFramePayload = DisplayMirrorProtocol.DefaultMaxFramePayload;
    private int maxTrafficPayload = DisplayMirrorProtocol.DefaultMaxTrafficPayload;
    private TimeSpan partialPacketTimeout = TimeSpan.FromSeconds(1);
    private bool isDisposed;
    private bool isApplying;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorReceiver"/> class.
    /// </summary>
    /// <param name="timeProvider">The clock for <see cref="PartialPacketTimeout"/> and <see cref="DisplayMirrorStatistics.LastPacket"/>; <see langword="null"/> uses the system clock.</param>
    public DisplayMirrorReceiver(TimeProvider? timeProvider = null)
        => this.timeProvider = timeProvider ?? TimeProvider.System;

    /// <summary>
    /// Occurs when a valid Describe packet arrives, whether or not a target is registered for its display id.
    /// Raised on the thread that fed the packet, after the receiver's lock is released.
    /// </summary>
    public event EventHandler<DisplayMirrorDescribeEventArgs>? DescribeReceived;

    /// <summary>
    /// Occurs for every rejected packet or header, including unsupported packets.
    /// Raised on the thread that fed the packet, after the receiver's lock is released.
    /// </summary>
    public event EventHandler<DisplayMirrorRejectedEventArgs>? PacketRejected;

    /// <summary>
    /// Gets or sets the payload cap of the frame messages (MonoFrame, ColorFrame, EPaperFrame), in bytes. The default is 4 MiB.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or too large for a packet.</exception>
    public int MaxFramePayload
    {
        get
        {
            lock (sync)
            {
                return maxFramePayload;
            }
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxPayloadLimit);

            lock (sync)
            {
                maxFramePayload = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the payload cap of every other message (Describe, Commands, Data, I2cWrite, Reset, Latch), in bytes. The default is 64 KiB.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or too large for a packet.</exception>
    public int MaxTrafficPayload
    {
        get
        {
            lock (sync)
            {
                return maxTrafficPayload;
            }
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxPayloadLimit);

            lock (sync)
            {
                maxTrafficPayload = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets how long <see cref="Feed"/> keeps a partial packet without new bytes: a partial packet older than this is dropped
    /// (as <see cref="DisplayMirrorRejectReason.Truncated"/>) when the next chunk arrives. The default is 1 second;
    /// <see cref="Timeout.InfiniteTimeSpan"/> never drops.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not positive and not <see cref="Timeout.InfiniteTimeSpan"/>.</exception>
    public TimeSpan PartialPacketTimeout
    {
        get
        {
            lock (sync)
            {
                return partialPacketTimeout;
            }
        }

        set
        {
            if (value <= TimeSpan.Zero &&
                value != Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The timeout must be positive or infinite.");
            }

            lock (sync)
            {
                partialPacketTimeout = value;
            }
        }
    }

    /// <summary>
    /// Registers the target of a display id, replacing any target registered for it.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="target">The target.</param>
    /// <exception cref="ObjectDisposedException">The receiver is disposed.</exception>
    public void Register(
        byte displayId,
        IDisplayMirrorTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        lock (sync)
        {
            ObjectDisposedException.ThrowIf(isDisposed, this);
            targets[displayId] = target;
        }
    }

    /// <summary>
    /// Unregisters the target of a display id.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <returns><see langword="true"/> when a target was registered.</returns>
    public bool Unregister(byte displayId)
    {
        lock (sync)
        {
            return targets.Remove(displayId);
        }
    }

    /// <summary>
    /// Feeds a chunk of a byte stream; complete packets are applied before the call returns and a partial packet is kept for the next call.
    /// Ignored after <see cref="Dispose"/>.
    /// </summary>
    /// <remarks>
    /// When the bytes held for a partial packet are older than <see cref="PartialPacketTimeout"/>, the partial packet is treated as a rejected
    /// candidate (<see cref="DisplayMirrorRejectReason.Truncated"/>, one resync): the parser skips its first byte and scans the held bytes again
    /// before the new chunk is appended. So good packets held behind a false header (noise, or a packet cut short by a sender reboot) are still applied;
    /// only the held bytes that never complete a packet are dropped.
    /// </remarks>
    /// <param name="chunk">The bytes, of any length.</param>
    /// <exception cref="InvalidOperationException">Called from inside a target applied by this receiver (re-entry is not supported).</exception>
    public void Feed(ReadOnlySpan<byte> chunk)
    {
        List<PendingEvent>? events = null;

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ThrowIfApplying();

            var now = timeProvider.GetUtcNow();
            if (count > 0 &&
                partialPacketTimeout != Timeout.InfiniteTimeSpan &&
                now - lastFeed >= partialPacketTimeout)
            {
                RejectStalePartial(ref events);
            }

            lastFeed = now;

            while (!chunk.IsEmpty && !isDisposed)
            {
                var target = EnsureSpace();
                var take = System.Math.Min(chunk.Length, target.Length - count);
                chunk[..take].CopyTo(target.AsSpan(count));
                count += take;
                chunk = chunk[take..];
                ParseBuffer(target, ref events);
            }

            if (isDisposed)
            {
                ReleaseBuffer();
            }
            else
            {
                ShrinkIfIdle();
            }
        }

        Raise(events);
    }

    /// <summary>
    /// Discards every byte held for a partial packet by <see cref="Feed"/>, for example when the stream was reconnected.
    /// Unlike the <see cref="PartialPacketTimeout"/>, which rescans the held bytes for good packets, this is a full discard that counts nothing.
    /// </summary>
    /// <exception cref="InvalidOperationException">Called from inside a target applied by this receiver.</exception>
    public void ResetPartial()
    {
        lock (sync)
        {
            ThrowIfApplying();
            count = 0;
            ShrinkIfIdle();
        }
    }

    /// <summary>
    /// Processes one message that must hold exactly one packet.
    /// </summary>
    /// <param name="packet">The packet bytes.</param>
    /// <returns>
    /// <see cref="DisplayMirrorRejectReason.None"/> when the packet was applied (or was a valid Describe); otherwise why it was rejected:
    /// <see cref="DisplayMirrorRejectReason.Truncated"/> when shorter than its header declares, <see cref="DisplayMirrorRejectReason.Malformed"/>
    /// for extra bytes (two packets, trailing bytes) or a wrong magic. After <see cref="Dispose"/> nothing is processed and
    /// <see cref="DisplayMirrorRejectReason.NoTarget"/> is returned without raising an event.
    /// </returns>
    /// <exception cref="InvalidOperationException">Called from inside a target applied by this receiver (re-entry is not supported).</exception>
    public DisplayMirrorRejectReason ProcessMessage(ReadOnlySpan<byte> packet)
    {
        List<PendingEvent>? events = null;
        DisplayMirrorRejectReason reason;

        lock (sync)
        {
            if (isDisposed)
            {
                return DisplayMirrorRejectReason.NoTarget;
            }

            ThrowIfApplying();

            reason = ProcessMessageCore(packet, ref events);
            if (isDisposed)
            {
                ReleaseBuffer();
            }
        }

        Raise(events);
        return reason;
    }

    /// <summary>
    /// Reads a byte stream (a TCP connection, a serial port stream) until it ends or the token is cancelled and feeds every chunk to <see cref="Feed"/>.
    /// </summary>
    /// <remarks>
    /// A partial packet that gets no new bytes for <see cref="PartialPacketTimeout"/> is dropped (as <see cref="DisplayMirrorRejectReason.Truncated"/>,
    /// the same way <see cref="Feed"/> drops a stale one) even when the stream goes silent, measured with the receiver's <see cref="TimeProvider"/>.
    /// The receiver never closes <paramref name="stream"/>: the caller owns it. The method returns normally when the stream ends
    /// (a read returns 0), when <paramref name="cancellationToken"/> is cancelled, or when the receiver is disposed (noticed after the next read).
    /// Cancellation returns promptly for any stream, also one that ignores the token: the pending read is then abandoned (its result and any
    /// later fault are ignored) and the caller must close the stream to end it. The read buffer is a plain array for that reason.
    /// After a cancellation a read may therefore still be pending on <paramref name="stream"/> and may consume bytes into the abandoned buffer:
    /// do not call <see cref="RunAsync"/> (or read) again on the same stream; close or dispose it and open a new one.
    /// An <see cref="IOException"/> or any other exception of the stream propagates to the caller.
    /// </remarks>
    /// <param name="stream">The readable stream.</param>
    /// <param name="cancellationToken">Cancels the loop; the cancellation is not reported as an exception.</param>
    /// <returns>A task that completes when the loop ends.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public async Task RunAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // A plain array, not a pooled one: when the loop ends while a read is still pending (cancellation, disposal) a stream that ignores
        // the token (SerialPort.BaseStream) may still write into the buffer later, so it must never be returned to a pool.
        var chunk = new byte[ReadBufferLength];
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(
            static state => ((TaskCompletionSource)state!).TrySetResult(),
            cancelled);
        Task<int>? read = null;
        try
        {
            read = BeginRead(stream, chunk, cancellationToken);
            while (true)
            {
                if (!read.IsCompleted &&
                    !await WaitForReadAsync(read, cancelled.Task, cancellationToken).ConfigureAwait(false))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    continue;
                }

                var length = await read.ConfigureAwait(false);
                if (length == 0 ||
                    IsDisposed)
                {
                    return;
                }

                Feed(chunk.AsSpan(0, length));
                read = BeginRead(stream, chunk, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancelling the loop is a normal way to stop it.
        }
        finally
        {
            // A read abandoned by cancellation may still fault later; observe it so it never surfaces as unobserved.
            read?.ContinueWith(
                static task => _ = task.Exception,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }

    /// <summary>
    /// Binds a UDP socket to <see cref="System.Net.IPAddress.Any"/> on <paramref name="port"/> and processes every datagram with <see cref="ProcessMessage"/>
    /// until the token is cancelled. The socket is created and disposed by this method.
    /// </summary>
    /// <param name="port">The UDP port to listen on, 0 to 65535 (0 picks a free port that nobody can read back; use the overload taking a client for that).</param>
    /// <param name="cancellationToken">Cancels the loop; the cancellation is not reported as an exception.</param>
    /// <returns>A task that completes when the loop ends.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not a valid port.</exception>
    public async Task RunUdpAsync(
        int port,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(port);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, System.Net.IPEndPoint.MaxPort);

        using var client = new System.Net.Sockets.UdpClient(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port));
        await RunUdpAsync(client, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes every datagram received by <paramref name="client"/> with <see cref="ProcessMessage"/> (one datagram must hold exactly one packet)
    /// until the token is cancelled. The receiver does not dispose <paramref name="client"/>: the caller owns it.
    /// </summary>
    /// <remarks>
    /// The method returns normally when <paramref name="cancellationToken"/> is cancelled, when the caller disposes the client
    /// (the pending receive then fails with a <see cref="System.Net.Sockets.SocketException"/> or an <see cref="ObjectDisposedException"/>,
    /// which is swallowed) or when the receiver is disposed (noticed after the next datagram). Other socket errors propagate.
    /// </remarks>
    /// <param name="client">The bound UDP client.</param>
    /// <param name="cancellationToken">Cancels the loop; the cancellation is not reported as an exception.</param>
    /// <returns>A task that completes when the loop ends.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
    public async Task RunUdpAsync(
        System.Net.Sockets.UdpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);

        while (!cancellationToken.IsCancellationRequested)
        {
            byte[] datagram;
            try
            {
                var result = await client.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                datagram = result.Buffer;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (System.Net.Sockets.SocketException ex) when (ex.SocketErrorCode is
                System.Net.Sockets.SocketError.OperationAborted or
                System.Net.Sockets.SocketError.Interrupted or
                System.Net.Sockets.SocketError.Shutdown or
                System.Net.Sockets.SocketError.NotSocket)
            {
                return;
            }
            catch (System.Net.Sockets.SocketException ex) when (ex.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset)
            {
                // An ICMP "port unreachable" for an earlier send on this socket; not a lost datagram of ours.
                continue;
            }

            if (IsDisposed)
            {
                return;
            }

            ProcessMessage(datagram);
        }
    }

    /// <summary>
    /// Gets the statistics of a display id.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <returns>A snapshot; all zero for a display id that has seen no packet.</returns>
    public DisplayMirrorStatistics GetStatistics(byte displayId)
    {
        lock (sync)
        {
            return counters.TryGetValue(displayId, out var value)
                ? value.ToStatistics()
                : new Counters().ToStatistics();
        }
    }

    /// <summary>
    /// Gets the statistics of every display id that has seen a packet.
    /// </summary>
    /// <returns>A snapshot by display id.</returns>
    public IReadOnlyDictionary<byte, DisplayMirrorStatistics> GetAllStatistics()
    {
        lock (sync)
        {
            var result = new Dictionary<byte, DisplayMirrorStatistics>(counters.Count);
            foreach (var (displayId, value) in counters)
            {
                result[displayId] = value.ToStatistics();
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the statistics of what has no trusted display id: CRC errors, resyncs and the headers or messages rejected before the CRC was checked.
    /// </summary>
    /// <returns>A snapshot; <see cref="DisplayMirrorStatistics.Packets"/>, <see cref="DisplayMirrorStatistics.Bytes"/> and
    /// <see cref="DisplayMirrorStatistics.Unsupported"/> are zero and <see cref="DisplayMirrorStatistics.LastPacket"/> is <see langword="null"/>.</returns>
    public DisplayMirrorStatistics GetStreamStatistics()
    {
        lock (sync)
        {
            return streamCounters.ToStatistics();
        }
    }

    /// <summary>
    /// Unregisters all targets and releases the stream buffer; later calls to <see cref="Feed"/> and <see cref="ProcessMessage"/> are ignored.
    /// Called from inside a target, the packets still held are not applied and the buffer is released when the current call ends.
    /// </summary>
    public void Dispose()
    {
        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            targets.Clear();
            if (!isApplying)
            {
                ReleaseBuffer();
            }
        }
    }

    [SuppressMessage("Reliability", "S5034:ValueTask should be consumed only once", Justification = "The ValueTask is converted once and never touched again.")]
    private static Task<int> BeginRead(
        Stream stream,
        byte[] chunk,
        CancellationToken cancellationToken)
        => stream.ReadAsync(chunk, cancellationToken).AsTask();

    /// <summary>
    /// Waits for the pending read, the cancellation or the staleness of a held partial packet, whichever comes first.
    /// </summary>
    /// <returns><see langword="true"/> when the read completed; <see langword="false"/> when the token was cancelled or a stale partial packet was dropped.</returns>
    private async Task<bool> WaitForReadAsync(
        Task<int> read,
        Task cancelled,
        CancellationToken cancellationToken)
    {
        if (!TryGetPartialAge(out var remaining))
        {
            // Steady state, nothing held: no timer and no linked token source, only the read against the cancellation.
            var winner = await Task.WhenAny(cancelled, read).ConfigureAwait(false);
            return winner == read && !cancellationToken.IsCancellationRequested;
        }

        using var delayStop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var delay = Task.Delay(remaining, timeProvider, delayStop.Token);
        var first = await Task.WhenAny(cancelled, read, delay).ConfigureAwait(false);
        await delayStop.CancelAsync().ConfigureAwait(false);
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        if (first == read)
        {
            return true;
        }

        DropStalePartial();
        return false;
    }

    private bool IsDisposed
    {
        get
        {
            lock (sync)
            {
                return isDisposed;
            }
        }
    }

    /// <summary>
    /// Gets how long a held partial packet still has before it is stale.
    /// </summary>
    /// <returns><see langword="false"/> when nothing is held, the timeout is infinite or the receiver is disposed.</returns>
    private bool TryGetPartialAge(out TimeSpan remaining)
    {
        lock (sync)
        {
            if (isDisposed ||
                count == 0 ||
                partialPacketTimeout == Timeout.InfiniteTimeSpan)
            {
                remaining = TimeSpan.Zero;
                return false;
            }

            remaining = lastFeed + partialPacketTimeout - timeProvider.GetUtcNow();
            if (remaining < TimeSpan.Zero)
            {
                remaining = TimeSpan.Zero;
            }

            return true;
        }
    }

    /// <summary>
    /// Drops the held partial packet when it has been silent for <see cref="PartialPacketTimeout"/> (the stale path of <see cref="Feed"/>, without new bytes).
    /// </summary>
    private void DropStalePartial()
    {
        List<PendingEvent>? events = null;

        lock (sync)
        {
            if (isDisposed ||
                count == 0 ||
                partialPacketTimeout == Timeout.InfiniteTimeSpan ||
                timeProvider.GetUtcNow() - lastFeed < partialPacketTimeout)
            {
                return;
            }

            ThrowIfApplying();
            RejectStalePartial(ref events);
            ShrinkIfIdle();
        }

        Raise(events);
    }

    private void ThrowIfApplying()
    {
        if (isApplying)
        {
            throw new InvalidOperationException("The receiver cannot be fed from inside a target it is applying a packet to.");
        }
    }

    private void ReleaseBuffer()
    {
        count = 0;
        if (buffer is not null)
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
            buffer = null;
        }
    }

    /// <summary>
    /// Treats the partial packet at the head of the held bytes as a rejected candidate, skips one byte and rescans,
    /// until no stale partial packet is left. Good packets among the held bytes are applied.
    /// </summary>
    private void RejectStalePartial(ref List<PendingEvent>? events)
    {
        while (count > 0 && buffer is not null && !isDisposed)
        {
            MarkResync();
            streamCounters.Rejected++;
            AddEvent(ref events, new PendingEvent(null, DisplayMirrorRejectReason.Truncated, null));

            buffer.AsSpan(1, count - 1).CopyTo(buffer);
            count--;
            ParseBuffer(buffer, ref events);
        }
    }

    private static bool IsMagicAt(
        ReadOnlySpan<byte> data,
        int index)
        => data[index] == DisplayMirrorProtocol.Magic0 &&
           (index + 1 == data.Length || data[index + 1] == DisplayMirrorProtocol.Magic1);

    private static bool IsCrcValid(
        ReadOnlySpan<byte> packet,
        int payloadLength)
    {
        var crc = DisplayMirrorProtocol.ComputeCrc(packet.Slice(2, DisplayMirrorProtocol.HeaderLength - 2 + payloadLength));
        return crc == BinaryPrimitives.ReadUInt16LittleEndian(
            packet.Slice(DisplayMirrorProtocol.HeaderLength + payloadLength, DisplayMirrorProtocol.CrcLength));
    }

    /// <summary>
    /// Checks, before waiting for the body, that a frame header is consistent with the first payload bytes: the width and height are 1 to 65535
    /// (and the plane count 1 or 2 for an e-paper frame) and the declared payload length is exactly the length those dimensions need.
    /// </summary>
    /// <returns><see langword="true"/> when plausible (or not a frame); <see langword="false"/> when not; <see langword="null"/> when more bytes are needed.</returns>
    private static bool? CheckFramePlausibility(
        ReadOnlySpan<byte> candidate,
        int length)
    {
        var type = (DisplayMirrorMessageType)candidate[4];
        if (!DisplayMirrorProtocol.IsFrameType(type))
        {
            return true;
        }

        var prefixLength = type == DisplayMirrorMessageType.EPaperFrame ? 5 : 4;
        if (length < prefixLength)
        {
            return false;
        }

        var header = DisplayMirrorProtocol.HeaderLength;
        if (candidate.Length < header + prefixLength)
        {
            return null;
        }

        long width = BinaryPrimitives.ReadUInt16LittleEndian(candidate[header..]);
        long height = BinaryPrimitives.ReadUInt16LittleEndian(candidate[(header + 2)..]);
        if (width == 0 || height == 0)
        {
            return false;
        }

        long expected;
        switch (type)
        {
            case DisplayMirrorMessageType.MonoFrame:
                expected = 4 + (width * ((height + 7) / 8));
                break;
            case DisplayMirrorMessageType.ColorFrame:
                expected = 4 + (width * height * 2);
                break;
            default:
                var planes = candidate[header + 4];
                if (planes is not (1 or 2))
                {
                    return false;
                }

                expected = 5 + (planes * ((width + 7) / 8) * height) + 1;
                break;
        }

        return expected == length;
    }

    private static void AddEvent(
        ref List<PendingEvent>? events,
        PendingEvent pending)
    {
        events ??= [];
        events.Add(pending);
    }

    private DisplayMirrorRejectReason ProcessMessageCore(
        ReadOnlySpan<byte> packet,
        ref List<PendingEvent>? events)
    {
        DisplayMirrorRejectReason reason;
        var length = 0;

        if (packet.Length < PacketOverhead)
        {
            reason = DisplayMirrorRejectReason.Truncated;
        }
        else if (packet[0] != DisplayMirrorProtocol.Magic0 ||
                 packet[1] != DisplayMirrorProtocol.Magic1)
        {
            reason = DisplayMirrorRejectReason.Malformed;
        }
        else
        {
            reason = ValidateHeader(packet, out length);
            if (reason == DisplayMirrorRejectReason.None)
            {
                var present = packet.Length - PacketOverhead;
                if (length > present)
                {
                    reason = DisplayMirrorRejectReason.Truncated;
                }
                else if (length < present)
                {
                    reason = DisplayMirrorRejectReason.Malformed;
                }
                else if (!IsCrcValid(packet, length))
                {
                    streamCounters.CrcErrors++;
                    AddEvent(ref events, new PendingEvent(null, DisplayMirrorRejectReason.BadCrc, null));
                    return DisplayMirrorRejectReason.BadCrc;
                }
            }
        }

        if (reason != DisplayMirrorRejectReason.None)
        {
            streamCounters.Rejected++;
            AddEvent(ref events, new PendingEvent(null, reason, null));
            return reason;
        }

        var rented = System.Buffers.ArrayPool<byte>.Shared.Rent(length);
        try
        {
            packet.Slice(DisplayMirrorProtocol.HeaderLength, length).CopyTo(rented);
            return Dispatch(
                packet[3],
                (DisplayMirrorMessageType)packet[4],
                packet[5],
                new ReadOnlyMemory<byte>(rented, 0, length),
                packet.Length,
                ref events);
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Checks the version, the type and the payload cap of a header whose magic is already checked.
    /// </summary>
    private DisplayMirrorRejectReason ValidateHeader(
        ReadOnlySpan<byte> header,
        out int length)
    {
        length = 0;

        if (header[2] != DisplayMirrorProtocol.Version)
        {
            return DisplayMirrorRejectReason.UnsupportedVersion;
        }

        var type = (DisplayMirrorMessageType)header[4];
        if (!DisplayMirrorProtocol.IsKnownType(type))
        {
            return DisplayMirrorRejectReason.UnknownType;
        }

        var declared = BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(6, 4));
        var cap = DisplayMirrorProtocol.IsFrameType(type)
            ? maxFramePayload
            : maxTrafficPayload;
        if (declared > (uint)cap)
        {
            return DisplayMirrorRejectReason.TooLarge;
        }

        length = (int)declared;
        return DisplayMirrorRejectReason.None;
    }

    /// <summary>
    /// Returns the stream buffer with room for at least one more byte; grows it to the length of the partial packet it holds when full.
    /// </summary>
    private byte[] EnsureSpace()
    {
        buffer ??= System.Buffers.ArrayPool<byte>.Shared.Rent(InitialCapacity);
        if (count < buffer.Length)
        {
            return buffer;
        }

        // A full buffer holds a partial packet whose header passed validation at index 0 (the parser compacts and keeps only that).
        var required = buffer.Length + 1;
        if (count >= DisplayMirrorProtocol.HeaderLength &&
            ValidateHeader(buffer, out var length) == DisplayMirrorRejectReason.None)
        {
            required = System.Math.Max(required, PacketOverhead + length);
        }

        Replace(System.Buffers.ArrayPool<byte>.Shared.Rent(required));
        return buffer;
    }

    private void ShrinkIfIdle()
    {
        if (buffer is not null &&
            buffer.Length > LargeCapacity &&
            count <= InitialCapacity)
        {
            Replace(System.Buffers.ArrayPool<byte>.Shared.Rent(InitialCapacity));
        }
    }

    [MemberNotNull(nameof(buffer))]
    private void Replace(byte[] replacement)
    {
        if (buffer is not null)
        {
            buffer.AsSpan(0, count).CopyTo(replacement);
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
        }

        buffer = replacement;
    }

    private void ParseBuffer(
        byte[] data,
        ref List<PendingEvent>? events)
    {
        var span = data.AsSpan(0, count);
        var position = 0;

        while (position < count && !isDisposed)
        {
            var magic = FindMagic(span, position);
            if (magic < 0)
            {
                MarkResync();
                position = count;
                break;
            }

            if (magic > position)
            {
                MarkResync();
                position = magic;
            }

            var consumed = ParseCandidate(data, position, ref events);
            if (consumed == 0)
            {
                break;
            }

            position += consumed;
        }

        if (position > 0)
        {
            span[position..].CopyTo(span);
            count -= position;
        }
    }

    /// <summary>
    /// Parses the candidate packet at a magic: returns the bytes consumed (one for a rejected candidate, the packet length for a good one)
    /// or zero when more bytes are needed.
    /// </summary>
    private int ParseCandidate(
        byte[] data,
        int position,
        ref List<PendingEvent>? events)
    {
        var candidate = data.AsSpan(position, count - position);
        if (candidate.Length < DisplayMirrorProtocol.HeaderLength)
        {
            return 0;
        }

        var reason = ValidateHeader(candidate, out var length);
        if (reason != DisplayMirrorRejectReason.None)
        {
            MarkResync();
            streamCounters.Rejected++;
            AddEvent(ref events, new PendingEvent(null, reason, null));
            return 1;
        }

        var plausibility = CheckFramePlausibility(candidate, length);
        if (plausibility is null)
        {
            return 0;
        }

        if (plausibility == false)
        {
            MarkResync();
            streamCounters.Rejected++;
            AddEvent(ref events, new PendingEvent(null, DisplayMirrorRejectReason.Malformed, null));
            return 1;
        }

        var total = PacketOverhead + length;
        if (candidate.Length < total)
        {
            return 0;
        }

        if (!IsCrcValid(candidate, length))
        {
            MarkResync();
            streamCounters.CrcErrors++;
            AddEvent(ref events, new PendingEvent(null, DisplayMirrorRejectReason.BadCrc, null));
            return 1;
        }

        isResyncing = false;
        Dispatch(
            candidate[3],
            (DisplayMirrorMessageType)candidate[4],
            candidate[5],
            new ReadOnlyMemory<byte>(data, position + DisplayMirrorProtocol.HeaderLength, length),
            total,
            ref events);
        return total;
    }

    private static int FindMagic(
        ReadOnlySpan<byte> data,
        int start)
    {
        var index = start;
        while (index < data.Length)
        {
            var found = data[index..].IndexOf(DisplayMirrorProtocol.Magic0);
            if (found < 0)
            {
                return -1;
            }

            index += found;
            if (IsMagicAt(data, index))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    private void MarkResync()
    {
        if (!isResyncing)
        {
            isResyncing = true;
            streamCounters.Resyncs++;
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing target is counted as TargetFailed and the receiver keeps running.")]
    private DisplayMirrorRejectReason SafeApply(
        IDisplayMirrorTarget target,
        DisplayMirrorPacket packet)
    {
        isApplying = true;
        try
        {
            return target.Apply(packet);
        }
        catch (Exception)
        {
            return DisplayMirrorRejectReason.TargetFailed;
        }
        finally
        {
            isApplying = false;
        }
    }

    private DisplayMirrorRejectReason Dispatch(
        byte displayId,
        DisplayMirrorMessageType type,
        byte flags,
        ReadOnlyMemory<byte> payload,
        int packetLength,
        ref List<PendingEvent>? events)
    {
        if (!counters.TryGetValue(displayId, out var statistics))
        {
            statistics = new Counters();
            counters[displayId] = statistics;
        }

        statistics.LastPacket = timeProvider.GetUtcNow();
        targets.TryGetValue(displayId, out var target);
        var packet = new DisplayMirrorPacket(displayId, type, flags, payload);

        if (type == DisplayMirrorMessageType.Describe)
        {
            return DispatchDescribe(statistics, target, packet, packetLength, ref events);
        }

        if (target is null)
        {
            return Reject(statistics, displayId, DisplayMirrorRejectReason.NoTarget, ref events);
        }

        var result = SafeApply(target, packet);
        switch (result)
        {
            case DisplayMirrorRejectReason.None:
                statistics.Packets++;
                statistics.Bytes += packetLength;
                return result;
            case DisplayMirrorRejectReason.Unsupported:
                statistics.Unsupported++;
                AddEvent(ref events, new PendingEvent(displayId, result, null));
                return result;
            default:
                return Reject(statistics, displayId, result, ref events);
        }
    }

    /// <summary>
    /// Raises <see cref="DescribeReceived"/> for a valid Describe and offers it to the target; a target that ignores it is not counted (ruling R1).
    /// </summary>
    private DisplayMirrorRejectReason DispatchDescribe(
        Counters statistics,
        IDisplayMirrorTarget? target,
        DisplayMirrorPacket packet,
        int packetLength,
        ref List<PendingEvent>? events)
    {
        if (!DisplayMirrorDescribe.TryParse(packet.Payload.Span, out var describe))
        {
            return Reject(statistics, packet.DisplayId, DisplayMirrorRejectReason.Malformed, ref events);
        }

        statistics.Packets++;
        statistics.Bytes += packetLength;
        AddEvent(ref events, new PendingEvent(packet.DisplayId, DisplayMirrorRejectReason.None, describe));

        if (target is null)
        {
            return DisplayMirrorRejectReason.None;
        }

        var result = SafeApply(target, packet);
        return result is DisplayMirrorRejectReason.None or DisplayMirrorRejectReason.Unsupported
            ? DisplayMirrorRejectReason.None
            : Reject(statistics, packet.DisplayId, result, ref events);
    }

    private static DisplayMirrorRejectReason Reject(
        Counters statistics,
        byte displayId,
        DisplayMirrorRejectReason reason,
        ref List<PendingEvent>? events)
    {
        statistics.Rejected++;
        AddEvent(ref events, new PendingEvent(displayId, reason, null));
        return reason;
    }

    private void Raise(List<PendingEvent>? events)
    {
        if (events is null)
        {
            return;
        }

        foreach (var pending in events)
        {
            if (pending.Describe is not null)
            {
                DescribeReceived?.Invoke(this, new DisplayMirrorDescribeEventArgs(pending.DisplayId!.Value, pending.Describe));
            }
            else
            {
                PacketRejected?.Invoke(this, new DisplayMirrorRejectedEventArgs(pending.DisplayId, pending.Reason));
            }
        }
    }

    private readonly record struct PendingEvent(
        byte? DisplayId,
        DisplayMirrorRejectReason Reason,
        DisplayMirrorDescribe? Describe);

    private sealed class Counters
    {
        public long Packets { get; set; }

        public long Bytes { get; set; }

        public long CrcErrors { get; set; }

        public long Resyncs { get; set; }

        public long Unsupported { get; set; }

        public long Rejected { get; set; }

        public DateTimeOffset? LastPacket { get; set; }

        public DisplayMirrorStatistics ToStatistics()
            => new(Packets, Bytes, CrcErrors, Resyncs, Unsupported, Rejected, LastPacket);
    }
}