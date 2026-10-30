namespace Atc.Wpf.Components.Tests.Progressing;

/// <summary>
/// <see cref="BusyIndicatorService.RunAsync"/> treats only its own Cancel button as a cancellation; any other
/// <see cref="OperationCanceledException"/> (for example an HTTP timeout) reaches the caller.
/// </summary>
/// <remarks>
/// A shown <see cref="BusyOverlay"/> applies its theme from a pack URI, hence the collection.
/// </remarks>
[Collection(nameof(PackResourceTestGroup))]
public sealed class BusyIndicatorServiceTests
{
    private static readonly TimeSpan PumpTimeout = TimeSpan.FromSeconds(5);

    [StaFact]
    public void RunAsync_CancelledByOwnCancelButton_CompletesAndMarksTokenCancelled()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);
        var operationCancelled = false;

        var task = service.RunAsync(
            async (_, ct) =>
            {
                try
                {
                    await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(true);
                }
                catch (OperationCanceledException)
                {
                    operationCancelled = true;
                    throw;
                }
            },
            regionName: region.Name,
            allowCancellation: true);

        // Act
        region.ClickCancel();
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        Assert.True(operationCancelled);
        Assert.True(task.IsCompletedSuccessfully);

        region.Dispose();
    }

    [StaFact]
    public void RunAsyncOfT_CancelledByOwnCancelButton_ReturnsDefault()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);

        var task = service.RunAsync(
            async (_, ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(true);
                return 42;
            },
            regionName: region.Name,
            allowCancellation: true);

        // Act
        region.ClickCancel();
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Equal(0, task.Result);

        region.Dispose();
    }

    [StaFact]
    public void RunAsync_ForeignOperationCanceledException_PropagatesAndHidesOverlay()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);
        var timeout = new TaskCanceledException("The request timed out.");

        // Act
        var task = service.RunAsync(
            (_, _) => Task.FromException(timeout),
            regionName: region.Name,
            allowCancellation: true);
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCanceled || task.IsFaulted);
        var thrown = Assert.ThrowsAny<OperationCanceledException>(() => task.GetAwaiter().GetResult());
        Assert.Same(timeout, thrown);

        region.Dispose();
    }

    [StaFact]
    public void RunAsyncOfT_ForeignOperationCanceledException_PropagatesAndHidesOverlay()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);
        using var foreignSource = new CancellationTokenSource();
        foreignSource.Cancel();

        // Act
        var task = service.RunAsync<int>(
            (_, _) => Task.FromException<int>(new OperationCanceledException(foreignSource.Token)),
            regionName: region.Name);
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        var thrown = Assert.ThrowsAny<OperationCanceledException>(() => task.GetAwaiter().GetResult());
        Assert.Equal(foreignSource.Token, thrown.CancellationToken);

        region.Dispose();
    }

    [StaFact]
    public void RunAsyncOfT_Completes_ReturnsResultAndHidesOverlay()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);
        var wasBusy = false;

        // Act
        var task = service.RunAsync(
            (_, _) =>
            {
                wasBusy = region.Overlay.IsBusy;
                return Task.FromResult(42);
            },
            regionName: region.Name);
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        Assert.True(wasBusy);
        Assert.Equal(42, task.Result);

        region.Dispose();
    }

    [StaFact]
    public void RunAsync_OperationThrows_PropagatesAndHidesOverlay()
    {
        // Arrange
        var region = new HostedRegion();
        var service = new BusyIndicatorService(Dispatcher.CurrentDispatcher);

        // Act
        var task = service.RunAsync(
            (_, _) => Task.FromException(new InvalidOperationException("Boom")),
            regionName: region.Name);
        var completed = Pump.Until(() => task.IsCompleted && !region.Overlay.IsBusy);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsFaulted);
        Assert.IsType<InvalidOperationException>(task.Exception!.InnerException);

        region.Dispose();
    }

    /// <summary>
    /// A loaded <see cref="BusyOverlay"/> registered under a region name unique to the test
    /// (the manager's region registry is static).
    /// </summary>
    private sealed class HostedRegion : IDisposable
    {
        private readonly HwndSource source;

        public HostedRegion()
        {
            Name = "BusyIndicatorServiceTests-" + Guid.NewGuid().ToString("N");
            Overlay = new BusyOverlay();
            BusyIndicatorManager.SetRegionName(Overlay, Name);

            source = new HwndSource(new HwndSourceParameters("BusyRegionHost") { Width = 1, Height = 1, WindowStyle = 0 })
            {
                RootVisual = Overlay,
            };

            Assert.True(Pump.Until(() => Overlay.IsLoaded));
        }

        public string Name { get; }

        public BusyOverlay Overlay { get; }

        public void ClickCancel()
        {
            var cancelButton = Assert.IsType<Button>(Overlay.BusyContentAfter);
            cancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        public void Dispose()
            => source.Dispose();
    }

    private static class Pump
    {
        public static bool Until(Func<bool> condition)
        {
            var deadline = DateTime.UtcNow + PumpTimeout;
            while (!condition())
            {
                if (DateTime.UtcNow >= deadline)
                {
                    return false;
                }

                var frame = new DispatcherFrame();
                var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(10) };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    frame.Continue = false;
                };
                timer.Start();
                Dispatcher.PushFrame(frame);
            }

            return true;
        }
    }
}