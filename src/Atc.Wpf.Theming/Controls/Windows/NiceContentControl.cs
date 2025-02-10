namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// A ContentControl which use a transition to slide in the content.
/// </summary>
[SuppressMessage("WpfAnalyzers.TemplatePart", "WPF0132:Use PART prefix", Justification = "OK.")]
[TemplatePart(Name = "AfterLoadedStoryboard", Type = typeof(Storyboard))]
[TemplatePart(Name = "AfterLoadedReverseStoryboard", Type = typeof(Storyboard))]
public sealed class NiceContentControl : ContentControl
{
    private Storyboard? afterLoadedStoryboard;
    private Storyboard? afterLoadedReverseStoryboard;
    private bool transitionLoaded;

    public static readonly DependencyProperty ReverseTransitionProperty = DependencyProperty.Register(
        nameof(ReverseTransition),
        typeof(bool),
        typeof(NiceContentControl),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets whether the reverse version of the transition should be used.
    /// </summary>
    public bool ReverseTransition
    {
        get => (bool)GetValue(ReverseTransitionProperty);
        set => SetValue(ReverseTransitionProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty TransitionsEnabledProperty = DependencyProperty.Register(
        nameof(TransitionsEnabled),
        typeof(bool),
        typeof(NiceContentControl),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets the value if a transition should be used or not.
    /// </summary>
    public bool TransitionsEnabled
    {
        get => (bool)GetValue(TransitionsEnabledProperty);
        set => SetValue(TransitionsEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty OnlyLoadTransitionProperty = DependencyProperty.Register(
        nameof(OnlyLoadTransition),
        typeof(bool),
        typeof(NiceContentControl),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets whether the transition should be used only at the loaded event of the control.
    /// </summary>
    public bool OnlyLoadTransition
    {
        get => (bool)GetValue(OnlyLoadTransitionProperty);
        set => SetValue(OnlyLoadTransitionProperty, BooleanBoxes.Box(value));
    }

    public static readonly RoutedEvent TransitionStartedEvent = EventManager.RegisterRoutedEvent(
        nameof(TransitionStarted),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NiceContentControl));

    /// <summary>
    /// The event which will be fired when the transition starts.
    /// </summary>
    public event RoutedEventHandler TransitionStarted
    {
        add => AddHandler(TransitionStartedEvent, value);
        remove => RemoveHandler(TransitionStartedEvent, value);
    }

    public static readonly RoutedEvent TransitionCompletedEvent = EventManager.RegisterRoutedEvent(
        nameof(TransitionCompleted),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NiceContentControl));

    /// <summary>
    /// The event which will be fired when the transition ends.
    /// </summary>
    public event RoutedEventHandler TransitionCompleted
    {
        add => AddHandler(TransitionCompletedEvent, value);
        remove => RemoveHandler(TransitionCompletedEvent, value);
    }

    private static readonly DependencyPropertyKey IsTransitioningPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsTransitioning),
        typeof(bool),
        typeof(NiceContentControl),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty IsTransitioningProperty = IsTransitioningPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets whether if the content is transitioning.
    /// </summary>
    public bool IsTransitioning
    {
        get => (bool)GetValue(IsTransitioningProperty);
        set => SetValue(IsTransitioningPropertyKey, BooleanBoxes.Box(value));
    }

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

        SetValue(IsTransitioningPropertyKey, BooleanBoxes.TrueBox);
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
        SetValue(IsTransitioningPropertyKey, BooleanBoxes.FalseBox);
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