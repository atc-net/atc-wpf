namespace Atc.Wpf.Network.Tests.Diagnostics;

public sealed class TracerouteViewModelTests
{
    private static readonly IPAddress Router = IPAddress.Parse("192.168.1.254");
    private static readonly IPAddress Target = IPAddress.Parse("1.1.1.1");

    [Fact]
    public async Task Trace_ListsTheHopsInOrder()
    {
        var sut = CreateSut("1.1.1.1");

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.Equal([1, 2, 3], sut.Hops.Select(x => x.HopNumber));
        Assert.Equal(Router, sut.Hops[0].IpAddress);
        Assert.Null(sut.Hops[1].IpAddress);
        Assert.True(sut.Hops[2].IsDestination);
        Assert.Equal("21 ms", sut.Hops[0].RoundtripTimeText);
        Assert.Equal("*", sut.Hops[1].RoundtripTimeText);
    }

    [Fact]
    public async Task Trace_ResolvesTheHopNames()
    {
        var sut = CreateSut("1.1.1.1");

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.Equal("router.lan", sut.Hops[0].HostName);
        Assert.Null(sut.Hops[1].HostName);
        Assert.Equal("one.one.one.one", sut.Hops[2].HostName);
    }

    [Fact]
    public async Task Trace_WithoutNameResolution_LeavesTheNamesEmpty()
    {
        var sut = CreateSut("1.1.1.1");
        sut.ResolveHostNames = false;

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.All(sut.Hops, x => Assert.Null(x.HostName));
    }

    [Fact]
    public async Task Trace_HostName_IsResolvedFirst_AndTheOptionsArePassed()
    {
        var sut = CreateSut("one.one.one.one");
        sut.MaxHops = 12;
        sut.TimeoutPerHopInMs = 700;
        (IPAddress Target, int MaxHops, TimeSpan Timeout)? traced = null;
        sut.Tracer = (target, maxHops, timeout, _) =>
        {
            traced = (target, maxHops, timeout);
            return Hops().ToAsyncEnumerable();
        };

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.Equal((Target, 12, TimeSpan.FromMilliseconds(700)), traced);
    }

    [Fact]
    public async Task Trace_UnknownHost_SaysSo()
    {
        var sut = CreateSut("no-such-host.invalid");

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.Empty(sut.Hops);
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Network.Resources.Resources.UnknownHostFormat1, "no-such-host.invalid"),
            sut.ErrorMessage);
    }

    [Fact]
    public async Task Trace_NoRoute_ShowsTheError()
    {
        var sut = CreateSut("1.1.1.1");
        sut.Tracer = (_, _, _, _) => Throw(new PingException("No route"));

        await sut.TraceCommand.ExecuteAsync(null);

        Assert.True(sut.HasError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("  ", false)]
    [InlineData("1.1.1.1", true)]
    public void Trace_NeedsATarget(
        string? target,
        bool expected)
        => Assert.Equal(expected, CreateSut(target).TraceCommand.CanExecute(null));

    private static TracerouteViewModel CreateSut(string? target)
        => new()
        {
            Target = target,
            Tracer = (_, _, _, _) => Hops().ToAsyncEnumerable(),
            AddressResolver = (host, _) => Task.FromResult(
                host switch
                {
                    "one.one.one.one" => Target,
                    _ when IPAddress.TryParse(host, out var address) => address,
                    _ => null,
                }),
            HostNameResolver = (address, _) => Task.FromResult(
                address.Equals(Router) ? "router.lan" : address.Equals(Target) ? "one.one.one.one" : null),
        };

    private static TracerouteHop[] Hops()
        =>
        [
            new(1, Router, TimeSpan.FromMilliseconds(21), IPStatus.TtlExpired, IsDestination: false),
            new(2, null, null, IPStatus.TimedOut, IsDestination: false),
            new(3, Target, TimeSpan.FromMilliseconds(6), IPStatus.Success, IsDestination: true),
        ];

    private static async IAsyncEnumerable<TracerouteHop> Throw(
        Exception exception)
    {
        await Task.CompletedTask;
        throw exception;
#pragma warning disable CS0162 // Unreachable code: the iterator needs a yield.
        yield break;
#pragma warning restore CS0162
    }
}