namespace Atc.Wpf.Network.Tests.Diagnostics;

public sealed class NtpTimeViewModelTests
{
    [Fact]
    public void Server_DefaultsToThePool()
        => Assert.Equal("pool.ntp.org", new NtpTimeViewModel().Server);

    [Fact]
    public async Task Query_ShowsTheServerTimeAndTheClockOffset()
    {
        var utcTime = new DateTime(2026, 11, 12, 10, 0, 0, DateTimeKind.Utc);
        string? queried = null;
        var sut = new NtpTimeViewModel
        {
            Server = "time.example",
            TimeQuery = (server, _) =>
            {
                queried = server;
                return Task.FromResult(new NtpResult(
                    new IPEndPoint(IPAddress.Parse("10.0.0.123"), 123),
                    stratum: 2,
                    offset: TimeSpan.FromMilliseconds(-1234),
                    roundTripDelay: TimeSpan.FromMilliseconds(18),
                    utcTime: utcTime));
            },
        };

        await sut.QueryCommand.ExecuteAsync(null);

        Assert.Equal("time.example", queried);
        Assert.Equal(utcTime, sut.ServerTimeUtc);
        Assert.Equal(TimeSpan.FromMilliseconds(-1234), sut.Offset);
        Assert.Equal(TimeSpan.FromMilliseconds(18), sut.RoundTripDelay);
        Assert.Equal(2, sut.Stratum);
        Assert.Equal("10.0.0.123:123", sut.ServerAddress);
        Assert.False(sut.HasError);
    }

    [Fact]
    public async Task Query_WhenTheServerDoesNotAnswer_ShowsTheError()
    {
        var sut = new NtpTimeViewModel
        {
            TimeQuery = (_, _) => Task.FromException<NtpResult>(new TimeoutException()),
        };

        await sut.QueryCommand.ExecuteAsync(null);

        Assert.True(sut.HasError);
        Assert.Null(sut.ServerTimeUtc);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(" ", false)]
    [InlineData("pool.ntp.org", true)]
    public void Query_NeedsAServer(
        string? server,
        bool expected)
        => Assert.Equal(expected, new NtpTimeViewModel { Server = server }.QueryCommand.CanExecute(null));
}