namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control that displays popup content positioned relative to an anchor element,
/// with configurable placement, trigger modes, and optional arrow pointer.
/// </summary>
[ContentProperty(nameof(PopoverContent))]
[TemplatePart(Name = PartPopup, Type = typeof(Popup))]
[TemplatePart(Name = PartAnchor, Type = typeof(ContentPresenter))]
public sealed partial class Popover : Control
{
    private const string PartPopup = "PART_Popup";
    private const string PartAnchor = "PART_Anchor";

    /// <summary>
    /// The anchor element displayed in the control.
    /// </summary>
    [DependencyProperty]
    private object? anchor;

    /// <summary>
    /// Optional template for the anchor content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? anchorTemplate;

    /// <summary>
    /// The content displayed inside the popover popup.
    /// </summary>
    [DependencyProperty]
    private object? popoverContent;

    /// <summary>
    /// Optional template for the popover content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? popoverContentTemplate;

    /// <summary>
    /// Whether the popover is currently open.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnIsOpenChanged))]
    private bool isOpen;

    /// <summary>
    /// Position of the popover relative to the anchor.
    /// </summary>
    [DependencyProperty(DefaultValue = default(PopoverPlacement))]
    private PopoverPlacement placement;

    /// <summary>
    /// How the popover is triggered to open.
    /// </summary>
    [DependencyProperty(
        DefaultValue = default(PopoverTriggerMode),
        PropertyChangedCallback = nameof(OnTriggerModeChanged))]
    private PopoverTriggerMode triggerMode;

    /// <summary>
    /// Whether to show the arrow pointer on the popover.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showArrow;

    /// <summary>
    /// Delay in milliseconds before opening the popover (for Hover trigger).
    /// </summary>
    [DependencyProperty(DefaultValue = 0)]
    private int openDelay;

    /// <summary>
    /// Delay in milliseconds before closing the popover (for Hover trigger).
    /// </summary>
    [DependencyProperty(DefaultValue = 200)]
    private int closeDelay;

    /// <summary>
    /// Maximum width of the popover content area.
    /// </summary>
    [DependencyProperty]
    private double maxPopoverWidth;

    /// <summary>
    /// Maximum height of the popover content area.
    /// </summary>
    [DependencyProperty]
    private double maxPopoverHeight;

    /// <summary>
    /// Corner radius of the popover border.
    /// </summary>
    [DependencyProperty(DefaultValue = "new CornerRadius(4)")]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Background brush for the popover.
    /// </summary>
    [DependencyProperty]
    private Brush? popoverBackground;

    /// <summary>
    /// Border brush for the popover.
    /// </summary>
    [DependencyProperty]
    private Brush? popoverBorderBrush;

    /// <summary>
    /// Border thickness of the popover.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(1)")]
    private Thickness popoverBorderThickness;

    /// <summary>
    /// Inner padding of the popover content.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(8)")]
    private Thickness popoverPadding;

    private Popup? popup;
    private ContentPresenter? anchorPresenter;
    private DispatcherTimer? openTimer;
    private DispatcherTimer? closeTimer;
    private Window? dismissWindow;

    /// <summary>
    /// Occurs when the popover is opened.
    /// </summary>
    public event RoutedEventHandler? Opened;

    /// <summary>
    /// Occurs when the popover is closed.
    /// </summary>
    public event RoutedEventHandler? Closed;

