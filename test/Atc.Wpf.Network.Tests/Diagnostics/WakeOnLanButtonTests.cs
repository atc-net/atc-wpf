namespace Atc.Wpf.Network.Tests.Diagnostics;

public sealed class WakeOnLanButtonTests
{
    [StaFact]
    public async Task Click_SendsTheMagicPacketToTheMacAddress()
    {
        var sent = new List<(string Mac, IPEndPoint EndPoint)>();
        var sut = new WakeOnLanButton
        {
            MacAddress = "00-11-22-33-44-55",
            Sender = (mac, endPoint, _) =>
            {
                sent.Add((mac, endPoint));
                return Task.CompletedTask;
            },
        };
        WakeOnLanSentEventArgs? args = null;
        sut.PacketSent += (_, e) => args = e;

        await sut.SendAsync();

        var (mac, endPoint) = Assert.Single(sent);
        Assert.Equal("00-11-22-33-44-55", mac);
        Assert.Equal(new IPEndPoint(IPAddress.Broadcast, 9), endPoint);
        Assert.Equal("00-11-22-33-44-55", args!.MacAddress);
        Assert.Null(args.Error);
    }

    [StaFact]
    public async Task Click_WhenSendingFails_ReportsTheError()
    {
        var sut = new WakeOnLanButton
        {
            MacAddress = "00-11-22-33-44-55",
            Sender = (_, _, _) => Task.FromException(new System.Net.Sockets.SocketException(10051)),
        };
        WakeOnLanSentEventArgs? args = null;
        sut.PacketSent += (_, e) => args = e;

        await sut.SendAsync();

        Assert.NotNull(args!.Error);
    }

    [StaTheory]
    [InlineData(null, false)]
    [InlineData("nope", false)]
    [InlineData("00:11:22:33:44:55", true)]
    public void IsEnabled_FollowsTheMacAddress(
        string? macAddress,
        bool expected)
    {
        var sut = new WakeOnLanButton { MacAddress = macAddress };

        Assert.Equal(expected, sut.IsEnabled);
    }

    [StaFact]
    public void Invoke_FromAutomation_Sends()
    {
        var sent = 0;
        var sut = new WakeOnLanButton
        {
            MacAddress = "00-11-22-33-44-55",
            Sender = (_, _, _) =>
            {
                sent++;
                return Task.CompletedTask;
            },
        };

        ((System.Windows.Automation.Provider.IInvokeProvider)new System.Windows.Automation.Peers.ButtonAutomationPeer(sut)
            .GetPattern(System.Windows.Automation.Peers.PatternInterface.Invoke)).Invoke();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));

        Assert.Equal(1, sent);
    }
}