// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable NotAccessedField.Global
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
public class NiceWindow : WindowChromeWindow
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

    public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register(
        nameof(ShowIconOnTitleBar),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(
            BooleanBoxes.TrueBox,
            OnShowIconOnTitleBarPropertyChangedCallback));

    private static void OnShowIconOnTitleBarPropertyChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var window = (NiceWindow)d;
        if (e.NewValue != e.OldValue)
        {
            window.UpdateIconVisibility();
        }
    }

    /// <summary>
    /// Get or sets whether the TitleBar icon is visible or not.
    /// </summary>
    public bool ShowIconOnTitleBar
    {
        get => (bool)GetValue(ShowIconOnTitleBarProperty);
        set => SetValue(ShowIconOnTitleBarProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
        nameof(IconWidth),
        typeof(double),
        typeof(NiceWindow),
        new PropertyMetadata(20.0));

    /// <summary>
    /// Gets or sets the width for the icon.
    /// </summary>
    public double IconWidth
    {
        get => (double)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
        nameof(IconHeight),
        typeof(double),
        typeof(NiceWindow),
        new PropertyMetadata(20.0));

    /// <summary>
    /// Gets or sets the height for the icon.
    /// </summary>
    public double IconHeight
    {
        get => (double)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register(
        nameof(IconMargin),
        typeof(Thickness),
        typeof(NiceWindow),
        new PropertyMetadata(new Thickness(10, 3, 10, 3)));

    /// <summary>
    /// Gets or sets the margin for the icon.
    /// </summary>
    public Thickness IconMargin
    {
        get => (Thickness)GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register(
        nameof(IconEdgeMode),
        typeof(EdgeMode),
        typeof(NiceWindow),
        new PropertyMetadata(EdgeMode.Aliased));

    /// <summary>
    /// Gets or sets the edge mode for the TitleBar icon.
    /// </summary>
    public EdgeMode IconEdgeMode
    {
        get => (EdgeMode)GetValue(IconEdgeModeProperty);
        set => SetValue(IconEdgeModeProperty, value);
    }

    public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register(
        nameof(IconBitmapScalingMode),
        typeof(BitmapScalingMode),
        typeof(NiceWindow),
        new PropertyMetadata(BitmapScalingMode.HighQuality));

    /// <summary>
    /// Gets or sets the bitmap scaling mode for the TitleBar icon.
    /// </summary>
    public BitmapScalingMode IconBitmapScalingMode
    {
        get => (BitmapScalingMode)GetValue(IconBitmapScalingModeProperty);
        set => SetValue(IconBitmapScalingModeProperty, value);
    }

    public static readonly DependencyProperty IconScalingModeProperty = DependencyProperty.Register(
        nameof(IconScalingMode),
        typeof(MultiFrameImageMode),
        typeof(NiceWindow),
        new FrameworkPropertyMetadata(
            MultiFrameImageMode.ScaleDownLargerFrame,
            FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the scaling mode for the TitleBar icon.
    /// </summary>
    public MultiFrameImageMode IconScalingMode
    {
        get => (MultiFrameImageMode)GetValue(IconScalingModeProperty);
        set => SetValue(IconScalingModeProperty, value);
    }

    public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(
        nameof(ShowTitleBar),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(
            BooleanBoxes.TrueBox,
            OnShowTitleBarPropertyChangedCallback,
            OnShowTitleBarCoerceValueCallback));

    [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "OK.")]
    private static object? OnShowTitleBarCoerceValueCallback(
        DependencyObject d,
        object? value)
    {
        // if UseNoneWindowStyle = true no title bar should be shown
        return ((NiceWindow)d).UseNoneWindowStyle
            ? false
            : value;
    }

    /// <summary>
    /// Gets or sets whether the TitleBar is visible or not.
    /// </summary>
    public bool ShowTitleBar
    {
        get => (bool)GetValue(ShowTitleBarProperty);
        set => SetValue(ShowTitleBarProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowDialogsOverTitleBarProperty = DependencyProperty.Register(
        nameof(ShowDialogsOverTitleBar),
        typeof(bool),
        typeof(NiceWindow),
        new FrameworkPropertyMetadata(
            BooleanBoxes.TrueBox,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Get or sets whether a dialog will be shown over the TitleBar.
    /// </summary>
    public bool ShowDialogsOverTitleBar
    {
        get => (bool)GetValue(ShowDialogsOverTitleBarProperty);
        set => SetValue(ShowDialogsOverTitleBarProperty, BooleanBoxes.Box(value));
    }

    internal static readonly DependencyPropertyKey IsAnyDialogOpenPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsAnyDialogOpen),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty IsAnyDialogOpenProperty = IsAnyDialogOpenPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets whether that there are one or more dialogs open.
    /// </summary>
    public bool IsAnyDialogOpen
    {
        get => (bool)GetValue(IsAnyDialogOpenProperty);
        protected set => SetValue(IsAnyDialogOpenPropertyKey, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(
        nameof(ShowCloseButton),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether if the close button is visible.
    /// </summary>
    public bool ShowCloseButton
    {
        get => (bool)GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IsMinButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsMinButtonEnabled),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets if the minimize button is enabled.
    /// </summary>
    public bool IsMinButtonEnabled
    {
        get => (bool)GetValue(IsMinButtonEnabledProperty);
        set => SetValue(IsMinButtonEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IsMaxRestoreButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsMaxRestoreButtonEnabled),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets if the maximize/restore button is enabled.
    /// </summary>
    public bool IsMaxRestoreButtonEnabled
    {
        get => (bool)GetValue(IsMaxRestoreButtonEnabledProperty);
        set => SetValue(IsMaxRestoreButtonEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsCloseButtonEnabled),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets if the close button is enabled.
    /// </summary>
    public bool IsCloseButtonEnabled
    {
        get => (bool)GetValue(IsCloseButtonEnabledProperty);
        set => SetValue(IsCloseButtonEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowSystemMenuProperty = DependencyProperty.Register(
        nameof(ShowSystemMenu),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the system menu should popup with left mouse click on the window icon.
    /// </summary>
    public bool ShowSystemMenu
    {
        get => (bool)GetValue(ShowSystemMenuProperty);
        set => SetValue(ShowSystemMenuProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register(
        nameof(ShowSystemMenuOnRightClick),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the system menu should popup with right mouse click if the mouse position is on title bar or on the entire window if it has no TitleBar (and no TitleBar height).
    /// </summary>
    public bool ShowSystemMenuOnRightClick
    {
        get => (bool)GetValue(ShowSystemMenuOnRightClickProperty);
        set => SetValue(ShowSystemMenuOnRightClickProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(
        nameof(TitleBarHeight),
        typeof(int),
        typeof(NiceWindow),
        new PropertyMetadata(
            30,
            TitleBarHeightPropertyChangedCallback));

    private static void TitleBarHeightPropertyChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).UpdateTitleBarElementsVisibility();
    }

    /// <summary>
    /// Gets or sets the TitleBar's height.
    /// </summary>
    public int TitleBarHeight
    {
        get => (int)GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    public static readonly DependencyProperty TitleCharacterCasingProperty = DependencyProperty.Register(
        nameof(TitleCharacterCasing),
        typeof(CharacterCasing),
        typeof(NiceWindow),
        new FrameworkPropertyMetadata(
            CharacterCasing.Normal,
            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    /// <summary>
    /// Gets or sets the Character casing of the title.
    /// </summary>
    public CharacterCasing TitleCharacterCasing
    {
        get => (CharacterCasing)GetValue(TitleCharacterCasingProperty);
        set => SetValue(TitleCharacterCasingProperty, value);
    }

    public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register(
        nameof(TitleAlignment),
        typeof(HorizontalAlignment),
        typeof(NiceWindow),
        new PropertyMetadata(
            HorizontalAlignment.Stretch,
            OnTitleAlignmentChanged));

    private static void OnTitleAlignmentChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        var window = (NiceWindow)dependencyObject;

        window.SizeChanged -= window.OnSizeChanged;
        if (e.NewValue is HorizontalAlignment.Center &&
            window.titleBar is not null)
        {
            window.SizeChanged += window.OnSizeChanged;
        }
    }

    /// <summary>
    /// Gets or sets the horizontal alignment of the title.
    /// </summary>
    public HorizontalAlignment TitleAlignment
    {
        get => (HorizontalAlignment)GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, value);
    }

    public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register(
        nameof(SaveWindowPosition),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets whether the window will save it's position and size.
    /// </summary>
    public bool SaveWindowPosition
    {
        get => (bool)GetValue(SaveWindowPositionProperty);
        set => SetValue(SaveWindowPositionProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register(
        nameof(WindowPlacementSettings),
        typeof(IWindowPlacementSettings),
        typeof(NiceWindow),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    ///  Gets or sets the settings to save and load the position and size of the window.
    /// </summary>
    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public IWindowPlacementSettings? WindowPlacementSettings
    {
        get => (IWindowPlacementSettings?)GetValue(WindowPlacementSettingsProperty);
        set => SetValue(WindowPlacementSettingsProperty, value);
    }

    public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(
        nameof(TitleForeground),
        typeof(Brush),
        typeof(NiceWindow));

    /// <summary>
    /// Gets or sets the brush used for the TitleBar's foreground.
    /// </summary>
    public Brush? TitleForeground
    {
        get => (Brush?)GetValue(TitleForegroundProperty);
        set => SetValue(TitleForegroundProperty, value);
    }

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate),
        typeof(DataTemplate),
        typeof(NiceWindow),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the <see cref="Window.Title"/>.
    /// </summary>
    public DataTemplate? TitleTemplate
    {
        get => (DataTemplate?)GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }

    public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register(
        nameof(WindowTitleBrush),
        typeof(Brush),
        typeof(NiceWindow),
        new PropertyMetadata(Brushes.Transparent));

    /// <summary>
    /// Gets or sets the brush used for the background of the TitleBar.
    /// </summary>
    public Brush WindowTitleBrush
    {
        get => (Brush)GetValue(WindowTitleBrushProperty);
        set => SetValue(WindowTitleBrushProperty, value);
    }

    public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register(
        nameof(NonActiveWindowTitleBrush),
        typeof(Brush),
        typeof(NiceWindow),
        new PropertyMetadata(Brushes.Gray));

    /// <summary>
    /// Gets or sets the non-active brush used for the background of the TitleBar.
    /// </summary>
    public Brush NonActiveWindowTitleBrush
    {
        get => (Brush)GetValue(NonActiveWindowTitleBrushProperty);
        set => SetValue(NonActiveWindowTitleBrushProperty, value);
    }

    public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register(
        nameof(NonActiveBorderBrush),
        typeof(Brush),
        typeof(NiceWindow),
        new PropertyMetadata(Brushes.Gray));

    /// <summary>
    /// Gets or sets the non-active brush used for the border of the window.
    /// </summary>
    public Brush NonActiveBorderBrush
    {
        get => (Brush)GetValue(NonActiveBorderBrushProperty);
        set => SetValue(NonActiveBorderBrushProperty, value);
    }

    public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register(
        nameof(OverlayBrush),
        typeof(Brush),
        typeof(NiceWindow),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the brush used for the overlay when a dialog is open.
    /// </summary>
    public Brush? OverlayBrush
    {
        get => (Brush?)GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register(
        nameof(OverlayOpacity),
        typeof(double),
        typeof(NiceWindow),
        new PropertyMetadata(0.7d));

    /// <summary>
    /// Gets or sets the opacity used for the overlay when a dialog is open.
    /// </summary>
    public double OverlayOpacity
    {
        get => (double)GetValue(OverlayOpacityProperty);
        set => SetValue(OverlayOpacityProperty, value);
    }

    public static readonly DependencyProperty OverlayFadeInProperty = DependencyProperty.Register(
        nameof(OverlayFadeIn),
        typeof(Storyboard),
        typeof(NiceWindow),
        new PropertyMetadata(default(Storyboard)));

    /// <summary>
    /// Gets or sets the storyboard for the overlay fade in effect.
    /// </summary>
    public Storyboard? OverlayFadeIn
    {
        get => (Storyboard?)GetValue(OverlayFadeInProperty);
        set => SetValue(OverlayFadeInProperty, value);
    }

    public static readonly DependencyProperty OverlayFadeOutProperty = DependencyProperty.Register(
        nameof(OverlayFadeOut),
        typeof(Storyboard),
        typeof(NiceWindow),
        new PropertyMetadata(default(Storyboard)));

    /// <summary>
    /// Gets or sets the storyboard for the overlay fade out effect.
    /// </summary>
    public Storyboard? OverlayFadeOut
    {
        get => (Storyboard?)GetValue(OverlayFadeOutProperty);
        set => SetValue(OverlayFadeOutProperty, value);
    }

    public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register(
        nameof(WindowTransitionsEnabled),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets whether the start animation of the window content is available.
    /// </summary>
    public bool WindowTransitionsEnabled
    {
        get => (bool)GetValue(WindowTransitionsEnabledProperty);
        set => SetValue(WindowTransitionsEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
        nameof(IconTemplate),
        typeof(DataTemplate),
        typeof(NiceWindow),
        new PropertyMetadata(
            defaultValue: null,
            (o, e) =>
            {
                if (e.NewValue != e.OldValue)
                {
                    (o as NiceWindow)?.UpdateIconVisibility();
                }
            }));

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the icon on the TitleBar.
    /// </summary>
    public DataTemplate? IconTemplate
    {
        get => (DataTemplate?)GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(
        nameof(LeftWindowCommands),
        typeof(WindowCommands),
        typeof(NiceWindow),
        new PropertyMetadata(
            defaultValue: null,
            OnLeftWindowCommandsPropertyChanged));

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

    /// <summary>
    /// Gets or sets the <see cref="WindowCommands"/> host on the left side of the TitleBar.
    /// </summary>
    public WindowCommands? LeftWindowCommands
    {
        get => (WindowCommands?)GetValue(LeftWindowCommandsProperty);
        set => SetValue(LeftWindowCommandsProperty, value);
    }

    public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(
        nameof(RightWindowCommands),
        typeof(WindowCommands),
        typeof(NiceWindow),
        new PropertyMetadata(
            defaultValue: null,
            OnRightWindowCommandsPropertyChanged));

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

    /// <summary>
    /// Gets or sets the <see cref="WindowCommands"/> host on the right side of the TitleBar.
    /// </summary>
    public WindowCommands? RightWindowCommands
    {
        get => (WindowCommands?)GetValue(RightWindowCommandsProperty);
        set => SetValue(RightWindowCommandsProperty, value);
    }

    public static readonly DependencyProperty WindowButtonCommandsProperty = DependencyProperty.Register(
        nameof(WindowButtonCommands),
        typeof(WindowButtonCommands),
        typeof(NiceWindow),
        new PropertyMetadata(
            defaultValue: null,
            UpdateLogicalChildren));

    /// <summary>
    /// Gets or sets the <see cref="WindowButtonCommands"/> host that shows the minimize/maximize/restore/close buttons.
    /// </summary>
    public WindowButtonCommands? WindowButtonCommands
    {
        get => (WindowButtonCommands?)GetValue(WindowButtonCommandsProperty);
        set => SetValue(WindowButtonCommandsProperty, value);
    }

    public static readonly DependencyProperty LeftWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register(
        nameof(LeftWindowCommandsOverlayBehavior),
        typeof(WindowCommandsOverlayBehaviorType),
        typeof(NiceWindow),
        new PropertyMetadata(
            WindowCommandsOverlayBehaviorType.Never,
            OnShowTitleBarPropertyChangedCallback));

    private static void OnShowTitleBarPropertyChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).UpdateTitleBarElementsVisibility();
    }

    /// <summary>
    /// Gets or sets the overlay behavior for the <see cref="WindowCommands"/> host on the left side.
    /// </summary>
    public WindowCommandsOverlayBehaviorType LeftWindowCommandsOverlayBehavior
    {
        get => (WindowCommandsOverlayBehaviorType)GetValue(LeftWindowCommandsOverlayBehaviorProperty);
        set => SetValue(LeftWindowCommandsOverlayBehaviorProperty, value);
    }

    public static readonly DependencyProperty RightWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register(
        nameof(RightWindowCommandsOverlayBehavior),
        typeof(WindowCommandsOverlayBehaviorType),
        typeof(NiceWindow),
        new PropertyMetadata(
            WindowCommandsOverlayBehaviorType.Never,
            OnShowTitleBarPropertyChangedCallback));

    /// <summary>
    /// Gets or sets the overlay behavior for the <see cref="WindowCommands"/> host on the right side.
    /// </summary>
    public WindowCommandsOverlayBehaviorType RightWindowCommandsOverlayBehavior
    {
        get => (WindowCommandsOverlayBehaviorType)GetValue(RightWindowCommandsOverlayBehaviorProperty);
        set => SetValue(RightWindowCommandsOverlayBehaviorProperty, value);
    }

    public static readonly DependencyProperty WindowButtonCommandsOverlayBehaviorProperty = DependencyProperty.Register(
        nameof(WindowButtonCommandsOverlayBehavior),
        typeof(OverlayBehavior),
        typeof(NiceWindow),
        new PropertyMetadata(
            OverlayBehavior.Always,
            OnShowTitleBarPropertyChangedCallback));

    /// <summary>
    /// Gets or sets the overlay behavior for the <see cref="WindowButtonCommands"/> host.
    /// </summary>
    public OverlayBehavior WindowButtonCommandsOverlayBehavior
    {
        get => (OverlayBehavior)GetValue(WindowButtonCommandsOverlayBehaviorProperty);
        set => SetValue(WindowButtonCommandsOverlayBehaviorProperty, value);
    }

    public static readonly DependencyProperty IconOverlayBehaviorProperty = DependencyProperty.Register(
        nameof(IconOverlayBehavior),
        typeof(OverlayBehavior),
        typeof(NiceWindow),
        new PropertyMetadata(OverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));

    /// <summary>
    /// Gets or sets the overlay behavior for the <see cref="Window.Icon"/>.
    /// </summary>
    public OverlayBehavior IconOverlayBehavior
    {
        get => (OverlayBehavior)GetValue(IconOverlayBehaviorProperty);
        set => SetValue(IconOverlayBehaviorProperty, value);
    }

    public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register(
        nameof(UseNoneWindowStyle),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(
            BooleanBoxes.FalseBox,
            OnUseNoneWindowStylePropertyChangedCallback));

    private static void OnUseNoneWindowStylePropertyChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((NiceWindow)d).CoerceValue(ShowTitleBarProperty);
    }

    /// <summary>
    /// Gets or sets whether the window will force WindowStyle to None.
    /// </summary>
    public bool UseNoneWindowStyle
    {
        get => (bool)GetValue(UseNoneWindowStyleProperty);
        set => SetValue(UseNoneWindowStyleProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty OverrideDefaultWindowCommandsBrushProperty = DependencyProperty.Register(
        nameof(OverrideDefaultWindowCommandsBrush),
        typeof(Brush),
        typeof(NiceWindow));

    /// <summary>
    /// Allows easy handling of <see cref="WindowCommands"/> brush. Theme is also applied based on this brush.
    /// </summary>
    public Brush? OverrideDefaultWindowCommandsBrush
    {
        get => (Brush?)GetValue(OverrideDefaultWindowCommandsBrushProperty);
        set => SetValue(OverrideDefaultWindowCommandsBrushProperty, value);
    }

    public static readonly DependencyProperty IsWindowDraggableProperty = DependencyProperty.Register(
        nameof(IsWindowDraggable),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether the whole window is draggable.
    /// </summary>
    public bool IsWindowDraggable
    {
        get => (bool)GetValue(IsWindowDraggableProperty);
        set => SetValue(IsWindowDraggableProperty, BooleanBoxes.Box(value));
    }

    public static readonly RoutedEvent WindowTransitionCompletedEvent = EventManager.RegisterRoutedEvent(
        nameof(WindowTransitionCompleted),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NiceWindow));

    public event RoutedEventHandler WindowTransitionCompleted
    {
        add => AddHandler(WindowTransitionCompletedEvent, value);
        remove => RemoveHandler(WindowTransitionCompletedEvent, value);
    }

    /// <summary>
    /// Gets the window placement settings (can be overwritten).
    /// </summary>
    public virtual IWindowPlacementSettings GetWindowPlacementSettings()
        => WindowPlacementSettings ?? new WindowApplicationSettings(this);

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
        this.BeginInvoke(() =>
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

        this.BeginInvoke(() =>
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

    public NiceWindow()
    {
        InitializeSettingsBehavior();

        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        ContentRendered += OnContentRendered;
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

    protected override void OnClosing(
        CancelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnClosing(e);
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

        var iconWidth = icon?.ActualWidth ?? 0;
        var leftWindowCommandsWidth = LeftWindowCommands?.ActualWidth ?? 0;
        var rightWindowCommandsWidth = RightWindowCommands?.ActualWidth ?? 0;
        var windowButtonCommandsWith = WindowButtonCommands?.ActualWidth ?? 0;

        var distanceFromLeft = iconWidth + leftWindowCommandsWidth;
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
            var value = typeof(Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this, Array.Empty<object>()) ?? IntPtr.Zero;
            return (IntPtr)value;
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
}