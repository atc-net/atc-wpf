// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotDisposedResourceIsReturnedByProperty
namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// An extended Window class.
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "OK.")]
[TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
[TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
[TemplatePart(Name = PART_WindowTitleBackground, Type = typeof(UIElement))]
[TemplatePart(Name = PART_WindowTitleThumb, Type = typeof(Thumb))]
[TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_RightWindowCommands, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_WindowButtonCommands, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_OverlayBox, Type = typeof(Grid))]
[TemplatePart(Name = PART_NiceActiveDialogContainer, Type = typeof(Grid))]
[TemplatePart(Name = PART_NiceInactiveDialogsContainer, Type = typeof(Grid))]
[TemplatePart(Name = PART_Content, Type = typeof(NiceContentControl))]
public partial class NiceWindow : WindowChromeWindow
{
    private const string PART_Icon = "PART_Icon";
    private const string PART_TitleBar = "PART_TitleBar";
    private const string PART_WindowTitleBackground = "PART_WindowTitleBackground";
    private const string PART_WindowTitleThumb = "PART_WindowTitleThumb";
    private const string PART_LeftWindowCommands = "PART_LeftWindowCommands";
    private const string PART_RightWindowCommands = "PART_RightWindowCommands";
    private const string PART_WindowButtonCommands = "PART_WindowButtonCommands";
    private const string PART_OverlayBox = "PART_OverlayBox";
    private const string PART_NiceActiveDialogContainer = "PART_NiceActiveDialogContainer";
    private const string PART_NiceInactiveDialogsContainer = "PART_NiceInactiveDialogsContainer";
    private const string PART_Content = "PART_Content";

    private FrameworkElement? icon;
    private UIElement? titleBar;
    private UIElement? titleBarBackground;
    private Thumb? windowTitleThumb;
    private IInputElement? restoreFocus;
    internal ContentPresenter? LeftWindowCommandsPresenter;
    internal ContentPresenter? RightWindowCommandsPresenter;
    internal ContentPresenter? WindowButtonCommandsPresenter;

    internal Grid? OverlayBox;
    internal Grid? NiceActiveDialogContainer;
    internal Grid? NiceInactiveDialogContainer;
    private Storyboard? overlayStoryboard;

    private EventHandler? onOverlayFadeInStoryboardCompleted;
    private EventHandler? onOverlayFadeOutStoryboardCompleted;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnShowIconOnTitleBarPropertyChanged))]
    private bool showIconOnTitleBar;

    [DependencyProperty(DefaultValue = 20.0d)]
    private double iconWidth;

    [DependencyProperty(DefaultValue = 20.0d)]
    private double iconHeight;

    [DependencyProperty(DefaultValue = "new Thickness(10, 3, 10, 3)")]
    private Thickness iconMargin;

    [DependencyProperty(DefaultValue = EdgeMode.Aliased)]
    private EdgeMode iconEdgeMode;

    [DependencyProperty(DefaultValue = BitmapScalingMode.HighQuality)]
    private BitmapScalingMode iconBitmapScalingMode;

    [DependencyProperty(
        DefaultValue = MultiFrameImageMode.ScaleDownLargerFrame,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private MultiFrameImageMode iconScalingMode;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnShowTitleBarPropertyChanged),
        CoerceValueCallback = nameof(OnShowTitleBarCoerceValue))]
    private bool showTitleBar;

    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private bool showDialogsOverTitleBar;

    [DependencyProperty(DefaultValue = false)]
    private bool isAnyDialogOpen;

    [DependencyProperty(DefaultValue = true)]
    private bool showCloseButton;

    [DependencyProperty(DefaultValue = true)]
    private bool isMinButtonEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isMaxRestoreButtonEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isCloseButtonEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool showSystemMenu;

    [DependencyProperty(DefaultValue = true)]
    private bool showSystemMenuOnRightClick;

    [DependencyProperty(
        DefaultValue = 30,
        PropertyChangedCallback = nameof(TitleBarHeightPropertyChanged))]
    private int titleBarHeight;

    [DependencyProperty(
        DefaultValue = CharacterCasing.Normal,
        Flags = FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(ValidateTitleCharacterCasing))]
    private CharacterCasing titleCharacterCasing;

    [DependencyProperty(
        DefaultValue = HorizontalAlignment.Stretch,
        PropertyChangedCallback = nameof(OnTitleAlignmentChanged))]
    private HorizontalAlignment titleAlignment;

    [DependencyProperty(DefaultValue = false)]
    private bool saveWindowPosition;

    [DependencyProperty]
    private IWindowPlacementSettings? windowPlacementSettings;

    [DependencyProperty]
    private Brush? titleForeground;

    [DependencyProperty]
    private DataTemplate? titleTemplate;

    [DependencyProperty(DefaultValue = nameof(Brushes.Transparent))]
    private Brush windowTitleBrush;

    [DependencyProperty(DefaultValue = nameof(Brushes.Gray))]
    private Brush nonActiveWindowTitleBrush;

    [DependencyProperty(DefaultValue = nameof(Brushes.Gray))]
    private Brush nonActiveBorderBrush;

    [DependencyProperty]
    private Brush? overlayBrush;

    [DependencyProperty(DefaultValue = 0.7d)]
    private double overlayOpacity;

    [DependencyProperty]
    private Storyboard? overlayFadeIn;

    [DependencyProperty]
    private Storyboard? overlayFadeOut;

    [DependencyProperty(DefaultValue = false)]
    private bool windowTransitionsEnabled;

    [DependencyProperty(PropertyChangedCallback = nameof(OnIconTemplatePropertyChanged))]
    private DataTemplate? iconTemplate;

    [DependencyProperty(PropertyChangedCallback = nameof(OnLeftWindowCommandsPropertyChanged))]
    private WindowCommands? leftWindowCommands;

    [DependencyProperty(PropertyChangedCallback = nameof(OnRightWindowCommandsPropertyChanged))]
    private WindowCommands? rightWindowCommands;

    [DependencyProperty(PropertyChangedCallback = nameof(UpdateLogicalChildren))]
    private WindowButtonCommands? windowButtonCommands;

    [DependencyProperty(
        DefaultValue = WindowCommandsOverlayBehaviorType.Never,
        PropertyChangedCallback = nameof(OnShowTitleBarPropertyChanged))]
    private WindowCommandsOverlayBehaviorType leftWindowCommandsOverlayBehavior;

    [DependencyProperty(
        DefaultValue = WindowCommandsOverlayBehaviorType.Never,
        PropertyChangedCallback = nameof(OnShowTitleBarPropertyChanged))]
    private WindowCommandsOverlayBehaviorType rightWindowCommandsOverlayBehavior;

    [DependencyProperty(
        DefaultValue = OverlayBehavior.Always,
        PropertyChangedCallback = nameof(OnShowTitleBarPropertyChanged))]
    private OverlayBehavior windowButtonCommandsOverlayBehavior;

    [DependencyProperty(
        DefaultValue = OverlayBehavior.Never,
        PropertyChangedCallback = nameof(OnShowTitleBarPropertyChanged))]
    private OverlayBehavior iconOverlayBehavior;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnUseNoneWindowStylePropertyChanged))]
    private bool useNoneWindowStyle;

    [DependencyProperty]
    private Brush? overrideDefaultWindowCommandsBrush;

    [DependencyProperty(DefaultValue = true)]
    private bool isWindowDraggable;

    [RoutedEvent]
    private static readonly RoutedEvent windowTransitionCompleted;

    /// <summary>
    /// Gets the window placement settings (can be overwritten).
    /// </summary>
    public virtual IWindowPlacementSettings GetWindowPlacementSettings()
        => WindowPlacementSettings ?? new WindowApplicationSettings(this);

    /// <summary>
    /// Starts the overlay fade in effect.
    /// </summary>
    /// <returns>A task representing the process.</returns>
    public async Task ShowOverlayAsync()
    {
        if (OverlayBox is null)
        {
            throw new InvalidOperationException("OverlayBox can not be founded in this NiceWindow's template. Are you calling this before the window has loaded?");
        }

        if (IsOverlayVisible() && overlayStoryboard is null)
        {
            return;
        }

        Dispatcher.VerifyAccess();

        var sb = OverlayFadeIn?.Clone();

        if (!CanUseOverlayFadingStoryboard(sb, out var animation))
        {
            await ShowOverlayAsync().ConfigureAwait(true);
            return;
        }

        overlayStoryboard = sb;

        var tcs = new TaskCompletionSource<object>();

        OverlayBox.SetCurrentValue(VisibilityProperty, Visibility.Visible);
        animation.To = OverlayOpacity;

        onOverlayFadeInStoryboardCompleted = (_, _) =>
        {
            sb.Completed -= onOverlayFadeInStoryboardCompleted;
            if (overlayStoryboard == sb)
            {
                overlayStoryboard = null;
            }

            tcs.TrySetResult(null!);
        };

        sb.Completed += onOverlayFadeInStoryboardCompleted;

        OverlayBox.BeginStoryboard(sb);

        await tcs.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the overlay fade out effect.
    /// </summary>
    /// <returns>A task representing the process.</returns>
    public async Task HideOverlayAsync()
    {
        if (OverlayBox is null)
        {
            throw new InvalidOperationException("OverlayBox can not be founded in this NiceWindow's template. Are you calling this before the window has loaded?");
        }

        if (OverlayBox.Visibility == Visibility.Visible && OverlayBox.Opacity <= 0.0)
        {
            OverlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
            return;
        }

        Dispatcher.VerifyAccess();

        var sb = OverlayFadeOut?.Clone();

        if (!CanUseOverlayFadingStoryboard(sb, out var animation))
        {
            await ShowOverlayAsync().ConfigureAwait(true);
            return;
        }

        overlayStoryboard = sb;

        var tcs = new TaskCompletionSource<object>();

        animation.To = 0d;

        onOverlayFadeOutStoryboardCompleted = (_, _) =>
        {
            sb.Completed -= onOverlayFadeOutStoryboardCompleted;
            if (overlayStoryboard == sb)
            {
                OverlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                overlayStoryboard = null;
            }

            tcs.TrySetResult(null!);
        };

        sb.Completed += onOverlayFadeOutStoryboardCompleted;

        OverlayBox.BeginStoryboard(sb);

        await tcs.Task.ConfigureAwait(true);
    }

    public bool IsOverlayVisible()
    {
        if (OverlayBox is null)
        {
            throw new InvalidOperationException("OverlayBox can not be founded in this NiceWindow's template. Are you calling this before the window has loaded?");
        }

        return OverlayBox.Visibility == Visibility.Visible && OverlayBox.Opacity >= OverlayOpacity;
    }

    public void ShowOverlay()
    {
        if (OverlayBox is null)
        {
            return;
        }

        OverlayBox.SetCurrentValue(VisibilityProperty, Visibility.Visible);
        OverlayBox.SetCurrentValue(OpacityProperty, OverlayOpacity);
    }

    public void HideOverlay()
    {
        if (OverlayBox is null)
        {
            return;
        }

        OverlayBox.SetCurrentValue(OpacityProperty, 0d);
        OverlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
    }

    /// <summary>
    /// Stores the given element, or the last focused element via FocusManager, for restoring the focus after closing a dialog.
    /// </summary>
    /// <param name="thisElement">The element which will be focused again.</param>
    public void StoreFocus(
        IInputElement? thisElement = null)
    {
        this.Invoke(() =>
        {
            restoreFocus = thisElement ?? restoreFocus ?? FocusManager.GetFocusedElement(this);
        });
    }

    internal void RestoreFocus()
    {
        if (restoreFocus is null)
        {
            return;
        }

        this.Invoke(() =>
        {
            Keyboard.Focus(restoreFocus);
            restoreFocus = null;
        });
    }

    /// <summary>
    /// Clears the stored element which would get the focus after closing a dialog.
    /// </summary>
    public void ResetStoredFocus()
    {
        restoreFocus = null;
    }

    [SuppressMessage("Minor Code Smell", "S3963:\"static\" fields should be initialized inline", Justification = "OK.")]
    static NiceWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NiceWindow),
            new FrameworkPropertyMetadata(typeof(NiceWindow)));

        EventManager.RegisterClassHandler(
            typeof(NiceWindow),
            AccessKeyManager.AccessKeyPressedEvent,
            new AccessKeyPressedEventHandler(OnAccessKeyPressed));

        IconProperty.OverrideMetadata(
            typeof(NiceWindow),
            new FrameworkPropertyMetadata((o, e) =>
            {
                if (e.NewValue != e.OldValue)
                {
                    (o as NiceWindow)?.UpdateIconVisibility();
                }
            }));
    }

    public NiceWindow()
    {
        InitializeSettingsBehavior();

        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        ContentRendered += OnContentRendered;
    }

    protected override void OnClosing(
        CancelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnClosing(e);
    }

    /// <inheritdoc />
    protected override IEnumerator LogicalChildren
    {
        get
        {
            var children = new ArrayList();
            if (Content is not null)
            {
                children.Add(Content);
            }

            if (LeftWindowCommands is not null)
            {
                children.Add(LeftWindowCommands);
            }

            if (RightWindowCommands is not null)
            {
                children.Add(RightWindowCommands);
            }

            if (WindowButtonCommands is not null)
            {
                children.Add(WindowButtonCommands);
            }

            return children.GetEnumerator();
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        LeftWindowCommandsPresenter = GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
        RightWindowCommandsPresenter = GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
        WindowButtonCommandsPresenter = GetTemplateChild(PART_WindowButtonCommands) as ContentPresenter;

        LeftWindowCommands ??= new WindowCommands();
        RightWindowCommands ??= new WindowCommands();
        WindowButtonCommands ??= new WindowButtonCommands();

        LeftWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
        RightWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
        WindowButtonCommands.SetValue(WindowButtonCommands.ParentWindowPropertyKey, this);

        OverlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
        NiceActiveDialogContainer = GetTemplateChild(PART_NiceActiveDialogContainer) as Grid;
        NiceInactiveDialogContainer = GetTemplateChild(PART_NiceInactiveDialogsContainer) as Grid;

        icon = GetTemplateChild(PART_Icon) as FrameworkElement;
        titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
        titleBarBackground = GetTemplateChild(PART_WindowTitleBackground) as UIElement;
        windowTitleThumb = GetTemplateChild(PART_WindowTitleThumb) as Thumb;

        UpdateTitleBarElementsVisibility();

        if (GetTemplateChild(PART_Content) is NiceContentControl niceContentControl)
        {
            niceContentControl.TransitionCompleted += (_, _) => RaiseEvent(new RoutedEventArgs(WindowTransitionCompletedEvent));
        }
    }

    /// <summary>
    /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
    /// </summary>
    protected override AutomationPeer OnCreateAutomationPeer()
        => new NiceWindowAutomationPeer(this);

    protected internal IntPtr CriticalHandle
    {
        get
        {
            VerifyAccess();

            var value = typeof(Window).GetProperty(
                "CriticalHandle",
                BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this, []) ?? IntPtr.Zero;

            return (IntPtr)value;
        }
    }

    internal static void DoWindowTitleThumbOnPreviewMouseLeftButtonUp(
        NiceWindow window,
        MouseButtonEventArgs e)
    {
        if (e.Source != e.OriginalSource)
        {
            return;
        }

        Mouse.Capture(element: null);
    }

    internal static void DoWindowTitleThumbMoveOnDragDelta(
        INiceThumb? thumb,
        NiceWindow? window,
        DragDeltaEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(thumb);
        ArgumentNullException.ThrowIfNull(window);

        if (!window.IsWindowDraggable ||
            ((System.Math.Abs(e.HorizontalChange) <= 2) && (System.Math.Abs(e.VerticalChange) <= 2)))
        {
            return;
        }

        window.VerifyAccess();

        var windowIsMaximized = window.WindowState == WindowState.Maximized;
        var isMouseOnTitlebar = Mouse.GetPosition(thumb).Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
        if (!isMouseOnTitlebar && windowIsMaximized)
        {
            return;
        }

        PInvoke.ReleaseCapture();

        if (windowIsMaximized)
        {
            // ReSharper disable once ConvertToLocalFunction
            EventHandler? onWindowStateChanged = null;
            onWindowStateChanged = (_, _) =>
            {
                window.StateChanged -= onWindowStateChanged;

                if (window.WindowState == WindowState.Normal)
                {
                    Mouse.Capture(thumb, CaptureMode.Element);
                }
            };

            window.StateChanged -= onWindowStateChanged;
            window.StateChanged += onWindowStateChanged;
        }

        var wpfPoint = window.PointToScreen(Mouse.GetPosition(window));
        var x = (int)wpfPoint.X;
        var y = (int)wpfPoint.Y;

        PInvoke.SendMessage(
            new HWND(window.CriticalHandle),
            PInvoke.WM_NCLBUTTONDOWN,
            new WPARAM((nuint)HT.CAPTION),
            new IntPtr(x | y << 16));
    }

    internal static void DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(
        NiceWindow window,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        var canResize = window.ResizeMode is ResizeMode.CanResizeWithGrip or ResizeMode.CanResize;
        var mousePos = Mouse.GetPosition(window);
        var isMouseOnTitlebar = mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
        if (!canResize || !isMouseOnTitlebar)
        {
            return;
        }

#pragma warning disable 618
        if (window.WindowState == WindowState.Normal)
        {
            ControlzEx.SystemCommands.MaximizeWindow(window);
        }
        else
        {
            ControlzEx.SystemCommands.RestoreWindow(window);
        }
#pragma warning restore 618

        e.Handled = true;
    }

    internal static void DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(
        NiceWindow window,
        MouseButtonEventArgs e)
    {
        if (!window.ShowSystemMenuOnRightClick)
        {
            return;
        }

        var mousePos = e.GetPosition(window);
        if ((mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0) ||
            window is { UseNoneWindowStyle: true, TitleBarHeight: <= 0 })
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.ShowSystemMenuPhysicalCoordinates(window, window.PointToScreen(mousePos));
#pragma warning restore 618
        }
    }

    /// <summary>
    /// Gets the template child with the given name.
    /// </summary>
    /// <typeparam name="T">The interface type inherited from DependencyObject.</typeparam>
    /// <param name="name">The name of the template child.</param>
    internal T? GetPart<T>(string name)
        where T : class
        => GetTemplateChild(name) as T;

    private static void OnShowIconOnTitleBarPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var window = (NiceWindow)d;
        if (e.NewValue != e.OldValue)
        {
            window.UpdateIconVisibility();
        }
    }

    [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "OK.")]
    private static object? OnShowTitleBarCoerceValue(
        DependencyObject d,
        object? value)
    {
        // if UseNoneWindowStyle = true no title bar should be shown
        return ((NiceWindow)d).UseNoneWindowStyle
            ? false
            : value;
    }

    private static bool ValidateTitleCharacterCasing(
        object value)
        => value is >= CharacterCasing.Normal and <= CharacterCasing.Upper;

    private static void TitleBarHeightPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).UpdateTitleBarElementsVisibility();
    }

    private static void OnTitleAlignmentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        var window = (NiceWindow)d;

        window.SizeChanged -= window.OnSizeChanged;
        if (e.NewValue is HorizontalAlignment.Center &&
            window.titleBar is not null)
        {
            window.SizeChanged += window.OnSizeChanged;
        }
    }

    private static void OnIconTemplatePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var window = (NiceWindow)d;

        if (e.NewValue != e.OldValue)
        {
            window.UpdateIconVisibility();
        }
    }

    private static void OnLeftWindowCommandsPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is WindowCommands windowCommands)
        {
            AutomationProperties.SetName(windowCommands, nameof(LeftWindowCommands));
        }

        UpdateLogicalChildren(d, e);
    }

    private static void OnRightWindowCommandsPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is WindowCommands windowCommands)
        {
            AutomationProperties.SetName(windowCommands, nameof(RightWindowCommands));
        }

        UpdateLogicalChildren(d, e);
    }

    private static void OnShowTitleBarPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).UpdateTitleBarElementsVisibility();
    }

    private static void OnUseNoneWindowStylePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).CoerceValue(ShowTitleBarProperty);
    }

    private void UpdateIconVisibility()
    {
        var isVisible = (Icon is not null || IconTemplate is not null) &&
                        ((IconOverlayBehavior.HasFlag(OverlayBehavior.HiddenTitleBar) && !ShowTitleBar) || (ShowIconOnTitleBar && ShowTitleBar));

        var visibility = isVisible
            ? Visibility.Visible
            : Visibility.Collapsed;

        icon?.SetCurrentValue(
            VisibilityProperty,
            visibility);
    }

    private void UpdateTitleBarElementsVisibility()
    {
        UpdateIconVisibility();

        var newVisibility = TitleBarHeight > 0 &&
                            ShowTitleBar &&
                            !UseNoneWindowStyle ? Visibility.Visible : Visibility.Collapsed;

        titleBar?.SetCurrentValue(VisibilityProperty, newVisibility);
        titleBarBackground?.SetCurrentValue(VisibilityProperty, newVisibility);

        var leftWindowCommandsVisibility = LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehaviorType.HiddenTitleBar) && !UseNoneWindowStyle ? Visibility.Visible : newVisibility;
        LeftWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, leftWindowCommandsVisibility);

        var rightWindowCommandsVisibility = RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehaviorType.HiddenTitleBar) && !UseNoneWindowStyle ? Visibility.Visible : newVisibility;
        RightWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, rightWindowCommandsVisibility);

        var windowButtonCommandsVisibility = WindowButtonCommandsOverlayBehavior.HasFlag(OverlayBehavior.HiddenTitleBar) ? Visibility.Visible : newVisibility;
        WindowButtonCommandsPresenter?.SetCurrentValue(VisibilityProperty, windowButtonCommandsVisibility);

        SetWindowEvents();
    }

    private static bool CanUseOverlayFadingStoryboard(
        [NotNullWhen(true)] Storyboard? sb,
        [NotNullWhen(true)] out DoubleAnimation? animation)
    {
        animation = null;

        if (sb is null)
        {
            return false;
        }

        sb.Dispatcher.VerifyAccess();

        animation = sb.Children.OfType<DoubleAnimation>().FirstOrDefault();

        if (animation is null)
        {
            return false;
        }

        return sb.Duration is { HasTimeSpan: true, TimeSpan.Ticks: > 0 }
               || sb.AccelerationRatio > 0
               || sb.DecelerationRatio > 0
               || animation.Duration is { HasTimeSpan: true, TimeSpan.Ticks: > 0 }
               || animation.AccelerationRatio > 0
               || animation.DecelerationRatio > 0;
    }

    private static void OnAccessKeyPressed(
        object sender,
        AccessKeyPressedEventArgs e)
    {
        if (e.Handled || sender is not NiceWindow { IsAnyDialogOpen: true })
        {
            return;
        }

        e.Scope = null;
        e.Target = null;
        e.Handled = true;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        this.ResetAllWindowCommandsBrush();

        ThemeManager.Current.ThemeChanged += HandleThemeManagerThemeChanged;
        Unloaded += (_, _) => ThemeManager.Current.ThemeChanged -= HandleThemeManagerThemeChanged;

        if (CornerPreference != ControlzEx.Behaviors.WindowCornerPreference.Default)
        {
            var backupCornerPreference = CornerPreference;
            CornerPreference = ControlzEx.Behaviors.WindowCornerPreference.Default;
            CornerPreference = backupCornerPreference;
        }
    }

    private static void OnContentRendered(
        object? sender,
        EventArgs e)
        => CultureManager.SetCultures(
            GlobalizationConstants.EnglishCultureInfo,
            CultureManager.UiCulture);

    private void InitializeSettingsBehavior()
    {
        Interaction.GetBehaviors(this).Add(new WindowsSettingBehavior());
    }

    private void OnDataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (LeftWindowCommands is not null)
        {
            LeftWindowCommands.DataContext = DataContext;
        }

        if (RightWindowCommands is not null)
        {
            RightWindowCommands.DataContext = DataContext;
        }

        if (WindowButtonCommands is not null)
        {
            WindowButtonCommands.DataContext = DataContext;
        }
    }

    private void OnSizeChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (TitleAlignment != HorizontalAlignment.Center ||
            titleBar is null)
        {
            return;
        }

        var halfDistance = ActualWidth / 2;
        var margin = (Thickness)titleBar.GetValue(MarginProperty);
        var distanceToCenter = (titleBar.DesiredSize.Width - margin.Left - margin.Right) / 2;

        var localIconWidth = icon?.ActualWidth ?? 0;
        var leftWindowCommandsWidth = LeftWindowCommands?.ActualWidth ?? 0;
        var rightWindowCommandsWidth = RightWindowCommands?.ActualWidth ?? 0;
        var windowButtonCommandsWith = WindowButtonCommands?.ActualWidth ?? 0;

        var distanceFromLeft = localIconWidth + leftWindowCommandsWidth;
        var distanceFromRight = rightWindowCommandsWidth + windowButtonCommandsWith;
        const double horizontalMargin = 5.0;

        var dLeft = distanceFromLeft + distanceToCenter + horizontalMargin;
        var dRight = distanceFromRight + distanceToCenter + horizontalMargin;
        if (dLeft < halfDistance && dRight < halfDistance)
        {
            titleBar.SetCurrentValue(MarginProperty, default(Thickness));
            Grid.SetColumn(titleBar, 0);
            Grid.SetColumnSpan(titleBar, 5);
        }
        else
        {
            titleBar.SetCurrentValue(MarginProperty, new Thickness(leftWindowCommandsWidth, 0, rightWindowCommandsWidth, 0));
            Grid.SetColumn(titleBar, 2);
            Grid.SetColumnSpan(titleBar, 1);
        }
    }

    private void HandleThemeManagerThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        this.Invoke(this.ResetAllWindowCommandsBrush);
    }

    private static void UpdateLogicalChildren(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not NiceWindow window)
        {
            return;
        }

        if (e.OldValue is FrameworkElement oldChild)
        {
            window.RemoveLogicalChild(oldChild);
        }

        if (e.NewValue is FrameworkElement newChild)
        {
            window.AddLogicalChild(newChild);
            newChild.DataContext = window.DataContext;
        }
    }

    private void ClearWindowEvents()
    {
        if (windowTitleThumb is not null)
        {
            windowTitleThumb.PreviewMouseLeftButtonUp -= WindowTitleThumbOnPreviewMouseLeftButtonUp;
            windowTitleThumb.DragDelta -= WindowTitleThumbMoveOnDragDelta;
            windowTitleThumb.MouseDoubleClick -= WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
            windowTitleThumb.MouseRightButtonUp -= WindowTitleThumbSystemMenuOnMouseRightButtonUp;
        }

        if (titleBar is INiceThumb thumbContentControl)
        {
            thumbContentControl.PreviewMouseLeftButtonUp -= WindowTitleThumbOnPreviewMouseLeftButtonUp;
            thumbContentControl.DragDelta -= WindowTitleThumbMoveOnDragDelta;
            thumbContentControl.MouseDoubleClick -= WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
            thumbContentControl.MouseRightButtonUp -= WindowTitleThumbSystemMenuOnMouseRightButtonUp;
        }

        if (icon is not null)
        {
            icon.MouseDown -= IconMouseDown;
        }

        SizeChanged -= OnSizeChanged;
    }

    private void SetWindowEvents()
    {
        ClearWindowEvents();

        if (icon is not null && icon.Visibility == Visibility.Visible)
        {
            icon.MouseDown += IconMouseDown;
        }

        if (windowTitleThumb is not null)
        {
            windowTitleThumb.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
            windowTitleThumb.DragDelta += WindowTitleThumbMoveOnDragDelta;
            windowTitleThumb.MouseDoubleClick += WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
            windowTitleThumb.MouseRightButtonUp += WindowTitleThumbSystemMenuOnMouseRightButtonUp;
        }

        if (titleBar is INiceThumb thumbContentControl)
        {
            thumbContentControl.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
            thumbContentControl.DragDelta += WindowTitleThumbMoveOnDragDelta;
            thumbContentControl.MouseDoubleClick += WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
            thumbContentControl.MouseRightButtonUp += WindowTitleThumbSystemMenuOnMouseRightButtonUp;
        }

        if (titleBar is not null && TitleAlignment == HorizontalAlignment.Center)
        {
            SizeChanged += OnSizeChanged;
        }
    }

    private void IconMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        if (e.ClickCount == 2)
        {
            Close();
        }
        else if (ShowSystemMenu)
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(BorderThickness.Left, TitleBarHeight + BorderThickness.Top)));
#pragma warning restore 618
        }
    }

    private void WindowTitleThumbOnPreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
        => DoWindowTitleThumbOnPreviewMouseLeftButtonUp(this, e);

    private void WindowTitleThumbMoveOnDragDelta(
        object sender,
        DragDeltaEventArgs e)
        => DoWindowTitleThumbMoveOnDragDelta(sender as INiceThumb, this, e);

    private void WindowTitleThumbChangeWindowStateOnMouseDoubleClick(
        object sender,
        MouseButtonEventArgs e)
        => DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(this, e);

    private void WindowTitleThumbSystemMenuOnMouseRightButtonUp(
        object sender,
        MouseButtonEventArgs e)
        => DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(this, e);
}