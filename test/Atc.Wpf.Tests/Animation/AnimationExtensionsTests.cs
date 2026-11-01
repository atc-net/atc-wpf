namespace Atc.Wpf.Tests.Animation;

public sealed class AnimationExtensionsTests
{
    private static readonly TimeSpan PumpTimeout = TimeSpan.FromSeconds(5);

    [StaFact]
    public void FadeInAsync_RunsToCompletion()
    {
        // Arrange
        var element = new Border { Opacity = 0 };
        using var host = Host(element);

        // Act
        var task = element.FadeInAsync(Short());
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Equal(1d, element.Opacity);
    }

    [StaFact]
    public void FadeOutAsync_RunsToCompletion_AndCollapses()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);

        // Act
        var task = element.FadeOutAsync(Short());
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Equal(Visibility.Collapsed, element.Visibility);
    }

    [StaFact]
    public void ScaleInAsync_RunsToCompletion()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);

        // Act
        var task = element.ScaleInAsync(Short());
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCompletedSuccessfully);
    }

    [StaFact]
    public void FadeOutAsync_ReplacedByFadeIn_CompletesWithoutCollapsing()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var fadeOut = element.FadeOutAsync(Long());

        // Act
        var fadeIn = element.FadeInAsync(Short());
        var completed = DispatcherPump.Until(() => fadeOut.IsCompleted && fadeIn.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(fadeOut.IsCompletedSuccessfully);
        Assert.Equal(Visibility.Visible, element.Visibility);
        Assert.Equal(1d, element.Opacity);
    }

    [StaFact]
    public void SlideOutToAsync_ReplacedBySlideIn_CompletesWithoutCollapsing()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var slideOut = element.SlideOutToAsync(SlideDirection.Left, Long());

        // Act
        var slideIn = element.SlideInFromAsync(SlideDirection.Left, Short());
        var completed = DispatcherPump.Until(() => slideOut.IsCompleted && slideIn.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(slideOut.IsCompletedSuccessfully);
        Assert.Equal(Visibility.Visible, element.Visibility);
    }

    [StaFact]
    public void ScaleOutAsync_ReplacedByScaleIn_CompletesWithoutCollapsing()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var scaleOut = element.ScaleOutAsync(Long());

        // Act
        var scaleIn = element.ScaleInAsync(Short());
        var completed = DispatcherPump.Until(() => scaleOut.IsCompleted && scaleIn.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(scaleOut.IsCompletedSuccessfully);
        Assert.Equal(Visibility.Visible, element.Visibility);
    }

    [StaFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "The removed clock must survive a collection.")]
    public void FadeInAsync_AnimationRemovedExternally_StillCompletes()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var task = element.FadeInAsync(Short());

        // Act
        element.BeginAnimation(UIElement.OpacityProperty, animation: null);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
    }

    [StaFact]
    public void FadeInAsync_Cancelled_CancelsTaskAndStopsWhereItIs()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var cts = new CancellationTokenSource();
        var task = element.FadeInAsync(Long(), cts.Token);
        DispatcherPump.For(TimeSpan.FromMilliseconds(200));

        // Act
        cts.Cancel();
        var completed = DispatcherPump.Until(() => task.IsCompleted && !element.HasAnimatedProperties, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCanceled);
        Assert.InRange(element.Opacity, 0d, 0.5d);

        cts.Dispose();
    }

    [StaFact]
    public void FadeOutAsync_CancelledFromAnotherThread_CancelsTaskWithoutCollapsing()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var cts = new CancellationTokenSource();
        var task = element.FadeOutAsync(Long(), cts.Token);

        // Act
        Task.Run(cts.Cancel, TestContext.Current.CancellationToken).Wait(TestContext.Current.CancellationToken);
        var completed = DispatcherPump.Until(() => task.IsCompleted && !element.HasAnimatedProperties, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCanceled);
        Assert.Equal(Visibility.Visible, element.Visibility);

        cts.Dispose();
    }

    [StaFact]
    public void ScaleOutAsync_Cancelled_CancelsTaskWithoutCollapsing()
    {
        // Arrange
        var element = new Border();
        using var host = Host(element);
        var cts = new CancellationTokenSource();
        var task = element.ScaleOutAsync(Long(), cts.Token);

        // Act
        cts.Cancel();
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCanceled);
        Assert.Equal(Visibility.Visible, element.Visibility);

        cts.Dispose();
    }

    [StaFact]
    public void AnimateAsync_AlreadyCancelled_ReturnsCanceledTaskWithoutAnimating()
    {
        // Arrange
        var element = new Border { Visibility = Visibility.Collapsed };
        using var host = Host(element);

        // Act
        var task = element.AnimateAsync(AnimationKind.FadeIn, Short(), new CancellationToken(canceled: true));

        // Assert
        Assert.True(task.IsCanceled);
        Assert.Equal(Visibility.Collapsed, element.Visibility);
        Assert.False(element.HasAnimatedProperties);
    }

    [StaFact]
    public void FadeInAsync_CancelledDuringDelay_CancelsTaskWithoutAnimating()
    {
        // Arrange
        var element = new Border { Visibility = Visibility.Collapsed };
        using var host = Host(element);
        var cts = new CancellationTokenSource();
        var task = element.FadeInAsync(new AnimationParameters { Delay = TimeSpan.FromSeconds(30) }, cts.Token);

        // Act
        cts.Cancel();
        var completed = DispatcherPump.Until(() => task.IsCompleted, PumpTimeout);

        // Assert
        Assert.True(completed);
        Assert.True(task.IsCanceled);
        Assert.Equal(Visibility.Collapsed, element.Visibility);

        cts.Dispose();
    }

    /// <summary>
    /// Animation clocks only tick once the thread has a render target, so each element is shown
    /// in a hidden 1x1 <see cref="HwndSource"/>.
    /// </summary>
    private static HwndSource Host(UIElement element)
        => new(new HwndSourceParameters("AnimationTestHost") { Width = 1, Height = 1, WindowStyle = 0 })
        {
            RootVisual = element,
        };

    private static AnimationParameters Short()
        => new() { Duration = TimeSpan.FromMilliseconds(50) };

    private static AnimationParameters Long()
        => new() { Duration = TimeSpan.FromSeconds(30) };
}