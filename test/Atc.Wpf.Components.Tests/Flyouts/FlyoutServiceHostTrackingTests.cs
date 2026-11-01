namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// A <see cref="FlyoutHost"/> tracks the flyouts a <see cref="FlyoutService"/> adds to it after it was created,
/// and the service does not keep closed flyouts alive through a long-lived cancellation token.
/// </summary>
public sealed class FlyoutServiceHostTrackingTests
{
    [StaFact]
    public void ShowAsync_WithFlyoutHost_HostTracksTheOpenFlyout()
    {
        // Arrange
        var host = new FlyoutHost();
        var service = new FlyoutService(host);

        // Act
        var task = service.ShowAsync("Header", new TextBlock());

        // Assert
        Assert.Equal(1, host.OpenFlyoutCount);
        Assert.True(host.IsAnyFlyoutOpen);
        Assert.Same(host.Items[0], host.TopFlyout);

        service.CloseTopFlyout();

        Assert.True(task.IsCompleted);
        Assert.Empty(host.Items);
        Assert.Equal(0, host.OpenFlyoutCount);
        Assert.False(host.IsAnyFlyoutOpen);
        Assert.Null(host.TopFlyout);
    }

    [StaFact]
    public void ShowAsync_WithFlyoutHost_HostEnforcesMaxNestingDepth()
    {
        // Arrange
        var host = new FlyoutHost { MaxNestingDepth = 1 };
        var service = new FlyoutService(host);
        _ = service.ShowAsync("First", new TextBlock());

        // Act
        var second = service.ShowAsync("Second", new TextBlock());

        // Assert
        Assert.Equal(1, host.OpenFlyoutCount);
        Assert.True(second.IsCompleted);

        service.CloseAllFlyouts();
    }

    [StaFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is the point of a leak test.")]
    public void ShowAsync_ManyCyclesWithOneLongLivedToken_ClosedFlyoutsAreCollected()
    {
        // Arrange
        var host = new FlyoutHost();
        var service = new FlyoutService(host);
        using var cts = new CancellationTokenSource();

        // Act
        var flyouts = ShowAndCloseMany(service, host, cts.Token, count: 200);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert
        Assert.Equal(0, flyouts.Count(reference => reference.TryGetTarget(out _)));
        GC.KeepAlive(cts);
    }

    [StaFact]
    public void ShowAsync_TokenCancelledAfterClose_DoesNotThrowOrReopen()
    {
        // Arrange
        var host = new FlyoutHost();
        var service = new FlyoutService(host);
        var cts = new CancellationTokenSource();
        var task = service.ShowAsync("Header", new TextBlock(), cancellationToken: cts.Token);
        service.CloseTopFlyout();

        // Act
        var exception = Record.Exception(cts.Cancel);

        // Assert
        Assert.Null(exception);
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Empty(host.Items);

        cts.Dispose();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static List<WeakReference<Flyout>> ShowAndCloseMany(
        FlyoutService service,
        FlyoutHost host,
        CancellationToken cancellationToken,
        int count)
    {
        var references = new List<WeakReference<Flyout>>();
        for (var i = 0; i < count; i++)
        {
            _ = service.ShowAsync("Header", new TextBlock(), cancellationToken: cancellationToken);
            references.Add(new WeakReference<Flyout>((Flyout)host.Items[0]));
            service.CloseTopFlyout();
        }

        return references;
    }
}