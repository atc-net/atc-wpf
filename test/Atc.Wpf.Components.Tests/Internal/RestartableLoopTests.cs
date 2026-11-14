namespace Atc.Wpf.Components.Tests.Internal;

// TerminalViewer starts its queue loop when loaded and stops it when unloaded, so a viewer that leaves
// the visual tree without being disposed leaves no loop behind.
public sealed class RestartableLoopTests
{
    [Fact]
    public void A_loop_that_was_never_started_is_not_running()
    {
        var sut = new RestartableLoop(_ => Task.CompletedTask);

        Assert.False(sut.IsRunning);
    }

    [Fact]
    public async Task Stop_cancels_the_running_loop()
    {
        var tokens = new List<CancellationToken>();
        var sut = new RestartableLoop(token => RunUntilCancelledAsync(token, tokens));

        sut.Start();
        Assert.True(sut.IsRunning);

        // On a busy thread pool a stop can come before the loop has started at all (then it never runs);
        // this test is about stopping a loop that is running.
        await WaitUntilAsync(() => tokens.Count == 1);
        sut.Stop();

        Assert.False(sut.IsRunning);
        await WaitUntilAsync(() => tokens[0].IsCancellationRequested);
        Assert.True(tokens[0].IsCancellationRequested);
    }

    [Fact]
    public async Task Start_after_stop_runs_again_with_a_new_token()
    {
        var tokens = new List<CancellationToken>();
        var sut = new RestartableLoop(token => RunUntilCancelledAsync(token, tokens));

        sut.Start();
        await WaitUntilAsync(() => tokens.Count == 1);
        sut.Stop();
        sut.Start();
        await WaitUntilAsync(() => tokens.Count == 2);

        Assert.True(sut.IsRunning);
        Assert.True(tokens[0].IsCancellationRequested);
        Assert.False(tokens[1].IsCancellationRequested);
        sut.Stop();
    }

    [Fact]
    public async Task Start_while_running_does_not_start_a_second_loop()
    {
        var tokens = new List<CancellationToken>();
        var sut = new RestartableLoop(token => RunUntilCancelledAsync(token, tokens));

        sut.Start();
        sut.Start();
        await Task.Delay(100, TestContext.Current.CancellationToken);

        Assert.Single(tokens);
        sut.Stop();
    }

    private static async Task RunUntilCancelledAsync(
        CancellationToken token,
        List<CancellationToken> tokens)
    {
        lock (tokens)
        {
            tokens.Add(token);
        }

        try
        {
            await Task.Delay(Timeout.Infinite, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Stopped.
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var i = 0; i < 200 && !condition(); i++)
        {
            await Task.Delay(10, TestContext.Current.CancellationToken);
        }
    }
}