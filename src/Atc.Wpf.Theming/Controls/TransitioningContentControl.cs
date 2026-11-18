namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A ContentControl that animates content as it loads and unloads.
/// </summary>
[SuppressMessage(
    "WpfAnalyzers.TemplatePart",
    "WPF0132:Use PART prefix",
    Justification = "OK.")]
public partial class TransitioningContentControl : ContentControl
{
    internal const string PresentationGroup = "PresentationStates";
    internal const string HiddenState = "Hidden";
    internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";
    internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

    [TemplatePartField(PreviousContentPresentationSitePartName)]
    private ContentPresenter? previousContentPresentationSite;

    [TemplatePartField(CurrentContentPresentationSitePartName)]
    private ContentPresenter? currentContentPresentationSite;

    private bool allowIsTransitioningPropertyWrite;
    private Storyboard? currentTransition;

    [DependencyProperty(
        DefaultValue = false,
        GeneratePartialHooks = true,
        GenerateClrAccessors = false)]
    private bool isTransitioning;

    /// <summary>
    /// Gets or sets the transition type.
    /// </summary>
    [DependencyProperty(
        DefaultValue = TransitionType.Default,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
        GeneratePartialHooks = true)]
    private TransitionType transition;

    /// <summary>
    /// Gets or sets whether if the transition should restart after the content change.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnRestartTransitionOnContentChangePropertyChanged))]
    private bool restartTransitionOnContentChange;

    /// <summary>
    /// Gets or sets customized visual states to use as transition.
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<VisualState>? customVisualStates;

    /// <summary>
    /// Gets or sets the name of a custom transition visual state.
    /// </summary>
    [DependencyProperty(DefaultValue = "CustomTransition")]
    private string customVisualStatesName;

    /// <summary>
    /// Occurs when a content transition has completed.
    /// </summary>
    public event RoutedEventHandler? TransitionCompleted;

    /// <summary>
    /// The default transition type.
    /// </summary>
    public const TransitionType DefaultTransitionState = TransitionType.Default;

    /// <summary>
    /// Gets whether if the content is transitioning.
    /// </summary>
    public bool IsTransitioning
    {
        get => (bool)GetValue(IsTransitioningProperty);
        private set
        {
            allowIsTransitioningPropertyWrite = true;
            try
            {
                SetValue(
                    IsTransitioningProperty,
                    BooleanBoxes.Box(value));
            }
            finally
            {
                allowIsTransitioningPropertyWrite = false;
            }
        }
    }

    internal Storyboard? CurrentTransition
    {
        get => currentTransition;
        set
        {
            // decouple event
            if (currentTransition is not null)
            {
                currentTransition.Completed -= OnTransitionCompleted;
            }

            currentTransition = value;

            if (currentTransition is not null)
            {
                currentTransition.Completed += OnTransitionCompleted;
            }
        }
    }

    partial void OnIsTransitioningChanged(
        bool oldValue,
        bool newValue)
    {
        if (!allowIsTransitioningPropertyWrite)
        {
            IsTransitioning = oldValue;
            throw new InvalidOperationException();
        }
    }

    partial void OnTransitionChanged(
        TransitionType oldValue,
        TransitionType newValue)
    {
        if (IsTransitioning)
        {
            AbortTransition();
        }

        var newStoryboard = GetStoryboard(newValue);

        if (newStoryboard is null)
        {
            if (VisualStates.TryGetVisualStateGroup(
                    this,
                    PresentationGroup) is null)
            {
                CurrentTransition = null;
            }
            else
            {
                SetValue(
                    TransitionProperty,
                    oldValue);
            }
        }
        else
        {
            CurrentTransition = newStoryboard;
        }
    }

    private static void OnRestartTransitionOnContentChangePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ((TransitioningContentControl)d).OnRestartTransitionOnContentChangeChanged(
            (bool)e.OldValue,
            (bool)e.NewValue);
    }

    /// <summary>
    /// Called when <see cref="RestartTransitionOnContentChange"/> changes.
    /// </summary>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    protected virtual void OnRestartTransitionOnContentChangeChanged(
        bool oldValue,
        bool newValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransitioningContentControl"/> class.
    /// </summary>
    public TransitioningContentControl()
    {
        CustomVisualStates = new ObservableCollection<VisualState>();
        DefaultStyleKey = typeof(TransitioningContentControl);
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (IsTransitioning)
        {
            AbortTransition();
        }

        if (CustomVisualStates is not null && CustomVisualStates.Any())
        {
            var presentationGroup = VisualStates.TryGetVisualStateGroup(
                this,
                PresentationGroup);
            if (presentationGroup is not null)
            {
                foreach (var state in CustomVisualStates)
                {
                    presentationGroup.States.Add(state);
                }
            }
        }

        base.OnApplyTemplate();

        ResolveTemplateParts();

        var storyboard = GetStoryboard(Transition);
        CurrentTransition = storyboard;
        if (storyboard is null)
        {
            var invalidTransition = Transition;
            Transition = DefaultTransitionState;

            throw new AtcAppsException($"'{invalidTransition}' transition could not be found!");
        }

        VisualStateManager.GoToState(
            this,
            HiddenState,
            false);
    }

    /// <inheritdoc />
    protected override void OnContentChanged(
        object oldContent,
        object newContent)
    {
        base.OnContentChanged(
            oldContent,
            newContent);

        if (oldContent != newContent)
        {
            StartTransition(
                oldContent,
                newContent);
        }
    }

    [SuppressMessage(
        "Microsoft.Usage",
        "CA1801:ReviewUnusedParameters",
        MessageId = "newContent",
        Justification = "Should be used in the future.")]
    private void StartTransition(
        object oldContent,
        object newContent)
    {
        // both presenters must be available, otherwise a transition is useless.
        if (currentContentPresentationSite is not null && previousContentPresentationSite is not null)
        {
            if (RestartTransitionOnContentChange
                && CurrentTransition is not null)
            {
                CurrentTransition.Completed -= OnTransitionCompleted;
            }

            currentContentPresentationSite.SetCurrentValue(
                ContentPresenter.ContentProperty,
                newContent);

            // A local value: a ContentPresenter in a ContentControl template is implicitly bound to the control's
            // Content by the template, and that template value would replace a current value with the new content.
            previousContentPresentationSite.SetValue(
                ContentPresenter.ContentProperty,
                oldContent);

            // and start a new transition
            if (!IsTransitioning || RestartTransitionOnContentChange)
            {
                if (RestartTransitionOnContentChange
                    && CurrentTransition is not null)
                {
                    CurrentTransition.Completed += OnTransitionCompleted;
                }

                IsTransitioning = true;
                VisualStateManager.GoToState(
                    this,
                    HiddenState,
                    false);
                VisualStateManager.GoToState(
                    this,
                    GetTransitionName(Transition),
                    true);
            }
        }
    }

    /// <summary>
    /// Reload the current transition if the content is the same.
    /// </summary>
    public void ReloadTransition()
    {
        // both presenters must be available, otherwise a transition is useless.
        if (currentContentPresentationSite is not null && previousContentPresentationSite is not null)
        {
            if (RestartTransitionOnContentChange
                && CurrentTransition is not null)
            {
                CurrentTransition.Completed -= OnTransitionCompleted;
            }

            if (!IsTransitioning || RestartTransitionOnContentChange)
            {
                if (RestartTransitionOnContentChange
                    && CurrentTransition is not null)
                {
                    CurrentTransition.Completed += OnTransitionCompleted;
                }

                IsTransitioning = true;
                VisualStateManager.GoToState(
                    this,
                    HiddenState,
                    false);
                VisualStateManager.GoToState(
                    this,
                    GetTransitionName(Transition),
                    true);
            }
        }
    }

    private void OnTransitionCompleted(
        object? sender,
        EventArgs e)
    {
        AbortTransition();
        if (sender is not ClockGroup clockGroup ||
            clockGroup.CurrentState == ClockState.Stopped)
        {
            TransitionCompleted?.Invoke(
                this,
                new RoutedEventArgs());
        }
    }

    /// <summary>
    /// Aborts the running transition, returns to the hidden state and releases the previous content.
    /// </summary>
    public void AbortTransition()
    {
        // go to normal state and release our hold on the old content.
        VisualStateManager.GoToState(
            this,
            HiddenState,
            false);
        IsTransitioning = false;
        previousContentPresentationSite?.SetValue(
            ContentPresenter.ContentProperty,
            null);
    }

    private Storyboard? GetStoryboard(TransitionType newTransition)
    {
        var presentationGroup = VisualStates.TryGetVisualStateGroup(
            this,
            PresentationGroup);
        if (presentationGroup is null)
        {
            return null;
        }

        var transitionName = GetTransitionName(newTransition);
        return presentationGroup.States
            .OfType<VisualState>()
            .Where(state => state.Name == transitionName)
            .Select(state => state.Storyboard)
            .FirstOrDefault();
    }

    private string GetTransitionName(TransitionType transition)
        => transition switch
        {
            TransitionType.Default => "DefaultTransition",
            TransitionType.Normal => "Normal",
            TransitionType.Up => "UpTransition",
            TransitionType.Down => "DownTransition",
            TransitionType.Right => "RightTransition",
            TransitionType.RightReplace => "RightReplaceTransition",
            TransitionType.Left => "LeftTransition",
            TransitionType.LeftReplace => "LeftReplaceTransition",
            TransitionType.Custom => CustomVisualStatesName,
            _ => "DefaultTransition",
        };
}