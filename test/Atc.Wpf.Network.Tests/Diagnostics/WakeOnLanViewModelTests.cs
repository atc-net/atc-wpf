namespace Atc.Wpf.Network.Tests.Diagnostics;

public sealed class WakeOnLanViewModelTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("00-11-22-33-44", false)]
    [InlineData("00-11-22-33-44-GG", false)]
    [InlineData("00-11-22-33-44-55", true)]
    [InlineData("00:11:22:33:44:55", true)]
    [InlineData("001122334455", true)]
    [InlineData("0011.2233.4455", true)]
    public void Send_NeedsAValidMacAddress(
        string? macAddress,
        bool expected)
    {
        var sut = new WakeOnLanViewModel { MacAddress = macAddress };

        Assert.Equal(expected, sut.SendCommand.CanExecute(null));
    }

    [Theory]
    [InlineData("not an address", false)]
    [InlineData("192.168.1.255", true)]
    public void Send_NeedsAValidBroadcastAddress(
        string broadcastAddress,
        bool expected)
    {
        var sut = new WakeOnLanViewModel
        {
            MacAddress = "00-11-22-33-44-55",
            BroadcastAddress = broadcastAddress,
        };

        Assert.Equal(expected, sut.SendCommand.CanExecute(null));
    }

    [Fact]
    public async Task Send_SendsTheMagicPacketToTheBroadcastAddressAndPort()
    {
        var sent = new List<(string Mac, IPEndPoint EndPoint)>();
        var sut = new WakeOnLanViewModel
        {
            MacAddress = "00:11:22:33:44:55",
            BroadcastAddress = "192.168.1.255",
            Port = 7,
            Sender = (mac, endPoint, _) =>
            {
                sent.Add((mac, endPoint));
                return Task.CompletedTask;
            },
        };

        await sut.SendCommand.ExecuteAsync(null);

        var (mac, endPoint) = Assert.Single(sent);
        Assert.Equal("00:11:22:33:44:55", mac);
        Assert.Equal(new IPEndPoint(IPAddress.Parse("192.168.1.255"), 7), endPoint);
        Assert.False(sut.HasError);
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Network.Resources.Resources.MagicPacketSentFormat1, "00:11:22:33:44:55"),
            sut.StatusMessage);
    }

    [Fact]
    public void Defaults_AreTheBroadcastAddressAndPort9()
    {
        var sut = new WakeOnLanViewModel();

        Assert.Equal("255.255.255.255", sut.BroadcastAddress);
        Assert.Equal(9, sut.Port);
    }

    [Fact]
    public async Task Send_WhenSendingFails_ShowsTheError()
    {
        var sut = new WakeOnLanViewModel
        {
            MacAddress = "00-11-22-33-44-55",
            Sender = (_, _, _) => Task.FromException(new System.Net.Sockets.SocketException(10051)),
        };

        await sut.SendCommand.ExecuteAsync(null);

        Assert.True(sut.HasError);
        Assert.Null(sut.StatusMessage);
    }
}