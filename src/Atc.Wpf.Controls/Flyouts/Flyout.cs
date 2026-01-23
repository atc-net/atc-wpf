namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// A flyout control that displays content in a sliding panel from an edge of its container.
/// Supports light dismiss behavior, animations, and nesting.
/// </summary>
/// <remarks>
/// Features:
/// <list type="bullet">
///   <item>Slide-in animation from Right, Left, Top, or Bottom</item>
///   <item>Light dismiss (click outside, Escape key)</item>
///   <item>Optional overlay background</item>
///   <item>Header with optional close button</item>
///   <item>Keyboard navigation support</item>
///   <item>Theme support (light/dark)</item>
/// </list>
/// </remarks>
[TemplatePart(Name = PartOverlay, Type = typeof(Border))]
[TemplatePart(Name = PartFlyoutPanel, Type = typeof(Border))]
[TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
[TemplatePart(Name = PartPinButton, Type = typeof(ToggleButton))]
[TemplatePart(Name = PartResizeGrip, Type = typeof(Thumb))]
[TemplatePart(Name = PartContentPresenter, Type = typeof(ContentPresenter))]
public sealed partial class Flyout : ContentControl
{
    private const string PartOverlay = "PART_Overlay";
    private const string PartFlyoutPanel = "PART_FlyoutPanel";
    private const string PartCloseButton = "PART_CloseButton";
    private const string PartPinButton = "PART_PinButton";
    private const string PartResizeGrip = "PART_ResizeGrip";
    private const string PartContentPresenter = "PART_ContentPresenter";

    private Border? overlayBorder;
    private Border? flyoutPanel;
    private Button? closeButton;
    private ToggleButton? pinButton;
    private Thumb? resizeGrip;
    private TranslateTransform? slideTransform;
    private ScaleTransform? scaleTransform;

    private bool isAnimating;
    private IInputElement? previouslyFocusedElement;
    private Window? parentWindow;

    /// <summary>
    /// Identifies the Opening routed event.
    /// </summary>
    public static readonly RoutedEvent OpeningEvent = EventManager.RegisterRoutedEvent(
        nameof(Opening),
        RoutingStrategy.Bubble,
        typeof(EventHandler<FlyoutOpeningEventArgs>),
        typeof(Flyout));

    /// <summary>
    /// Identifies the Opened routed event.
    /// </summary>
    public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent(
        nameof(Opened),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(Flyout));

    /// <summary>
    /// Identifies the Closing routed event.
    /// </summary>
    public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
        nameof(Closing),
        RoutingStrategy.Bubble,
        typeof(EventHandler<FlyoutClosingEventArgs>),
        typeof(Flyout));

    /// <summary>
    /// Identifies the Closed routed event.
    /// </summary>
    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
        nameof(Closed),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(Flyout));

    /// <summary>
    /// Gets or sets whether the flyout is currently open.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnIsOpenChanged))]
    private bool isOpen;

    /// <summary>
    /// Gets or sets the position from which the flyout slides in.
    /// </summary>
    [DependencyProperty(DefaultValue = FlyoutPosition.Right)]
    private FlyoutPosition position;

    /// <summary>
    /// Gets or sets the width of the flyout panel (for Left/Right positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 400.0)]
    private double flyoutWidth;

    /// <summary>
    /// Gets or sets the height of the flyout panel (for Top/Bottom positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 300.0)]
    private double flyoutHeight;

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    [DependencyProperty]
    private object? header;

    /// <summary>
    /// Gets or sets the header template.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? headerTemplate;

    /// <summary>
    /// Gets or sets the header background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerBackground;

    /// <summary>
    /// Gets or sets the header foreground brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerForeground;

    /// <summary>
    /// Gets or sets whether clicking outside the flyout dismisses it.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isLightDismissEnabled;

    /// <summary>
    /// Gets or sets whether the overlay background is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showOverlay;

    /// <summary>
    /// Gets or sets the overlay opacity (0.0 - 1.0).
    /// </summary>
    [DependencyProperty(DefaultValue = 0.5)]
    private double overlayOpacity;

    /// <summary>
    /// Gets or sets the overlay brush.
    /// </summary>
    [DependencyProperty]
    private Brush? overlayBrush;

    /// <summary>
    /// Gets or sets whether the close button is visible.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showCloseButton;

    /// <summary>
    /// Gets or sets the animation duration in milliseconds.
    /// </summary>
    [DependencyProperty(DefaultValue = 300.0)]
    private double animationDuration;

    /// <summary>
    /// Gets or sets whether the Escape key closes the flyout.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool closeOnEscape;

    /// <summary>
    /// Gets or sets the corner radius for the flyout panel.
    /// </summary>
    [DependencyProperty]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Gets or sets the flyout panel background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? flyoutBackground;

    /// <summary>
    /// Gets or sets the flyout panel border brush.
    /// </summary>
    [DependencyProperty]
    private Brush? flyoutBorderBrush;

    /// <summary>
    /// Gets or sets the flyout panel border thickness.
    /// </summary>
    [DependencyProperty]
    private Thickness flyoutBorderThickness;

    /// <summary>
    /// Gets or sets the result value when the flyout closes.
    /// </summary>
    [DependencyProperty]
    private object? result;

    /// <summary>
    /// Gets or sets whether the flyout is pinned (prevents light dismiss while pinned).
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isPinned;

    /// <summary>
    /// Gets or sets whether the pin button is visible.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool showPinButton;

    /// <summary>
    /// Gets or sets whether the flyout can be resized by dragging the edge.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isResizable;

    /// <summary>
    /// Gets or sets the minimum width when resizing (for Left/Right/Center positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 200.0)]
    private double minFlyoutWidth;

    /// <summary>
    /// Gets or sets the maximum width when resizing (for Left/Right/Center positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 800.0)]
    private double maxFlyoutWidth;

    /// <summary>
    /// Gets or sets the minimum height when resizing (for Top/Bottom/Center positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 150.0)]
    private double minFlyoutHeight;

    /// <summary>
    /// Gets or sets the maximum height when resizing (for Top/Bottom/Center positions).
    /// </summary>
    [DependencyProperty(DefaultValue = 600.0)]
    private double maxFlyoutHeight;

    /// <summary>
    /// Gets or sets the easing function for animations.
    /// If null, CubicEase is used.
    /// </summary>
    [DependencyProperty]
    private IEasingFunction? easingFunction;

    /// <summary>
    /// Occurs before the flyout opens. Can be cancelled.
    /// </summary>
    public event EventHandler<FlyoutOpeningEventArgs> Opening
    {
        add => AddHandler(OpeningEvent, value);
        remove => RemoveHandler(OpeningEvent, value);
    }

    /// <summary>
    /// Occurs after the flyout has fully opened.
    /// </summary>
    public event RoutedEventHandler Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    /// <summary>
    /// Occurs before the flyout closes. Can be cancelled.
    /// </summary>
    public event EventHandler<FlyoutClosingEventArgs> Closing
    {
        add => AddHandler(ClosingEvent, value);
        remove => RemoveHandler(ClosingEvent, value);
    }

    /// <summary>
    /// Occurs after the flyout has fully closed.
    /// </summary>
    public event RoutedEventHandler Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    static Flyout()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Flyout),
            new FrameworkPropertyMetadata(typeof(Flyout)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Flyout"/> class.
    /// </summary>
    public Flyout()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UnhookEvents();

        overlayBorder = GetTemplateChild(PartOverlay) as Border;
        flyoutPanel = GetTemplateChild(PartFlyoutPanel) as Border;
        closeButton = GetTemplateChild(PartCloseButton) as Button;
        pinButton = GetTemplateChild(PartPinButton) as ToggleButton;
        resizeGrip = GetTemplateChild(PartResizeGrip) as Thumb;
        _ = GetTemplateChild(PartContentPresenter) as ContentPresenter;

        if (flyoutPanel?.RenderTransform is TransformGroup transformGroup)
        {
            foreach (var transform in transformGroup.Children)
            {
                if (transform is TranslateTransform translate)
                {
                    slideTransform = translate;
                }
                else if (transform is ScaleTransform scale)
                {
                    scaleTransform = scale;
                }
            }
        }
        else if (flyoutPanel?.RenderTransform is TranslateTransform translate)
        {
            slideTransform = translate;
        }

        if (slideTransform is null && flyoutPanel is not null)
        {
            slideTransform = new TranslateTransform();
            flyoutPanel.RenderTransform = slideTransform;
        }

        HookEvents();
        UpdateVisualState();
    }

    /// <summary>
    /// Opens the flyout with animation.
    /// </summary>
    /// <returns>A task that completes when the animation finishes.</returns>
    public Task OpenAsync()
    {
        if (IsOpen || isAnimating)
        {
            return Task.CompletedTask;
        }

        IsOpen = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the flyout with animation.
    /// </summary>
    /// <returns>A task that completes when the animation finishes.</returns>
    public Task CloseAsync()
    {
        if (!IsOpen || isAnimating)
        {
            return Task.CompletedTask;
        }

        IsOpen = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the flyout with the specified result.
    /// </summary>
    /// <param name="resultValue">The result value to return.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public Task CloseWithResultAsync(object? resultValue)
    {
        Result = resultValue;
        return CloseAsync();
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (e.Handled)
        {
            return;
        }

        if (e.Key == Key.Escape && CloseOnEscape && IsOpen)
        {
            CloseWithLightDismiss();
            e.Handled = true;
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        parentWindow = Window.GetWindow(this);

        if (parentWindow is not null)
        {
            parentWindow.PreviewKeyDown += OnParentWindowPreviewKeyDown;
        }

        UpdateVisualState();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (parentWindow is not null)
        {
            parentWindow.PreviewKeyDown -= OnParentWindowPreviewKeyDown;
            parentWindow = null;
        }
    }

    private void OnParentWindowPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Escape && CloseOnEscape && IsOpen && !isAnimating && !IsPinned)
        {
            CloseWithLightDismiss();
            e.Handled = true;
        }
    }

    private void UnhookEvents()
    {
        if (closeButton is not null)
        {
            closeButton.Click -= OnCloseButtonClick;
        }

        if (overlayBorder is not null)
        {
            overlayBorder.MouseLeftButtonDown -= OnOverlayMouseLeftButtonDown;
        }

        if (pinButton is not null)
        {
            pinButton.Checked -= OnPinButtonChecked;
            pinButton.Unchecked -= OnPinButtonUnchecked;
        }

        if (resizeGrip is not null)
        {
            resizeGrip.DragDelta -= OnResizeGripDragDelta;
        }
    }

    private void HookEvents()
    {
        if (closeButton is not null)
        {
            closeButton.Click += OnCloseButtonClick;
        }

        if (overlayBorder is not null)
        {
            overlayBorder.MouseLeftButtonDown += OnOverlayMouseLeftButtonDown;
        }

        if (pinButton is not null)
        {
            pinButton.Checked += OnPinButtonChecked;
            pinButton.Unchecked += OnPinButtonUnchecked;
        }

        if (resizeGrip is not null)
        {
            resizeGrip.DragDelta += OnResizeGripDragDelta;
        }
    }

    private void OnCloseButtonClick(
        object sender,
        RoutedEventArgs e)
        => _ = CloseAsync();

    private void OnOverlayMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (IsLightDismissEnabled && !IsPinned)
        {
            CloseWithLightDismiss();
        }
    }

    private void OnPinButtonChecked(
        object sender,
        RoutedEventArgs e)
        => IsPinned = true;

    private void OnPinButtonUnchecked(
        object sender,
        RoutedEventArgs e)
        => IsPinned = false;

    private void OnResizeGripDragDelta(
        object sender,
        DragDeltaEventArgs e)
    {
        if (!IsResizable)
        {
            return;
        }

        switch (Position)
        {
            case FlyoutPosition.Right:
                var newWidthRight = FlyoutWidth - e.HorizontalChange;
                FlyoutWidth = global::System.Math.Clamp(newWidthRight, MinFlyoutWidth, MaxFlyoutWidth);
                break;

            case FlyoutPosition.Left:
                var newWidthLeft = FlyoutWidth + e.HorizontalChange;
                FlyoutWidth = global::System.Math.Clamp(newWidthLeft, MinFlyoutWidth, MaxFlyoutWidth);
                break;

            case FlyoutPosition.Top:
                var newHeightTop = FlyoutHeight + e.VerticalChange;
                FlyoutHeight = global::System.Math.Clamp(newHeightTop, MinFlyoutHeight, MaxFlyoutHeight);
                break;

            case FlyoutPosition.Bottom:
                var newHeightBottom = FlyoutHeight - e.VerticalChange;
                FlyoutHeight = global::System.Math.Clamp(newHeightBottom, MinFlyoutHeight, MaxFlyoutHeight);
                break;

            case FlyoutPosition.Center:
                var newWidthCenter = FlyoutWidth + (e.HorizontalChange * 2);
                var newHeightCenter = FlyoutHeight + (e.VerticalChange * 2);
                FlyoutWidth = global::System.Math.Clamp(newWidthCenter, MinFlyoutWidth, MaxFlyoutWidth);
                FlyoutHeight = global::System.Math.Clamp(newHeightCenter, MinFlyoutHeight, MaxFlyoutHeight);
                break;
        }
    }

    private void CloseWithLightDismiss()
    {
        var args = new FlyoutClosingEventArgs(ClosingEvent, this)
        {
            IsLightDismiss = true,
        };

        RaiseEvent(args);

        if (!args.Cancel)
        {
            IsOpen = false;
        }
    }

    private static void OnIsOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Flyout flyout)
        {
            var newIsOpen = (bool)e.NewValue;

            if (newIsOpen)
            {
                flyout.OnOpening();
            }
            else
            {
                flyout.OnClosing(isLightDismiss: false);
            }
        }
    }

    private void OnOpening()
    {
        var args = new FlyoutOpeningEventArgs(OpeningEvent, this);
        RaiseEvent(args);

        if (args.Cancel)
        {
            IsOpen = false;
            return;
        }

        // Store current focus
        previouslyFocusedElement = Keyboard.FocusedElement;

        // Show and animate
        Visibility = Visibility.Visible;
        AnimateOpen();
    }

    private void OnClosing(bool isLightDismiss)
    {
        if (!isLightDismiss)
        {
            var args = new FlyoutClosingEventArgs(ClosingEvent, this)
            {
                IsLightDismiss = false,
            };

            RaiseEvent(args);

            if (args.Cancel)
            {
                IsOpen = true;
                return;
            }
        }

        AnimateClose();
    }

    private void AnimateOpen()
    {
        if (flyoutPanel is null)
        {
            Visibility = Visibility.Visible;
            RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            return;
        }

        isAnimating = true;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        var easing = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseOut };

        AnimateOverlayOpen(duration);

        if (UseScaleAnimation)
        {
            AnimateOpenWithScale(duration, easing);
            return;
        }

        AnimateSlideOpen(duration, easing);
    }

    private void AnimateOverlayOpen(TimeSpan duration)
    {
        if (overlayBorder is null || !ShowOverlay)
        {
            return;
        }

        overlayBorder.Visibility = Visibility.Visible;
        var overlayAnimation = new DoubleAnimation
        {
            From = 0,
            To = OverlayOpacity,
            Duration = duration,
        };
        overlayBorder.BeginAnimation(OpacityProperty, overlayAnimation);
    }

    private void AnimateSlideOpen(
        TimeSpan duration,
        IEasingFunction easing)
    {
        if (slideTransform is null)
        {
            Visibility = Visibility.Visible;
            RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            return;
        }

        var startValue = GetSlideStartValue();
        var property = GetSlideProperty();

        SetSlideTransformValue(property, startValue);

        var slideAnimation = new DoubleAnimation
        {
            From = startValue,
            To = 0,
            Duration = duration,
            EasingFunction = easing,
        };

        slideAnimation.Completed += (_, _) =>
        {
            isAnimating = false;
            RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            _ = Focus();
        };

        slideTransform.BeginAnimation(property, slideAnimation);
    }

    private void SetSlideTransformValue(
        DependencyProperty property,
        double value)
    {
        if (slideTransform is null)
        {
            return;
        }

        if (property == TranslateTransform.XProperty)
        {
            slideTransform.X = value;
        }
        else
        {
            slideTransform.Y = value;
        }
    }

    private void AnimateOpenWithScale(
        TimeSpan duration,
        IEasingFunction easing)
    {
        if (scaleTransform is null)
        {
            // Ensure we have a scale transform
            var transformGroup = new TransformGroup();
            scaleTransform = new ScaleTransform(0.9, 0.9);
            slideTransform = new TranslateTransform();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(slideTransform);

            if (flyoutPanel is not null)
            {
                flyoutPanel.RenderTransform = transformGroup;
                flyoutPanel.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        scaleTransform.ScaleX = 0.9;
        scaleTransform.ScaleY = 0.9;
        flyoutPanel!.Opacity = 0;

        var scaleXAnimation = new DoubleAnimation
        {
            From = 0.9,
            To = 1,
            Duration = duration,
            EasingFunction = easing,
        };

        var scaleYAnimation = new DoubleAnimation
        {
            From = 0.9,
            To = 1,
            Duration = duration,
            EasingFunction = easing,
        };

        var opacityAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = duration,
            EasingFunction = easing,
        };

        opacityAnimation.Completed += (_, _) =>
        {
            isAnimating = false;
            RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            _ = Focus();
        };

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        flyoutPanel.BeginAnimation(OpacityProperty, opacityAnimation);
    }

    private void AnimateClose()
    {
        if (flyoutPanel is null)
        {
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
            return;
        }

        isAnimating = true;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        var easing = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseIn };

        AnimateOverlayClose(duration);

        if (UseScaleAnimation)
        {
            AnimateCloseWithScale(duration, easing);
            return;
        }

        AnimateSlideClose(duration, easing);
    }

    private void AnimateOverlayClose(TimeSpan duration)
    {
        if (overlayBorder is null || !ShowOverlay)
        {
            return;
        }

        var overlayAnimation = new DoubleAnimation
        {
            From = OverlayOpacity,
            To = 0,
            Duration = duration,
        };
        overlayAnimation.Completed += (_, _) =>
        {
            overlayBorder.Visibility = Visibility.Collapsed;
        };
        overlayBorder.BeginAnimation(OpacityProperty, overlayAnimation);
    }

    private void AnimateSlideClose(
        TimeSpan duration,
        IEasingFunction easing)
    {
        if (slideTransform is null)
        {
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
            return;
        }

        var endValue = GetSlideStartValue();
        var property = GetSlideProperty();

        var slideAnimation = new DoubleAnimation
        {
            From = 0,
            To = endValue,
            Duration = duration,
            EasingFunction = easing,
        };

        slideAnimation.Completed += (_, _) =>
        {
            isAnimating = false;
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
        };

        slideTransform.BeginAnimation(property, slideAnimation);
    }

    private void AnimateCloseWithScale(
        TimeSpan duration,
        IEasingFunction easing)
    {
        if (scaleTransform is null || flyoutPanel is null)
        {
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
            return;
        }

        var scaleXAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0.9,
            Duration = duration,
            EasingFunction = easing,
        };

        var scaleYAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0.9,
            Duration = duration,
            EasingFunction = easing,
        };

        var opacityAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = duration,
            EasingFunction = easing,
        };

        opacityAnimation.Completed += (_, _) =>
        {
            isAnimating = false;
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
        };

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        flyoutPanel.BeginAnimation(OpacityProperty, opacityAnimation);
    }

    private double GetSlideStartValue()
        => Position switch
        {
            FlyoutPosition.Right => FlyoutWidth,
            FlyoutPosition.Left => -FlyoutWidth,
            FlyoutPosition.Bottom => FlyoutHeight,
            FlyoutPosition.Top => -FlyoutHeight,
            FlyoutPosition.Center => 0,
            _ => FlyoutWidth,
        };

    private DependencyProperty GetSlideProperty()
        => Position switch
        {
            FlyoutPosition.Right or FlyoutPosition.Left => TranslateTransform.XProperty,
            FlyoutPosition.Top or FlyoutPosition.Bottom => TranslateTransform.YProperty,
            FlyoutPosition.Center => ScaleTransform.ScaleXProperty,
            _ => TranslateTransform.XProperty,
        };

    private bool UseScaleAnimation => Position == FlyoutPosition.Center;

    private void RestoreFocus()
    {
        if (previouslyFocusedElement is not null)
        {
            _ = previouslyFocusedElement.Focus();
            previouslyFocusedElement = null;
        }
    }

    private void UpdateVisualState()
    {
        if (!IsOpen)
        {
            ApplyClosedVisualState();
        }
        else
        {
            ApplyOpenVisualState();
        }
    }

    private void ApplyClosedVisualState()
    {
        Visibility = Visibility.Collapsed;

        if (overlayBorder is not null)
        {
            overlayBorder.Visibility = Visibility.Collapsed;
        }

        if (UseScaleAnimation)
        {
            ApplyClosedScaleState();
        }
        else if (slideTransform is not null)
        {
            var startValue = GetSlideStartValue();
            var property = GetSlideProperty();
            SetSlideTransformValue(property, startValue);
        }
    }

    private void ApplyClosedScaleState()
    {
        if (scaleTransform is not null)
        {
            scaleTransform.ScaleX = 0.9;
            scaleTransform.ScaleY = 0.9;
        }

        if (flyoutPanel is not null)
        {
            flyoutPanel.Opacity = 0;
        }
    }

    private void ApplyOpenVisualState()
    {
        Visibility = Visibility.Visible;

        if (overlayBorder is not null && ShowOverlay)
        {
            overlayBorder.Visibility = Visibility.Visible;
            overlayBorder.Opacity = OverlayOpacity;
        }

        if (UseScaleAnimation)
        {
            ApplyOpenScaleState();
        }
        else if (slideTransform is not null)
        {
            var property = GetSlideProperty();
            SetSlideTransformValue(property, 0);
        }
    }

    private void ApplyOpenScaleState()
    {
        if (scaleTransform is not null)
        {
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
        }

        if (flyoutPanel is not null)
        {
            flyoutPanel.Opacity = 1;
        }
    }
}