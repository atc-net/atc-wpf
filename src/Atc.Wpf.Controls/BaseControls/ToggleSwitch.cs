namespace Atc.Wpf.Controls.BaseControls;

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
public class ToggleSwitch : HeaderedContentControl, ICommandSource
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

    public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.Register(
        nameof(ContentDirection),
        typeof(FlowDirection),
        typeof(ToggleSwitch),
        new PropertyMetadata(FlowDirection.LeftToRight));

    public FlowDirection ContentDirection
    {
        get => (FlowDirection)GetValue(ContentDirectionProperty);
        set => SetValue(ContentDirectionProperty, value);
    }

    public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(
        nameof(ContentPadding),
        typeof(Thickness),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(
            new Thickness(0),
            FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
        nameof(IsOn),
        typeof(bool),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnIsOnChanged));

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(
        nameof(OnContent),
        typeof(object),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(
            "On",
            OnOnContentChanged));

    public object OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }

    public static readonly DependencyProperty OnContentTemplateProperty = DependencyProperty.Register(
        nameof(OnContentTemplate),
        typeof(DataTemplate),
        typeof(ToggleSwitch));

    public DataTemplate? OnContentTemplate
    {
        get => (DataTemplate?)GetValue(OnContentTemplateProperty);
        set => SetValue(OnContentTemplateProperty, value);
    }

    public static readonly DependencyProperty OnContentTemplateSelectorProperty = DependencyProperty.Register(
        nameof(OnContentTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public DataTemplateSelector? OnContentTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(OnContentTemplateSelectorProperty);
        set => SetValue(OnContentTemplateSelectorProperty, value);
    }

    public static readonly DependencyProperty OnContentStringFormatProperty = DependencyProperty.Register(
        nameof(OnContentStringFormat),
        typeof(string),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public string? OnContentStringFormat
    {
        get => (string?)GetValue(OnContentStringFormatProperty);
        set => SetValue(OnContentStringFormatProperty, value);
    }

    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(
        nameof(OffContent),
        typeof(object),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(
            "Off",
            OnOffContentChanged));

    public object OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    public static readonly DependencyProperty OffContentTemplateProperty = DependencyProperty.Register(
        nameof(OffContentTemplate),
        typeof(DataTemplate),
        typeof(ToggleSwitch));

    public DataTemplate? OffContentTemplate
    {
        get => (DataTemplate?)GetValue(OffContentTemplateProperty);
        set => SetValue(OffContentTemplateProperty, value);
    }

    public static readonly DependencyProperty OffContentTemplateSelectorProperty = DependencyProperty.Register(
        nameof(OffContentTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public DataTemplateSelector? OffContentTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(OffContentTemplateSelectorProperty);
        set => SetValue(OffContentTemplateSelectorProperty, value);
    }

    public static readonly DependencyProperty OffContentStringFormatProperty = DependencyProperty.Register(
        nameof(OffContentStringFormat),
        typeof(string),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public string? OffContentStringFormat
    {
        get => (string?)GetValue(OffContentStringFormatProperty);
        set => SetValue(OffContentStringFormatProperty, value);
    }

    private static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsPressed),
        typeof(bool),
        typeof(ToggleSwitch),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

    public bool IsPressed
    {
        get => (bool)GetValue(IsPressedProperty);
        protected set => SetValue(IsPressedPropertyKey, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(ToggleSwitch),
        new PropertyMetadata(
            defaultValue: null,
            OnCommandChanged));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty OnCommandProperty = DependencyProperty.Register(
        nameof(OnCommand),
        typeof(ICommand),
        typeof(ToggleSwitch),
        new PropertyMetadata(
            defaultValue: null,
            OnCommandChanged));

    public ICommand? OnCommand
    {
        get => (ICommand?)GetValue(OnCommandProperty);
        set => SetValue(OnCommandProperty, value);
    }

    public static readonly DependencyProperty OffCommandProperty = DependencyProperty.Register(
        nameof(OffCommand),
        typeof(ICommand),
        typeof(ToggleSwitch),
        new PropertyMetadata(
            defaultValue: null,
            OnCommandChanged));

    public ICommand? OffCommand
    {
        get => (ICommand?)GetValue(OffCommandProperty);
        set => SetValue(OffCommandProperty, value);
    }

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter),
        typeof(object),
        typeof(ToggleSwitch),
        new PropertyMetadata(propertyChangedCallback: null));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
        nameof(CommandTarget),
        typeof(IInputElement),
        typeof(ToggleSwitch),
        new PropertyMetadata(propertyChangedCallback: null));

    public IInputElement? CommandTarget
    {
        get => (IInputElement?)GetValue(CommandTargetProperty);
        set => SetValue(CommandTargetProperty, value);
    }

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

    protected override void OnKeyUp(
        KeyEventArgs e)
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

    protected override void OnRenderSizeChanged(
        SizeChangedInfo sizeInfo)
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
        => new Internal.ToggleSwitchAutomationPeer(this);

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

        if (UIElementAutomationPeer.FromElement(toggleSwitch) is Internal.ToggleSwitchAutomationPeer peer)
        {
            peer.RaiseToggleStatePropertyChangedEvent(oldValue, newValue);
        }

        toggleSwitch.OnToggled();
        toggleSwitch.UpdateVisualStates(useTransitions: true);
    }

    private static void OnOffContentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ToggleSwitch)d).OnOffContentChanged(e.OldValue, e.NewValue);

    private static void OnOnContentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ToggleSwitch)d).OnOnContentChanged(e.OldValue, e.NewValue);

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
        if (e.HorizontalChange == 0)
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

    private void UpdateVisualStates(
        bool useTransitions)
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

        if (SwitchThumb != null && SwitchThumb.IsDragging)
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
        => ((ToggleSwitch)d).OnCommandChanged((ICommand?)e.OldValue, (ICommand?)e.NewValue);

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

    private void UnhookCommand(
        ICommand command)
    {
        CanExecuteChangedEventManager.RemoveHandler(command, OnCanExecuteChanged);
        UpdateCanExecute();
    }

    private void HookCommand(
        ICommand command)
    {
        CanExecuteChangedEventManager.AddHandler(command, OnCanExecuteChanged);
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