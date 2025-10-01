// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// A ContentControl which use a transition to slide in the content.
/// </summary>
[SuppressMessage("WpfAnalyzers.TemplatePart", "WPF0132:Use PART prefix", Justification = "OK.")]
[TemplatePart(Name = "AfterLoadedStoryboard", Type = typeof(Storyboard))]
[TemplatePart(Name = "AfterLoadedReverseStoryboard", Type = typeof(Storyboard))]
public sealed partial class NiceContentControl : ContentControl
{
    private Storyboard? afterLoadedStoryboard;
    private Storyboard? afterLoadedReverseStoryboard;
    private bool transitionLoaded;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent transitionStarted;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent transitionCompleted;

    [DependencyProperty]
    private bool reverseTransition;

    [DependencyProperty(DefaultValue = true)]
    private bool transitionsEnabled;

    [DependencyProperty]
    private bool onlyLoadTransition;

    [DependencyProperty]
    private bool isTransitioning;

    public NiceContentControl()
    {
        DefaultStyleKey = typeof(NiceContentControl);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnIsVisibleChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (!TransitionsEnabled || transitionLoaded)
        {
            return;
        }

        if (IsVisible)
        {
            VisualStateManager.GoToState(this, ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", useTransitions: true);
        }
        else
        {
            VisualStateManager.GoToState(this, ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", useTransitions: false);
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (!TransitionsEnabled)
        {
            return;
        }

        UnsetStoryboardEvents();
        if (transitionLoaded)
        {
            VisualStateManager.GoToState(this, ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", useTransitions: false);
        }

        IsVisibleChanged -= OnIsVisibleChanged;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (TransitionsEnabled)
        {
            if (!transitionLoaded)
            {
                SetStoryboardEvents();
                transitionLoaded = OnlyLoadTransition;
                VisualStateManager.GoToState(this, ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", useTransitions: true);
            }

            IsVisibleChanged -= OnIsVisibleChanged;
            IsVisibleChanged += OnIsVisibleChanged;
        }
        else
        {
            if (GetTemplateChild("RootGrid") is Grid rootGrid)
            {
                rootGrid.Opacity = 1.0;
                var transform = (TranslateTransform)rootGrid.RenderTransform;
                if (transform.IsFrozen)
                {
                    var modifiedTransform = transform.Clone();
                    modifiedTransform.X = 0;
                    rootGrid.RenderTransform = modifiedTransform;
                }
                else
                {
                    transform.X = 0;
                }
            }
        }
    }

    /// <summary>
    /// Execute the transition again.
    /// </summary>
    public void Reload()
    {
        if (!TransitionsEnabled || transitionLoaded)
        {
            return;
        }

        if (ReverseTransition)
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", true);
            VisualStateManager.GoToState(this, "AfterUnLoadedReverse", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", true);
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        afterLoadedStoryboard = GetTemplateChild("AfterLoadedStoryboard") as Storyboard;
        afterLoadedReverseStoryboard = GetTemplateChild("AfterLoadedReverseStoryboard") as Storyboard;
    }

    private void AfterLoadedStoryboardCurrentTimeInvalidated(
        object? sender,
        EventArgs e)
    {
        if (sender is not Clock { CurrentState: ClockState.Active })
        {
            return;
        }

        SetValue(IsTransitioningProperty, BooleanBoxes.TrueBox);
        RaiseEvent(new RoutedEventArgs(TransitionStartedEvent));
    }

    private void AfterLoadedStoryboardCompleted(
        object? sender,
        EventArgs e)
    {
        if (transitionLoaded)
        {
            UnsetStoryboardEvents();
        }

        InvalidateVisual();
        SetValue(IsTransitioningProperty, BooleanBoxes.FalseBox);
        RaiseEvent(new RoutedEventArgs(TransitionCompletedEvent));
    }

    private void SetStoryboardEvents()
    {
        if (afterLoadedStoryboard is not null)
        {
            afterLoadedStoryboard.CurrentTimeInvalidated += AfterLoadedStoryboardCurrentTimeInvalidated;
            afterLoadedStoryboard.Completed += AfterLoadedStoryboardCompleted;
        }

        if (afterLoadedReverseStoryboard is not null)
        {
            afterLoadedReverseStoryboard.CurrentTimeInvalidated += AfterLoadedStoryboardCurrentTimeInvalidated;
            afterLoadedReverseStoryboard.Completed += AfterLoadedStoryboardCompleted;
        }
    }

    private void UnsetStoryboardEvents()
    {
        if (afterLoadedStoryboard is not null)
        {
            afterLoadedStoryboard.CurrentTimeInvalidated -= AfterLoadedStoryboardCurrentTimeInvalidated;
            afterLoadedStoryboard.Completed -= AfterLoadedStoryboardCompleted;
        }

        if (afterLoadedReverseStoryboard is not null)
        {
            afterLoadedReverseStoryboard.CurrentTimeInvalidated -= AfterLoadedStoryboardCurrentTimeInvalidated;
            afterLoadedReverseStoryboard.Completed -= AfterLoadedStoryboardCompleted;
        }
    }
}