namespace Atc.Wpf.Network.Tests.Vnc;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The substitute client is disposed by the view model.")]
public sealed class VncViewerViewModelTests
{
    [Theory]
    [InlineData(VncScaleMode.Fit, 60, 10, 10, 10)]
    [InlineData(VncScaleMode.Original, 30, 40, 30, 40)]
    [InlineData(VncScaleMode.Fill, 100, 50, 50, 50)]
    public void MapToFramebuffer_FollowsTheScaleMode(
        VncScaleMode scaleMode,
        double x,
        double y,
        int expectedX,
        int expectedY)
    {
        // A 100x100 framebuffer shown in a 200x100 image.
        var (vncX, vncY) = VncPointerMapping.MapToFramebuffer(new Point(x, y), new Size(200, 100), 100, 100, scaleMode);

        Assert.Equal((expectedX, expectedY), (vncX, vncY));
    }

    [Fact]
    public void MapToFramebuffer_OutsideTheImage_IsClamped()
        => Assert.Equal((99, 0), VncPointerMapping.MapToFramebuffer(new Point(500, -20), new Size(100, 100), 100, 100, VncScaleMode.Original));

    [Theory]
    [InlineData(VncScaleMode.Fit, Stretch.Uniform)]
    [InlineData(VncScaleMode.Fill, Stretch.UniformToFill)]
    [InlineData(VncScaleMode.Original, Stretch.None)]
    public void Stretch_FollowsTheScaleMode(
        VncScaleMode scaleMode,
        Stretch expected)
    {
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => Substitute.For<IVncClient>()))
        {
            ScaleMode = scaleMode,
        };

        Assert.Equal(expected, sut.Stretch);
    }

    [WpfFact]
    public async Task ConnectionFailed_ShowsTheError_AndRetryReconnectsWithTheSameServer()
    {
        var attempts = new List<(string Host, int Port)>();
        var failing = Substitute.For<IVncClient>();
        failing.Connect(Arg.Any<CancellationToken>()).Returns(false);
        var working = CreateConnectableClient();
        using var sut = new VncViewerViewModel(new VncConnectionService((host, port) =>
        {
            attempts.Add((host, port));
            return attempts.Count == 1 ? failing : working;
        }));

        await sut.ConnectAsync("vnc-host", 5901, "secret");
        Assert.True(sut.HasError);
        Assert.True(sut.RetryCommand.CanExecute(null));

        await sut.RetryCommand.ExecuteAsync(null);

        Assert.Equal([("vnc-host", 5901), ("vnc-host", 5901)], attempts);
        await working.Received(1).Authenticate("secret", Arg.Any<CancellationToken>());
        Assert.True(sut.IsConnected);
        Assert.False(sut.HasError);
    }

    [Fact]
    public void Retry_BeforeAnyConnect_IsDisabled()
    {
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => Substitute.For<IVncClient>()));

        Assert.False(sut.RetryCommand.CanExecute(null));
    }

    [WpfFact]
    public async Task ViewOnly_DoesNotSendInput()
    {
        var client = CreateConnectableClient();
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => client)) { ViewOnly = true };
        await sut.ConnectAsync("vnc-host", 5900, password: null);

        await sut.SendPointerEventAsync(1, 10, 10);
        await sut.SendKeyEventAsync(0xFF0D, pressed: true);

        await client.DidNotReceiveWithAnyArgs().SendPointerEvent(default, default, default, default);
        await client.DidNotReceiveWithAnyArgs().SendKeyEvent(default, default, default);
    }

    [WpfFact]
    public async Task ServerClipboard_IsCopiedOnlyWhenSharingIsOn()
    {
        var client = CreateConnectableClient();
        var copied = new List<string>();
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => client))
        {
            ClipboardSetter = copied.Add,
        };
        await sut.ConnectAsync("vnc-host", 5900, password: null);

        client.ServerCutText += Raise.Event<Action<string>>("not shared");
        sut.ShareClipboard = true;
        client.ServerCutText += Raise.Event<Action<string>>("shared");
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));

        Assert.Equal(["shared"], copied);
    }

    [WpfFact]
    public async Task SendClipboard_SendsTheLocalClipboardText()
    {
        var client = CreateConnectableClient();
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => client))
        {
            ShareClipboard = true,
            ClipboardGetter = () => "local text",
        };
        await sut.ConnectAsync("vnc-host", 5900, password: null);

        Assert.True(sut.SendClipboardCommand.CanExecute(null));
        await sut.SendClipboardCommand.ExecuteAsync(null);

        await client.Received(1).SendClientCutText("local text", Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SendClipboard_WithoutSharing_IsDisabled()
    {
        using var sut = new VncViewerViewModel(new VncConnectionService((_, _) => Substitute.For<IVncClient>()));

        Assert.False(sut.SendClipboardCommand.CanExecute(null));
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