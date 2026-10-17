namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

/// <summary>
/// Disposing a live preview on its UI thread used to block on a stop that needed that same thread, so it
/// never returned. Each test disposes on an STA thread whose dispatcher is not pumping, and fails on a timeout
/// instead of hanging the run.
/// </summary>
public sealed class LivePreviewDisposeTests
{
    private static readonly TimeSpan DisposeTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public void LiveCameraPreview_DisposeOnItsUiThread_ReturnsAndCompletesTeardown()
        => AssertDisposeReturnsOnUiThread(() => new LiveCameraPreview());

    [Fact]
    public void LiveAudioInputMeter_DisposeOnItsUiThread_ReturnsAndCompletesTeardown()
        => AssertDisposeReturnsOnUiThread(() => new LiveAudioInputMeter());

    [Fact]
    public void LiveAudioOutputTester_DisposeOnItsUiThread_ReturnsAndCompletesTeardown()
        => AssertDisposeReturnsOnUiThread(() => new LiveAudioOutputTester());

    [Fact]
    public async Task LiveCameraPreview_DisposeFromAnotherThread_ReturnsAndCompletesTeardown()
    {
        LiveCameraPreview? preview = null;
        var thread = StartUiThread(() => preview = new LiveCameraPreview(), out var dispatcher);
        try
        {
            await preview!.DisposeAsync().AsTask().WaitAsync(DisposeTimeout, TestContext.Current.CancellationToken);
        }
        finally
        {
            dispatcher.InvokeShutdown();
            thread.Join(DisposeTimeout);
        }
    }

    [SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "The synchronous Dispose is what is under test.")]
    [SuppressMessage("Major Code Smell", "S6966:Awaitable method should be used", Justification = "The synchronous Dispose is what is under test.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Any failure on the UI thread is reported to the test thread.")]
    private static void AssertDisposeReturnsOnUiThread<T>(Func<T> create)
        where T : IDisposable, IAsyncDisposable
    {
        var teardownCompleted = false;
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var control = create();
                control.Dispose();
                teardownCompleted = control.DisposeAsync().AsTask().IsCompleted;
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(DisposeTimeout), "Dispose blocked the UI thread.");
        Assert.Null(failure);
        Assert.True(teardownCompleted);
    }

    private static Thread StartUiThread(
        Action create,
        out Dispatcher dispatcher)
    {
        Dispatcher? created = null;
        using var ready = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            create();
            created = Dispatcher.CurrentDispatcher;
            ready.Set();
            Dispatcher.Run();
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(ready.Wait(DisposeTimeout));
        dispatcher = created!;
        return thread;
    }
}