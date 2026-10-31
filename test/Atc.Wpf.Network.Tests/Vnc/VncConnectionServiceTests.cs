namespace Atc.Wpf.Network.Tests.Vnc;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The substitute client is disposed by VncConnectionService.Dispose.")]
public sealed class VncConnectionServiceTests
{
    [WpfFact]
    public async Task SendPointerEventAsync_WhenTransportFails_DoesNotThrowAndReportsDisconnected()
    {
        var client = CreateConnectableClient();
        client
            .SendPointerEvent(Arg.Any<byte>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new IOException("Connection reset by peer")));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        Assert.True(service.IsConnected);

        var disconnectedCount = 0;
        service.Disconnected += (_, _) => disconnectedCount++;

        var exception = await Record.ExceptionAsync(() => service.SendPointerEventAsync(1, 10, 10));

        Assert.Null(exception);
        Assert.False(service.IsConnected);
        Assert.Equal(1, disconnectedCount);
    }

    [WpfFact]
    public async Task SendKeyEventAsync_WhenTransportFails_DoesNotThrowAndReportsDisconnected()
    {
        var client = CreateConnectableClient();
        client
            .SendKeyEvent(Arg.Any<uint>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new ObjectDisposedException("socket")));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        Assert.True(service.IsConnected);

        var disconnectedCount = 0;
        service.Disconnected += (_, _) => disconnectedCount++;

        var exception = await Record.ExceptionAsync(() => service.SendKeyEventAsync(0xFF0D, pressed: true));

        Assert.Null(exception);
        Assert.False(service.IsConnected);
        Assert.Equal(1, disconnectedCount);
    }

    [WpfFact]
    public async Task SendPointerEventAsync_AfterTransportFailure_DoesNotSendAgain()
    {
        var client = CreateConnectableClient();
        client
            .SendPointerEvent(Arg.Any<byte>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new IOException("Connection reset by peer")));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        await service.SendPointerEventAsync(0, 1, 1);
        await service.SendPointerEventAsync(0, 2, 2);

        await client.Received(1).SendPointerEvent(Arg.Any<byte>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [WpfFact]
    public async Task SendPointerEventAsync_WhenTransportFailsWithAnUnexpectedException_DoesNotThrow()
    {
        var client = CreateConnectableClient();
        client
            .SendPointerEvent(Arg.Any<byte>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new System.ComponentModel.Win32Exception(10054)));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        var exception = await Record.ExceptionAsync(() => service.SendPointerEventAsync(1, 10, 10));

        Assert.Null(exception);
        Assert.False(service.IsConnected);
    }

    [WpfFact]
    public async Task ConnectionLost_AfterASendFailureAlreadyReportedIt_DoesNotRaiseDisconnectedAgain()
    {
        var client = CreateConnectableClient();
        client
            .SendPointerEvent(Arg.Any<byte>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new IOException("Connection reset by peer")));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        var disconnectedCount = 0;
        service.Disconnected += (_, _) => disconnectedCount++;

        await service.SendPointerEventAsync(1, 10, 10);
        client.ConnectionLost += Raise.Event<Action>();

        Assert.Equal(1, disconnectedCount);
    }

    [WpfFact]
    public async Task FramebufferUpdated_WithARectangleOutsideTheSourceFramebuffer_LeavesTheBitmapUnlocked()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        var bitmap = service.CurrentFrame!;

        var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(-2, 0, 4, 4), source));
        await Dispatcher.Yield(DispatcherPriority.Background);

        Assert.True(bitmap.CanFreeze);
    }

    [WpfFact]
    public async Task FramebufferUpdated_WithAValidRectangle_RendersAndRaisesFramebufferUpdated()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        var bitmap = service.CurrentFrame!;

        var updatedCount = 0;
        service.FramebufferUpdated += (_, _) => updatedCount++;

        var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        source.FillRectangle(new VncRectangle(0, 0, 4, 4), unchecked((int)0xFF112233));
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 4, 4), source));
        await Dispatcher.Yield(DispatcherPriority.Background);

        var pixels = new int[16];
        bitmap.CopyPixels(pixels, 16, 0);

        Assert.Equal(1, updatedCount);
        Assert.True(bitmap.CanFreeze);
        Assert.All(pixels, p => Assert.Equal(unchecked((int)0xFF112233), p));
    }

    [WpfFact]
    public async Task FramebufferUpdated_WhenTheSourceIsLargerThanTheBitmap_ClipsToTheBitmap()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        var bitmap = service.CurrentFrame!;

        var source = new VncFramebuffer(64, 64, "test", new VncPixelFormat());
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 64, 64), source));
        await Dispatcher.Yield(DispatcherPriority.Background);

        Assert.True(bitmap.CanFreeze);
    }

    [WpfFact]
    public async Task FramebufferUpdated_WhenTheServiceIsDisposedBeforeTheUpdateIsRendered_DoesNotThrow()
    {
        var client = CreateConnectableClient();
        var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        Exception? unhandled = null;
        void OnUnhandled(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            unhandled = e.Exception;
            e.Handled = true;
        }

        Dispatcher.CurrentDispatcher.UnhandledException += OnUnhandled;
        try
        {
            var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
            client.FramebufferUpdated += Raise.EventWith(
                client,
                new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 4, 4), source));
            service.Dispose();
            await Dispatcher.Yield(DispatcherPriority.Background);
        }
        finally
        {
            Dispatcher.CurrentDispatcher.UnhandledException -= OnUnhandled;
        }

        Assert.Null(unhandled);
    }

    [WpfFact]
    public async Task FramebufferUpdated_BurstOfRectangles_RendersAllAndRaisesFramebufferUpdatedOnce()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        var bitmap = service.CurrentFrame!;

        var updatedCount = 0;
        service.FramebufferUpdated += (_, _) => updatedCount++;

        var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        source.FillRectangle(new VncRectangle(0, 0, 4, 4), unchecked((int)0xFF445566));
        for (var y = 0; y < 4; y += 2)
        {
            for (var x = 0; x < 4; x += 2)
            {
                client.FramebufferUpdated += Raise.EventWith(
                    client,
                    new VncFramebufferUpdateEventArgs(new VncRectangle(x, y, 2, 2), source));
            }
        }

        await Dispatcher.Yield(DispatcherPriority.Background);

        var pixels = new int[16];
        bitmap.CopyPixels(pixels, 16, 0);

        Assert.Equal(1, updatedCount);
        Assert.True(bitmap.CanFreeze);
        Assert.All(pixels, p => Assert.Equal(unchecked((int)0xFF445566), p));
    }

    [WpfFact]
    public async Task FramebufferUpdated_BatchWithARectangleOutsideTheFramebuffer_StillRendersTheOthers()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        var bitmap = service.CurrentFrame!;

        var updatedCount = 0;
        service.FramebufferUpdated += (_, _) => updatedCount++;

        var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        source.FillRectangle(new VncRectangle(0, 0, 4, 4), unchecked((int)0xFF778899));
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(100, 100, 4, 4), source));
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 4, 4), source));
        await Dispatcher.Yield(DispatcherPriority.Background);

        var pixels = new int[16];
        bitmap.CopyPixels(pixels, 16, 0);

        Assert.Equal(1, updatedCount);
        Assert.True(bitmap.CanFreeze);
        Assert.All(pixels, p => Assert.Equal(unchecked((int)0xFF778899), p));
    }

    [WpfFact]
    public async Task FramebufferUpdated_OnlyRectanglesOutsideTheFramebuffer_DoesNotRaiseFramebufferUpdated()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        var updatedCount = 0;
        service.FramebufferUpdated += (_, _) => updatedCount++;

        var source = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        client.FramebufferUpdated += Raise.EventWith(
            client,
            new VncFramebufferUpdateEventArgs(new VncRectangle(100, 100, 4, 4), source));
        await Dispatcher.Yield(DispatcherPriority.Background);

        Assert.Equal(0, updatedCount);
        Assert.True(service.CurrentFrame!.CanFreeze);
    }

    [WpfFact]
    public async Task FramebufferUpdated_QueuedBeforeAReconnect_IsNotRenderedIntoTheNewFrame()
    {
        var firstClient = CreateConnectableClient();
        var secondClient = CreateConnectableClient();
        var clients = new Queue<IVncClient>([firstClient, secondClient]);
        using var service = new VncConnectionService((_, _) => clients.Dequeue());
        await service.ConnectAsync("vnc-host", 5900, password: null);

        var staleSource = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        staleSource.FillRectangle(new VncRectangle(0, 0, 4, 4), unchecked((int)0xFFAA0000));
        firstClient.FramebufferUpdated += Raise.EventWith(
            firstClient,
            new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 4, 4), staleSource));

        await service.ConnectAsync("vnc-host", 5900, password: null);
        var newBitmap = service.CurrentFrame!;
        var updatedCount = 0;
        service.FramebufferUpdated += (_, _) => updatedCount++;

        var freshSource = new VncFramebuffer(4, 4, "test", new VncPixelFormat());
        freshSource.FillRectangle(new VncRectangle(0, 0, 2, 2), unchecked((int)0xFF00BB00));
        secondClient.FramebufferUpdated += Raise.EventWith(
            secondClient,
            new VncFramebufferUpdateEventArgs(new VncRectangle(0, 0, 2, 2), freshSource));
        await Dispatcher.Yield(DispatcherPriority.Background);

        var pixels = new int[16];
        newBitmap.CopyPixels(pixels, 16, 0);

        Assert.Equal(1, updatedCount);
        Assert.Equal(unchecked((int)0xFF00BB00), pixels[0]);
        Assert.Equal(0, pixels[15]);
        Assert.DoesNotContain(unchecked((int)0xFFAA0000), pixels);
    }

    private static IVncClient CreateConnectableClient()
    {
        var client = Substitute.For<IVncClient>();
        client.Connect(Arg.Any<CancellationToken>()).Returns(true);
        client.Authenticate(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        client.Initialize(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        client.StartUpdates(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        client.Framebuffer.Returns(new VncFramebuffer(4, 4, "test", new VncPixelFormat()));
        return client;
    }

    [WpfFact]
    public async Task DisconnectAsync_WhenTheClientThrows_StillCleansUpAndReportsDisconnected()
    {
        var client = CreateConnectableClient();
        client
            .Disconnect()
            .Returns(Task.FromException(new IOException("socket closed")));

        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        Assert.True(service.IsConnected);

        var disconnectedCount = 0;
        service.Disconnected += (_, _) => disconnectedCount++;

        var exception = await Record.ExceptionAsync(service.DisconnectAsync);

        Assert.Null(exception);
        Assert.False(service.IsConnected);
        Assert.Null(service.CurrentFrame);
        Assert.Equal(1, disconnectedCount);
    }

    [WpfFact]
    public async Task ConnectAsync_FailingReconnectAfterALiveSession_ReportsDisconnected()
    {
        var client = CreateConnectableClient();
        using var service = new VncConnectionService((_, _) => client);
        await service.ConnectAsync("vnc-host", 5900, password: null);
        Assert.True(service.IsConnected);

        client.Connect(Arg.Any<CancellationToken>()).Returns(false);
        await service.ConnectAsync("vnc-host", 5900, password: null);

        Assert.False(service.IsConnected);
    }
}