namespace Atc.Wpf.Controls.Inputs;

[ContentProperty(nameof(Content))]
[TemplatePart(Name = nameof(HeaderContentPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(ContentPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(OffContentPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(OnContentPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(SwitchKnobBounds), Type = typeof(FrameworkElement))]
[TemplatePart(Name = nameof(SwitchKnob), Type = typeof(FrameworkElement))]
[TemplatePart(Name = nameof(KnobTranslateTransform), Type = typeof(TranslateTransform))]
[TemplatePart(Name = nameof(SwitchThumb), Type = typeof(Thumb))]
[TemplateVisualState(GroupName = Internal.VisualStates.GroupCommon, Name = Internal.VisualStates.StateNormal)]
[TemplateVisualState(GroupName = Internal.VisualStates.GroupCommon, Name = Internal.VisualStates.StateMouseOver)]
[TemplateVisualState(GroupName = Internal.VisualStates.GroupCommon, Name = Internal.VisualStates.StatePressed)]
[TemplateVisualState(GroupName = Internal.VisualStates.GroupCommon, Name = Internal.VisualStates.StateDisabled)]
[TemplateVisualState(GroupName = ContentStatesGroup, Name = OffContentState)]
[TemplateVisualState(GroupName = ContentStatesGroup, Name = OnContentState)]
[TemplateVisualState(GroupName = ToggleStatesGroup, Name = DraggingState)]
[TemplateVisualState(GroupName = ToggleStatesGroup, Name = OffState)]
[TemplateVisualState(GroupName = ToggleStatesGroup, Name = OnState)]
public partial class ToggleSwitch : HeaderedContentControl, ICommandSource
{
    private const string ContentStatesGroup = "ContentStates";
    private const string OffContentState = "OffContent";
    private const string OnContentState = "OnContent";
    private const string ToggleStatesGroup = "ToggleStates";
    private const string DraggingState = "Dragging";
    private const string OffState = "Off";
    private const string OnState = "On";

    private double onTranslation;
    private double startTranslation;
    private bool wasDragged;
    private bool canExecute = true;

    private ContentPresenter? HeaderContentPresenter { get; set; }

    private ContentPresenter? ContentPresenter { get; set; }

    private ContentPresenter? OffContentPresenter { get; set; }

    private ContentPresenter? OnContentPresenter { get; set; }

    private FrameworkElement? SwitchKnobBounds { get; set; }

    private FrameworkElement? SwitchKnob { get; set; }

    private TranslateTransform? KnobTranslateTransform { get; set; }

    private Thumb? SwitchThumb { get; set; }

    [DependencyProperty(DefaultValue = FlowDirection.LeftToRight)]
    private FlowDirection contentDirection;

    [DependencyProperty(
        DefaultValue = "new Thickness(0,1,0,0)",
        Flags = FrameworkPropertyMetadataOptions.AffectsParentMeasure)]
    private Thickness contentPadding;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsOnChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private bool isOn;

    [DependencyProperty(
        DefaultValue = "\"On\"",
        PropertyChangedCallback = nameof(OnOnContentChanged),
        Flags = FrameworkPropertyMetadataOptions.None)]
    private object onContent;

    [DependencyProperty]
    private DataTemplate? onContentTemplate;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.None)]
    private DataTemplateSelector? onContentTemplateSelector;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.None)]
    private string? onContentStringFormat;

    [DependencyProperty(
        DefaultValue = "\"Off\"",
        PropertyChangedCallback = nameof(OnOffContentChanged),
        Flags = FrameworkPropertyMetadataOptions.None)]
    private object offContent;

    [DependencyProperty]
    private DataTemplate? offContentTemplate;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.None)]
    private DataTemplateSelector? offContentTemplateSelector;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.None)]
    private string? offContentStringFormat;

    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.None)]
    private bool isPressed;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnCommandChanged))]
    private ICommand? command;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnCommandChanged))]
    private ICommand? onCommand;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnCommandChanged))]
    private ICommand? offCommand;

    [DependencyProperty]
    private object? commandParameter;

    [DependencyProperty]
    private IInputElement? commandTarget;

    public event RoutedEventHandler? Toggled;

    static ToggleSwitch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(typeof(ToggleSwitch)));

        EventManager.RegisterClassHandler(
            typeof(ToggleSwitch),
            MouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnMouseLeftButtonDown),
            handledEventsToo: true);
    }

    public ToggleSwitch()
    {
        IsEnabledChanged += OnIsEnabledChanged;
    }

    protected virtual void OnOnContentChanged(
        object oldContent,
        object newContent)
    {
    }

    protected virtual void OnOffContentChanged(
        object oldContent,
        object newContent)
    {
    }

    protected virtual void OnToggled()
        => Toggled?.Invoke(this, new RoutedEventArgs());

    public override void OnApplyTemplate()
    {
        if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
        {
            SwitchThumb.DragStarted -= OnSwitchThumbDragStarted;
            SwitchThumb.DragDelta -= OnSwitchThumbDragDelta;
            SwitchThumb.DragCompleted -= OnSwitchThumbDragCompleted;
        }

        base.OnApplyTemplate();

        HeaderContentPresenter = GetTemplateChild(nameof(HeaderContentPresenter)) as ContentPresenter;
        ContentPresenter = GetTemplateChild(nameof(ContentPresenter)) as ContentPresenter;
        OffContentPresenter = GetTemplateChild(nameof(OffContentPresenter)) as ContentPresenter;
        OnContentPresenter = GetTemplateChild(nameof(OnContentPresenter)) as ContentPresenter;
        SwitchKnobBounds = GetTemplateChild(nameof(SwitchKnobBounds)) as FrameworkElement;
        SwitchKnob = GetTemplateChild(nameof(SwitchKnob)) as FrameworkElement;
        KnobTranslateTransform = GetTemplateChild(nameof(KnobTranslateTransform)) as TranslateTransform;
        SwitchThumb = GetTemplateChild(nameof(SwitchThumb)) as Thumb;

        if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
        {
            SwitchThumb.DragStarted += OnSwitchThumbDragStarted;
            SwitchThumb.DragDelta += OnSwitchThumbDragDelta;
            SwitchThumb.DragCompleted += OnSwitchThumbDragCompleted;
        }

        UpdateHeaderContentPresenterVisibility();
        UpdateContentPresenterVisibility();

        UpdateVisualStates(useTransitions: false);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Key == Key.Space)
        {
            e.Handled = true;
            Toggle();
        }

        base.OnKeyUp(e);
    }

    protected override void OnHeaderChanged(
        object oldHeader,
        object newHeader)
    {
        base.OnHeaderChanged(oldHeader, newHeader);

        UpdateHeaderContentPresenterVisibility();
    }

    protected override void OnContentChanged(
        object oldContent,
        object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        UpdateContentPresenterVisibility();
    }

    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == IsMouseOverProperty)
        {
            UpdateVisualStates(useTransitions: true);
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (SwitchKnobBounds != null && SwitchKnob != null)
        {
            onTranslation = SwitchKnobBounds.ActualWidth - SwitchKnob.ActualWidth - SwitchKnob.Margin.Left - SwitchKnob.Margin.Right;
        }
    }

    protected override bool IsEnabledCore
        => base.IsEnabledCore && CanExecute;

    protected override AutomationPeer OnCreateAutomationPeer()
        => new ToggleSwitchAutomationPeer(this);

    private static void OnIsOnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ToggleSwitch toggleSwitch ||
            e.NewValue == e.OldValue ||
            e.NewValue is not bool newValue ||
            e.OldValue is not bool oldValue)
        {
            return;
        }

        if (UIElementAutomationPeer.FromElement(toggleSwitch) is ToggleSwitchAutomationPeer peer)
        {
            peer.RaiseToggleStatePropertyChangedEvent(oldValue, newValue);
        }

        toggleSwitch.OnToggled();
        toggleSwitch.UpdateVisualStates(useTransitions: true);
    }

    private static void OnOffContentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ToggleSwitch)d).OnOffContentChanged(
            e.OldValue,
            e.NewValue);

    private static void OnOnContentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ToggleSwitch)d).OnOnContentChanged(
            e.OldValue,
            e.NewValue);

    private void OnIsEnabledChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
        => UpdateVisualStates(useTransitions: true);

    private static void OnMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is ToggleSwitch { IsKeyboardFocusWithin: false } toggle)
        {
            e.Handled = toggle.Focus() || e.Handled;
        }
    }

    private void UpdateHeaderContentPresenterVisibility()
    {
        if (HeaderContentPresenter == null)
        {
            return;
        }

        var showHeader = (Header is string s && !string.IsNullOrEmpty(s)) || Header != null;
        HeaderContentPresenter.Visibility = showHeader ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateContentPresenterVisibility()
    {
        if (ContentPresenter is null ||
            OffContentPresenter is null ||
            OnContentPresenter is null)
        {
            return;
        }

        var showContent = (Content is string s && !string.IsNullOrEmpty(s)) || Content != null;
        ContentPresenter.Visibility = showContent ? Visibility.Visible : Visibility.Collapsed;
        OffContentPresenter.Visibility = !showContent ? Visibility.Visible : Visibility.Collapsed;
        OnContentPresenter.Visibility = !showContent ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnSwitchThumbDragStarted(
        object sender,
        DragStartedEventArgs e)
    {
        e.Handled = true;
        IsPressed = true;
        wasDragged = false;
        startTranslation = KnobTranslateTransform!.X;
        UpdateVisualStates(useTransitions: true);
        KnobTranslateTransform.X = startTranslation;
    }

    private void OnSwitchThumbDragDelta(
        object sender,
        DragDeltaEventArgs e)
    {
        e.Handled = true;
        if (e.HorizontalChange.IsZero())
        {
            return;
        }

        wasDragged = System.Math.Abs(e.HorizontalChange) >= SystemParameters.MinimumHorizontalDragDistance;

        var dragTranslation = startTranslation + e.HorizontalChange;
        KnobTranslateTransform!.X = System.Math.Max(0, System.Math.Min(onTranslation, dragTranslation));
    }

    private void OnSwitchThumbDragCompleted(
        object sender,
        DragCompletedEventArgs e)
    {
        e.Handled = true;
        IsPressed = false;
        if (wasDragged)
        {
            switch (IsOn)
            {
                case false when KnobTranslateTransform!.X + (SwitchKnob!.ActualWidth / 2) >= SwitchKnobBounds!.ActualWidth / 2:
                case true when KnobTranslateTransform!.X + (SwitchKnob!.ActualWidth / 2) <= SwitchKnobBounds!.ActualWidth / 2:
                    Toggle();
                    break;
                default:
                    UpdateVisualStates(useTransitions: true);
                    break;
            }
        }
        else
        {
            Toggle();
        }

        wasDragged = false;
    }

    private void UpdateVisualStates(bool useTransitions)
    {
        string stateName;

        if (!IsEnabled)
        {
            stateName = Internal.VisualStates.StateDisabled;
        }
        else if (IsPressed)
        {
            stateName = Internal.VisualStates.StatePressed;
        }
        else if (IsMouseOver)
        {
            stateName = Internal.VisualStates.StateMouseOver;
        }
        else
        {
            stateName = Internal.VisualStates.StateNormal;
        }

        VisualStateManager.GoToState(this, stateName, useTransitions);

        if (SwitchThumb is { IsDragging: true })
        {
            stateName = DraggingState;
        }
        else
        {
            stateName = IsOn ? OnState : OffState;
        }

        VisualStateManager.GoToState(this, stateName, useTransitions);

        VisualStateManager.GoToState(this, IsOn ? OnContentState : OffContentState, useTransitions);
    }

    private void Toggle()
    {
        var newValue = !IsOn;
        SetCurrentValue(IsOnProperty, BooleanBoxes.Box(newValue));

        CommandHelpers.ExecuteCommandSource(this);
        CommandHelpers.ExecuteCommandSource(this, newValue ? OnCommand : OffCommand);
    }

    private static void OnCommandChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ToggleSwitch)d).OnCommandChanged(
            (ICommand?)e.OldValue,
            (ICommand?)e.NewValue);

    private void OnCommandChanged(
        ICommand? oldCommand,
        ICommand? newCommand)
    {
        if (oldCommand != null)
        {
            UnhookCommand(oldCommand);
        }

        if (newCommand != null)
        {
            HookCommand(newCommand);
        }
    }

    private void UnhookCommand(ICommand cmd)
    {
        CanExecuteChangedEventManager.RemoveHandler(cmd, OnCanExecuteChanged);
        UpdateCanExecute();
    }

    private void HookCommand(ICommand cmd)
    {
        CanExecuteChangedEventManager.AddHandler(cmd, OnCanExecuteChanged);
        UpdateCanExecute();
    }

    private void OnCanExecuteChanged(
        object? sender,
        EventArgs e)
        => UpdateCanExecute();

    private void UpdateCanExecute()
        => CanExecute = Command is null ||
                        CommandHelpers.CanExecuteCommandSource(this);

    private bool CanExecute
    {
        get => canExecute;
        set
        {
            if (value == canExecute)
            {
                return;
            }

            canExecute = value;
            CoerceValue(IsEnabledProperty);
        }
    }

    internal void AutomationPeerToggle()
        => Toggle();
}