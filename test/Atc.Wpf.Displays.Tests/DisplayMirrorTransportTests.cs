namespace Atc.Wpf.Displays.Tests;

public sealed class DisplayMirrorTransportTests
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task RunAsync_TcpLoopback_AppliesPackets()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new CountingTarget(2);
        receiver.Register(4, target);

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port, TestContext.Current.CancellationToken).AsTask().WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
            using var accepted = await listener.AcceptTcpClientAsync(TestContext.Current.CancellationToken).AsTask().WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);

            var run = receiver.RunAsync(accepted.GetStream(), CancellationToken.None);

            byte[] stream =
            [
                .. DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Data, [1, 2, 3]),
                .. DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Commands, [0xAE]),
            ];
            var clientStream = client.GetStream();
            await clientStream.WriteAsync(stream.AsMemory(0, 7), TestContext.Current.CancellationToken);
            await clientStream.FlushAsync(TestContext.Current.CancellationToken);
            await clientStream.WriteAsync(stream.AsMemory(7), TestContext.Current.CancellationToken);
            await clientStream.FlushAsync(TestContext.Current.CancellationToken);

            await target.Done.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
            Assert.Equal(2, target.Types.Count);

            client.Close();
            await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
            Assert.Equal(2, receiver.GetStatistics(4).Packets);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task RunAsync_StalledPartialPacket_IsDroppedAfterTheTimeout()
    {
        var time = new ManualTimeProvider();
        using var receiver = new DisplayMirrorReceiver(time)
        {
            PartialPacketTimeout = TimeSpan.FromSeconds(1),
        };
        var target = new CountingTarget(1);
        receiver.Register(1, target);
        var rejected = new TaskCompletionSource<DisplayMirrorRejectReason>(TaskCreationOptions.RunContinuationsAsynchronously);
        receiver.PacketRejected += (_, e) => rejected.TrySetResult(e.Reason);

        using var channel = new ChannelStream();
        var run = receiver.RunAsync(channel, CancellationToken.None);

        var packet = DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [5, 6, 7, 8]);
        channel.Send(packet.AsSpan(0, packet.Length / 2).ToArray());
        await WaitUntil(() => time.ActiveTimerCount > 0);

        time.Advance(TimeSpan.FromSeconds(2));

        Assert.Equal(DisplayMirrorRejectReason.Truncated, await rejected.Task.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));
        Assert.Empty(target.Types);

        channel.Send(packet);
        await target.Done.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        Assert.Equal([DisplayMirrorMessageType.Data], target.Types);

        channel.Complete();
        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task RunAsync_Cancellation_ReturnsPromptly()
    {
        using var receiver = new DisplayMirrorReceiver();
        using var cts = new CancellationTokenSource();
        using var channel = new ChannelStream();

        var run = receiver.RunAsync(channel, cts.Token);
        await Task.Delay(20, TestContext.Current.CancellationToken);
        Assert.False(run.IsCompleted);

        await cts.CancelAsync();

        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        Assert.True(run.IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RunAsync_CancellationWithAStreamThatIgnoresTheToken_ReturnsPromptlyAndObservesTheLateRead(
        bool partialHeld)
    {
        var unobserved = new List<Exception>();
        void OnUnobserved(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            lock (unobserved)
            {
                unobserved.Add(e.Exception);
            }
        }

        TaskScheduler.UnobservedTaskException += OnUnobserved;
        try
        {
            using var receiver = new DisplayMirrorReceiver();
            using var cts = new CancellationTokenSource();
            using var channel = new FaultingChannelStream();

            var run = receiver.RunAsync(channel, cts.Token);
            if (partialHeld)
            {
                var packet = DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1, 2, 3, 4]);
                channel.Send(packet.AsSpan(0, packet.Length / 2).ToArray());
                await Task.Delay(50, TestContext.Current.CancellationToken);
            }

            await cts.CancelAsync();
            await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
            Assert.True(run.IsCompletedSuccessfully);

            // The abandoned read faults after RunAsync has returned.
            channel.Fault();
            await Task.Delay(50, TestContext.Current.CancellationToken);
            for (var i = 0; i < 3; i++)
            {
#pragma warning disable S1215 // The unobserved-exception check needs finalizers to run.
                GC.Collect();
#pragma warning restore S1215
                GC.WaitForPendingFinalizers();
            }

            lock (unobserved)
            {
                Assert.Empty(unobserved);
            }
        }
        finally
        {
            TaskScheduler.UnobservedTaskException -= OnUnobserved;
        }
    }

    [Fact]
    public async Task RunAsync_StreamThatEnds_Returns()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new CountingTarget(1);
        receiver.Register(1, target);
        using var stream = new MemoryStream(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Reset, []));

        await receiver.RunAsync(stream, CancellationToken.None).WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);

        Assert.Equal([DisplayMirrorMessageType.Reset], target.Types);
        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task RunUdpAsync_LoopbackDatagrams_AppliesEachPacket()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new CountingTarget(2);
        receiver.Register(2, target);
        using var server = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var endpoint = (IPEndPoint)server.Client.LocalEndPoint!;
        using var cts = new CancellationTokenSource();

        var run = receiver.RunUdpAsync(server, cts.Token);

        using var sender = new UdpClient();
        var first = DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Data, [1]);
        var second = DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Commands, [2, 3]);
        await sender.SendAsync(first, endpoint, TestContext.Current.CancellationToken);
        await sender.SendAsync(second, endpoint, TestContext.Current.CancellationToken);

        await target.Done.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        Assert.Equal([DisplayMirrorMessageType.Data, DisplayMirrorMessageType.Commands], target.Types);

        await cts.CancelAsync();
        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);

        // The caller's client is still usable: the receiver did not dispose it.
        Assert.NotNull(server.Client);
        _ = server.Client.LocalEndPoint;
    }

    [Fact]
    public async Task RunUdpAsync_DatagramWithTwoPackets_IsMalformed()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new CountingTarget(1);
        receiver.Register(2, target);
        var rejected = new TaskCompletionSource<DisplayMirrorRejectReason>(TaskCreationOptions.RunContinuationsAsynchronously);
        receiver.PacketRejected += (_, e) => rejected.TrySetResult(e.Reason);
        using var server = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var endpoint = (IPEndPoint)server.Client.LocalEndPoint!;
        using var cts = new CancellationTokenSource();

        var run = receiver.RunUdpAsync(server, cts.Token);

        using var sender = new UdpClient();
        byte[] two =
        [
            .. DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Data, [1]),
            .. DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Data, [2]),
        ];
        await sender.SendAsync(two, endpoint, TestContext.Current.CancellationToken);

        Assert.Equal(DisplayMirrorRejectReason.Malformed, await rejected.Task.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));
        Assert.Empty(target.Types);

        await cts.CancelAsync();
        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task RunUdpAsync_ClientDisposedByTheCaller_ReturnsNormally()
    {
        using var receiver = new DisplayMirrorReceiver();
        var server = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));

        var run = receiver.RunUdpAsync(server, CancellationToken.None);
        await Task.Delay(50, TestContext.Current.CancellationToken);
        server.Dispose();

        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        Assert.True(run.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task RunUdpAsync_Port_BindsAndAppliesPackets()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new CountingTarget(1);
        receiver.Register(2, target);

        int port;
        using (var probe = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
        {
            port = ((IPEndPoint)probe.Client.LocalEndPoint!).Port;
        }

        using var cts = new CancellationTokenSource();
        var run = receiver.RunUdpAsync(port, cts.Token);

        using var sender = new UdpClient();
        var packet = DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Reset, []);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var deadline = DateTime.UtcNow + WaitTimeout;
        while (!target.Done.IsCompleted && DateTime.UtcNow < deadline)
        {
            await sender.SendAsync(packet, endpoint, TestContext.Current.CancellationToken);
            await Task.Delay(20, TestContext.Current.CancellationToken);
        }

        try
        {
            await target.Done.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        }
        catch (TimeoutException) when (run.IsFaulted)
        {
            await run;
        }

        await cts.CancelAsync();
        await run.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken);
        Assert.Equal([DisplayMirrorMessageType.Reset], target.Types);
    }

    private static async Task WaitUntil(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow + WaitTimeout;
        while (!condition())
        {
            Assert.True(DateTime.UtcNow < deadline, "The condition was not met in time.");
            await Task.Delay(5, TestContext.Current.CancellationToken);
        }
    }

    private sealed class CountingTarget(int expected) : IDisplayMirrorTarget
    {
        private readonly TaskCompletionSource done = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public List<DisplayMirrorMessageType> Types { get; } = [];

        public Task Done => done.Task;

        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            lock (Types)
            {
                Types.Add(packet.Type);
                if (Types.Count >= expected)
                {
                    done.TrySetResult();
                }
            }

            return DisplayMirrorRejectReason.None;
        }
    }

    /// <summary>
    /// A stream that ignores cancellation tokens and fails its pending read when <see cref="Fault"/> is called.
    /// </summary>
    private sealed class FaultingChannelStream : Stream
    {
        private readonly TaskCompletionSource<int> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly ChannelStream inner = new(honourToken: false);

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public void Send(byte[] bytes)
            => inner.Send(bytes);

        public void Fault()
        {
            pending.TrySetException(new IOException("The stream failed after the receiver stopped reading."));
            inner.Complete();
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            var first = await inner.ReadAsync(buffer, CancellationToken.None);
            if (first > 0)
            {
                return first;
            }

            return await pending.Task;
        }

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();

        public override void Flush()
        {
        }

        public override long Seek(
            long offset,
            SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A read-only stream fed by the test: bytes written arrive as the next read, <see cref="Complete"/> is the end of the stream.
    /// </summary>
    private sealed class ChannelStream(bool honourToken = true) : Stream
    {
        private readonly Channel<byte[]> channel = Channel.CreateUnbounded<byte[]>();
        private byte[] current = [];
        private int offset;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public void Send(byte[] bytes)
            => channel.Writer.TryWrite(bytes);

        public void Complete()
            => channel.Writer.TryComplete();

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            if (!honourToken)
            {
                cancellationToken = CancellationToken.None;
            }

            if (offset >= current.Length)
            {
                if (!await channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    return 0;
                }

                current = await channel.Reader.ReadAsync(cancellationToken);
                offset = 0;
            }

            var take = System.Math.Min(buffer.Length, current.Length - offset);
            current.AsMemory(offset, take).CopyTo(buffer);
            offset += take;
            return take;
        }

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();

        public override void Flush()
        {
        }

        public override long Seek(
            long offset,
            SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();
    }
}