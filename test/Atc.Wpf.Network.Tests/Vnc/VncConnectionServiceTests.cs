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
}