namespace Atc.Wpf.Tests.Animation;

public sealed class AnimateAttachTests
{
    [StaFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    public void OnVisible_ClearedToNone_ReleasesElement()
    {
        // Arrange & Act
        var weakElement = CreateElementWithOnVisibleThenClear();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert
        Assert.False(weakElement.IsAlive);
    }

    [StaFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    public void OnVisible_SetTwiceThenCleared_ReleasesElement()
    {
        // Arrange & Act
        var weakElement = CreateElementWithOnVisibleSetTwiceThenClear();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert
        Assert.False(weakElement.IsAlive);
    }

    [StaFact]
    public void OnVisible_ClearedToNone_DoesNotAnimateOnVisibilityChange()
    {
        // Arrange
        var element = new Border { Visibility = Visibility.Collapsed };
        AnimateAttach.SetOnVisible(element, AnimationKind.FadeIn);
        AnimateAttach.SetOnVisible(element, AnimationKind.None);

        // Act
        element.Visibility = Visibility.Visible;

        // Assert
        Assert.False(element.HasAnimatedProperties);
        Assert.Equal(1d, element.Opacity);
    }

    [StaFact]
    public void OnVisible_Set_AnimatesWhenBecomingVisible()
    {
        // Arrange
        var element = new Border { Visibility = Visibility.Collapsed };
        AnimateAttach.SetOnVisible(element, AnimationKind.FadeIn);

        // Act
        element.Visibility = Visibility.Visible;

        // Assert
        Assert.True(element.HasAnimatedProperties);
    }

    [StaFact]
    public void OnLoaded_ClearedToNone_DoesNotAnimateOnLoaded()
    {
        // Arrange
        var element = new Border();
        AnimateAttach.SetOnLoaded(element, AnimationKind.FadeIn);
        AnimateAttach.SetOnLoaded(element, AnimationKind.None);

        // Act
        element.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        // Assert
        Assert.False(element.HasAnimatedProperties);
    }

    [StaFact]
    public void OnLoaded_Set_AnimatesOnLoaded()
    {
        // Arrange
        var element = new Border();
        AnimateAttach.SetOnLoaded(element, AnimationKind.FadeIn);

        // Act
        element.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        // Assert
        Assert.True(element.HasAnimatedProperties);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateElementWithOnVisibleThenClear()
    {
        var element = new Border();
        AnimateAttach.SetOnVisible(element, AnimationKind.FadeIn);
        AnimateAttach.SetOnVisible(element, AnimationKind.None);
        return new WeakReference(element);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateElementWithOnVisibleSetTwiceThenClear()
    {
        var element = new Border();
        AnimateAttach.SetOnVisible(element, AnimationKind.FadeIn);
        AnimateAttach.SetOnVisible(element, AnimationKind.SlideInFromLeft);
        AnimateAttach.SetOnVisible(element, AnimationKind.None);
        return new WeakReference(element);
    }
}