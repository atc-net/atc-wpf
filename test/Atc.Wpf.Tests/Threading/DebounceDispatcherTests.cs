namespace Atc.Wpf.Tests.Threading;

public sealed class DebounceDispatcherTests
{
    [StaFact]
    public void Throttle_fires_at_every_interval_while_calls_keep_coming()
    {
        var sut = new DebounceDispatcher();
        var invocations = new List<object?>();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var call = 0;
        while (stopwatch.ElapsedMilliseconds < 350)
        {
            sut.Throttle(100, invocations.Add, ++call);
            DispatcherPump.For(TimeSpan.FromMilliseconds(10));
        }

        // A debounce would not have fired at all yet.
        Assert.True(invocations.Count >= 2, $"Expected at least 2 invocations while calling, got {invocations.Count}.");

        DispatcherPump.For(TimeSpan.FromMilliseconds(200));

        Assert.True(invocations.Count >= 3, $"Expected at least 3 invocations, got {invocations.Count}.");
        Assert.Equal(call, invocations[^1]);
    }

    [StaFact]
    public void Throttle_fires_the_latest_parameter_once_the_calls_stop()
    {
        var sut = new DebounceDispatcher();
        var invocations = new List<object?>();

        sut.Throttle(50, invocations.Add, 1);
        sut.Throttle(50, invocations.Add, 2);
        sut.Throttle(50, invocations.Add, 3);

        Assert.True(DispatcherPump.Until(() => invocations.Count > 0, TimeSpan.FromSeconds(5)));
        DispatcherPump.For(TimeSpan.FromMilliseconds(150));

        Assert.Equal([3], invocations);
    }

    [StaFact]
    public void Debounce_fires_once_after_the_calls_stop()
    {
        var sut = new DebounceDispatcher();
        var invocations = new List<object?>();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var call = 0;
        while (stopwatch.ElapsedMilliseconds < 250)
        {
            sut.Debounce(100, invocations.Add, ++call);
            DispatcherPump.For(TimeSpan.FromMilliseconds(10));
        }

        Assert.Empty(invocations);
        Assert.True(DispatcherPump.Until(() => invocations.Count > 0, TimeSpan.FromSeconds(5)));
        DispatcherPump.For(TimeSpan.FromMilliseconds(150));

        Assert.Equal([call], invocations);
    }
}