    static Popover()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Popover),
            new FrameworkPropertyMetadata(typeof(Popover)));
    }

    public Popover()
    {
        MaxPopoverWidth = double.PositiveInfinity;
        MaxPopoverHeight = double.PositiveInfinity;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UnwireEvents();

        anchorPresenter = GetTemplateChild(PartAnchor) as ContentPresenter;
        popup = GetTemplateChild(PartPopup) as Popup;

        if (popup is not null)
        {
            popup.Closed += OnPopupClosed;
            popup.CustomPopupPlacementCallback = GetPopupPlacement;
        }

        WireEvents();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (e.Key == Key.Escape && IsOpen)
        {
            SetCurrentValue(IsOpenProperty, false);
            e.Handled = true;
        }
    }

    private void WireEvents()
    {
        if (anchorPresenter is null)
        {
            return;
        }

        switch (TriggerMode)
        {
            case PopoverTriggerMode.Click:
                anchorPresenter.AddHandler(
                    MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(OnAnchorClick),
                    handledEventsToo: true);
                break;
            case PopoverTriggerMode.Hover:
                anchorPresenter.MouseEnter += OnAnchorMouseEnter;
                anchorPresenter.MouseLeave += OnAnchorMouseLeave;
                if (popup is not null)
                {
                    popup.MouseEnter += OnPopupMouseEnter;
                    popup.MouseLeave += OnPopupMouseLeave;
                }

                break;
            case PopoverTriggerMode.Manual:
            default:
                break;
        }
    }

    private void UnwireEvents()
    {
        if (anchorPresenter is not null)
        {
            anchorPresenter.RemoveHandler(
                MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(OnAnchorClick));
            anchorPresenter.MouseEnter -= OnAnchorMouseEnter;
            anchorPresenter.MouseLeave -= OnAnchorMouseLeave;
        }

        if (popup is not null)
        {
            popup.Closed -= OnPopupClosed;
            popup.MouseEnter -= OnPopupMouseEnter;
            popup.MouseLeave -= OnPopupMouseLeave;
        }

        StopTimers();
    }

    private void OnAnchorClick(
        object sender,
        MouseButtonEventArgs e)
    {
        SetCurrentValue(IsOpenProperty, !IsOpen);
    }

    private void OnAnchorMouseEnter(
        object sender,
        MouseEventArgs e)
    {
        StopCloseTimer();
        StartOpenTimer();
    }

    private void OnAnchorMouseLeave(
        object sender,
        MouseEventArgs e)
    {
        StopOpenTimer();
        StartCloseTimer();
    }

    private void OnPopupMouseEnter(
        object sender,
        MouseEventArgs e)
    {
        StopCloseTimer();
    }

    private void OnPopupMouseLeave(
        object sender,
        MouseEventArgs e)
    {
        StartCloseTimer();
    }

    private void OnPopupClosed(
        object? sender,
        EventArgs e)
    {
        if (IsOpen)
        {
            SetCurrentValue(IsOpenProperty, false);
        }
    }

    private void StartOpenTimer()
    {
        if (OpenDelay <= 0)
        {
            SetCurrentValue(IsOpenProperty, true);
            return;
        }

        openTimer ??= new DispatcherTimer();
        openTimer.Interval = TimeSpan.FromMilliseconds(OpenDelay);
        openTimer.Tick += OnOpenTimerTick;
        openTimer.Start();
    }

    private void StopOpenTimer()
    {
        if (openTimer is null)
        {
            return;
        }

        openTimer.Stop();
        openTimer.Tick -= OnOpenTimerTick;
    }

    private void OnOpenTimerTick(
        object? sender,
        EventArgs e)
    {
        StopOpenTimer();
        SetCurrentValue(IsOpenProperty, true);
    }

    private void StartCloseTimer()
    {
        if (CloseDelay <= 0)
        {
            SetCurrentValue(IsOpenProperty, false);
            return;
        }

        closeTimer ??= new DispatcherTimer();
        closeTimer.Interval = TimeSpan.FromMilliseconds(CloseDelay);
        closeTimer.Tick += OnCloseTimerTick;
        closeTimer.Start();
    }

    private void StopCloseTimer()
    {
        if (closeTimer is null)
        {
            return;
        }

        closeTimer.Stop();
        closeTimer.Tick -= OnCloseTimerTick;
    }

    private void OnCloseTimerTick(
        object? sender,
        EventArgs e)
    {
        StopCloseTimer();
        SetCurrentValue(IsOpenProperty, false);
    }

    private void StopTimers()
    {
        StopOpenTimer();
        StopCloseTimer();
    }

    private CustomPopupPlacement[] GetPopupPlacement(
        Size popupSize,
        Size targetSize,
        Point offset)
    {
        var primary = CalculatePlacement(
            Placement,
            popupSize,
            targetSize);
        var fallback = CalculatePlacement(
            GetFlippedPlacement(Placement),
            popupSize,
            targetSize);

        return
        [
            new CustomPopupPlacement(primary, PopupPrimaryAxis.None),
            new CustomPopupPlacement(fallback, PopupPrimaryAxis.None),
        ];
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Switch expression for 12 placement calculations.")]
    private static Point CalculatePlacement(
        PopoverPlacement placement,
        Size popupSize,
        Size targetSize)
    {
        const double gap = 4;

        return placement switch
        {
            PopoverPlacement.Top => new Point(
                (targetSize.Width - popupSize.Width) / 2,
                -popupSize.Height - gap),
            PopoverPlacement.TopStart => new Point(
                0,
                -popupSize.Height - gap),
            PopoverPlacement.TopEnd => new Point(
                targetSize.Width - popupSize.Width,
                -popupSize.Height - gap),

            PopoverPlacement.Bottom => new Point(
                (targetSize.Width - popupSize.Width) / 2,
                targetSize.Height + gap),
            PopoverPlacement.BottomStart => new Point(
                0,
                targetSize.Height + gap),
            PopoverPlacement.BottomEnd => new Point(
                targetSize.Width - popupSize.Width,
                targetSize.Height + gap),

            PopoverPlacement.Left => new Point(
                -popupSize.Width - gap,
                (targetSize.Height - popupSize.Height) / 2),
            PopoverPlacement.LeftStart => new Point(
                -popupSize.Width - gap,
                0),
            PopoverPlacement.LeftEnd => new Point(
                -popupSize.Width - gap,
                targetSize.Height - popupSize.Height),

            PopoverPlacement.Right => new Point(
                targetSize.Width + gap,
                (targetSize.Height - popupSize.Height) / 2),
            PopoverPlacement.RightStart => new Point(
                targetSize.Width + gap,
                0),
            PopoverPlacement.RightEnd => new Point(
                targetSize.Width + gap,
                targetSize.Height - popupSize.Height),

            _ => new Point(0, targetSize.Height + gap),
        };
    }

    private static PopoverPlacement GetFlippedPlacement(
        PopoverPlacement placement)
        => placement switch
        {
            PopoverPlacement.Top => PopoverPlacement.Bottom,
            PopoverPlacement.TopStart => PopoverPlacement.BottomStart,
            PopoverPlacement.TopEnd => PopoverPlacement.BottomEnd,
            PopoverPlacement.Bottom => PopoverPlacement.Top,
            PopoverPlacement.BottomStart => PopoverPlacement.TopStart,
            PopoverPlacement.BottomEnd => PopoverPlacement.TopEnd,
            PopoverPlacement.Left => PopoverPlacement.Right,
            PopoverPlacement.LeftStart => PopoverPlacement.RightStart,
            PopoverPlacement.LeftEnd => PopoverPlacement.RightEnd,
            PopoverPlacement.Right => PopoverPlacement.Left,
            PopoverPlacement.RightStart => PopoverPlacement.LeftStart,
            PopoverPlacement.RightEnd => PopoverPlacement.LeftEnd,
            _ => PopoverPlacement.Top,
        };

    private static void OnIsOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Popover popover || e.NewValue is not bool newIsOpen)
        {
            return;
        }

        if (newIsOpen)
        {
            popover.OnOpened();
        }
        else
        {
            popover.OnClosed();
        }
    }

    private static void OnTriggerModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Popover popover)
        {
            return;
        }

        popover.UnwireEvents();
        popover.WireEvents();
    }

    private void OnOpened()
    {
        if (TriggerMode == PopoverTriggerMode.Click)
        {
            dismissWindow = Window.GetWindow(this);
            dismissWindow?.AddHandler(
                UIElement.PreviewMouseDownEvent,
                new MouseButtonEventHandler(OnWindowPreviewMouseDown),
                handledEventsToo: true);
        }

        Opened?.Invoke(this, new RoutedEventArgs());
    }

    private void OnClosed()
    {
        if (dismissWindow is not null)
        {
            dismissWindow.RemoveHandler(
                UIElement.PreviewMouseDownEvent,
                new MouseButtonEventHandler(OnWindowPreviewMouseDown));
            dismissWindow = null;
        }

        Closed?.Invoke(this, new RoutedEventArgs());
    }

    private void OnWindowPreviewMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!IsOpen)
        {
            return;
        }

        if (popup?.Child is UIElement child && child.IsMouseOver)
        {
            return;
        }

        if (anchorPresenter?.IsMouseOver == true)
        {
            return;
        }

        SetCurrentValue(IsOpenProperty, false);
    }
}