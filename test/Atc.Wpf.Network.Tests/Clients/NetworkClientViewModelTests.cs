namespace Atc.Wpf.Network.Tests.Clients;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Substitutes.")]
public sealed class NetworkClientViewModelTests
{
    [Fact]
    public async Task Tcp_Connect_CreatesTheClientWithTheOptions()
    {
        var client = ConnectableTcpClient();
        (string Host, int Port, TcpClientConfig Config)? created = null;
        using var sut = new TcpClientViewModel
        {
            Host = "device.local",
            Port = 4001,
            UseTls = true,
            MessageFraming = TcpMessageFraming.TerminationType,
            FramingTermination = TerminationType.LineFeed,
            ClientFactory = (host, port, config) =>
            {
                created = (host, port, config);
                return client;
            },
        };
        var output = Capture(sut);

        await sut.ConnectCommand.ExecuteAsync(null);

        Assert.True(sut.IsConnected);
        Assert.Equal("device.local", created!.Value.Host);
        Assert.Equal(4001, created.Value.Port);
        Assert.Equal("device.local", created.Value.Config.SslOptions?.TargetHost);
        Assert.Equal(TcpMessageFraming.TerminationType, created.Value.Config.MessageFraming);
        Assert.Equal(TerminationType.LineFeed, created.Value.Config.TerminationType);
        Assert.Contains(output, x => x.Contains("device.local:4001", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Tcp_ConnectFails_ShowsTheError()
    {
        var client = Substitute.For<ITcpClient>();
        client.Connect(Arg.Any<CancellationToken>()).Returns(false);
        using var sut = new TcpClientViewModel { Host = "device.local", Port = 4001, ClientFactory = (_, _, _) => client };

        await sut.ConnectCommand.ExecuteAsync(null);

        Assert.False(sut.IsConnected);
        Assert.True(sut.HasError);
    }

    [Fact]
    public async Task Send_Text_AppendsTheLineEnding()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        sut.LineEnding = TerminationType.CarriageReturnLineFeed;
        sut.SendText = "PING";

        await sut.SendCommand.ExecuteAsync(null);

        await client.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(Encoding.UTF8.GetBytes("PING\r\n"))), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_Hex_SendsTheBytes()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        sut.SendAsHex = true;
        sut.SendText = "48 65 6c-6C 0x6F";

        await sut.SendCommand.ExecuteAsync(null);

        await client.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_InvalidHex_ShowsTheErrorAndSendsNothing()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        sut.SendAsHex = true;
        sut.SendText = "48 6";

        await sut.SendCommand.ExecuteAsync(null);

        Assert.True(sut.HasError);
        await client.DidNotReceive().Send(Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DataReceived_IsShownAsTextOrHex()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        var output = Capture(sut);

        client.DataReceived += Raise.Event<Action<byte[]>>(Encoding.UTF8.GetBytes("hi"));
        sut.ShowReceivedAsHex = true;
        client.DataReceived += Raise.Event<Action<byte[]>>(Encoding.UTF8.GetBytes("hi"));

        Assert.Equal(["← hi", "← 68 69"], output);
    }

    [Fact]
    public async Task DataReceived_Text_WithoutTrailingLineEndings()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        var output = Capture(sut);

        client.DataReceived += Raise.Event<Action<byte[]>>(Encoding.UTF8.GetBytes("hello\r\n"));

        Assert.Equal(["← hello"], output);
    }

    [Fact]
    public async Task ClientError_IsShownAsAnErrorLine()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);
        var output = Capture(sut);

        client.ErrorOccurred += Raise.EventWith(client, new NetworkErrorEventArgs(NetworkErrorKind.Receive, new IOException("reset")));

        Assert.Contains(output, x => x.StartsWith(NetworkClientViewModelBase.ErrorPrefix, StringComparison.Ordinal) && x.Contains("reset", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Disconnect_ClosesTheClient_AndAServerDisconnectIsShown()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);

        await sut.DisconnectCommand.ExecuteAsync(null);

        await client.Received(1).Disconnect();
        Assert.False(sut.IsConnected);
    }

    [Fact]
    public async Task ClientDisconnected_SetsIsConnectedFalse()
    {
        var client = ConnectableTcpClient();
        using var sut = await ConnectedTcp(client);

        client.Disconnected += Raise.Event<Action>();

        Assert.False(sut.IsConnected);
    }

    [Theory]
    [InlineData(null, 4001, false)]
    [InlineData("device", 0, false)]
    [InlineData("device", 70000, false)]
    [InlineData("device", 4001, true)]
    public void Connect_NeedsAHostAndAPort(
        string? host,
        int port,
        bool expected)
    {
        using var sut = new TcpClientViewModel { Host = host, Port = port };

        Assert.Equal(expected, sut.ConnectCommand.CanExecute(null));
    }

    [Fact]
    public async Task Udp_Connect_ResolvesTheHost_AndSendsAndReceives()
    {
        var client = Substitute.For<IUdpClient>();
        client.Connect(Arg.Any<CancellationToken>()).Returns(true);
        (IPAddress Address, int Port)? created = null;
        using var sut = new UdpClientViewModel
        {
            Host = "sensor.local",
            Port = 5005,
            AddressResolver = (_, _) => Task.FromResult<IPAddress?>(IPAddress.Parse("10.0.0.9")),
            ClientFactory = (address, port) =>
            {
                created = (address, port);
                return client;
            },
        };
        var output = Capture(sut);

        await sut.ConnectCommand.ExecuteAsync(null);
        sut.SendText = "hello";
        await sut.SendCommand.ExecuteAsync(null);
        client.DataReceived += Raise.Event<Action<byte[]>>(Encoding.UTF8.GetBytes("world"));

        Assert.Equal((IPAddress.Parse("10.0.0.9"), 5005), created);
        await client.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(Encoding.UTF8.GetBytes("hello"))), Arg.Any<CancellationToken>());
        Assert.Contains("← world", output, StringComparer.Ordinal);
    }

    [Fact]
    public async Task Udp_UnknownHost_ShowsTheError()
    {
        using var sut = new UdpClientViewModel
        {
            Host = "nowhere.invalid",
            Port = 5005,
            AddressResolver = (_, _) => Task.FromResult<IPAddress?>(null),
        };

        await sut.ConnectCommand.ExecuteAsync(null);

        Assert.False(sut.IsConnected);
        Assert.True(sut.HasError);
    }

    private static ITcpClient ConnectableTcpClient()
    {
        var client = Substitute.For<ITcpClient>();
        client.Connect(Arg.Any<CancellationToken>()).Returns(true);
        return client;
    }

    private static async Task<TcpClientViewModel> ConnectedTcp(
        ITcpClient client)
    {
        var sut = new TcpClientViewModel { Host = "device.local", Port = 4001, ClientFactory = (_, _, _) => client };
        await sut.ConnectCommand.ExecuteAsync(null);
        return sut;
    }

    private static List<string> Capture(NetworkClientViewModelBase sut)
    {
        var lines = new List<string>();
        sut.Output = lines.Add;
        return lines;
    }
}