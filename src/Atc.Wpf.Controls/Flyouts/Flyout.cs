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
[TemplatePart(Name = PartContentPresenter, Type = typeof(ContentPresenter))]
public sealed partial class Flyout : ContentControl
{
    private const string PartOverlay = "PART_Overlay";
    private const string PartFlyoutPanel = "PART_FlyoutPanel";
    private const string PartCloseButton = "PART_CloseButton";
    private const string PartContentPresenter = "PART_ContentPresenter";

    private Border? overlayBorder;
    private Border? flyoutPanel;
    private Button? closeButton;
    private TranslateTransform? slideTransform;

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
        _ = GetTemplateChild(PartContentPresenter) as ContentPresenter;

        if (flyoutPanel?.RenderTransform is TranslateTransform transform)
        {
            slideTransform = transform;
        }
        else if (flyoutPanel is not null)
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
        if (e.Key == Key.Escape && CloseOnEscape && IsOpen && !isAnimating)
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
    }

    private void OnCloseButtonClick(
        object sender,
        RoutedEventArgs e)
        => _ = CloseAsync();

    private void OnOverlayMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (IsLightDismissEnabled)
        {
            CloseWithLightDismiss();
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
        if (flyoutPanel is null || slideTransform is null)
        {
            Visibility = Visibility.Visible;
            RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            return;
        }

        isAnimating = true;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        // Set initial position based on flyout position
        var startValue = GetSlideStartValue();
        var property = GetSlideProperty();

        if (property == TranslateTransform.XProperty)
        {
            slideTransform.X = startValue;
        }
        else
        {
            slideTransform.Y = startValue;
        }

        // Animate to 0
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

            // Focus the flyout
            _ = Focus();
        };

        // Animate overlay opacity
        if (overlayBorder is not null && ShowOverlay)
        {
            overlayBorder.Visibility = Visibility.Visible;
            var overlayAnimation = new DoubleAnimation
            {
                From = 0,
                To = OverlayOpacity,
                Duration = duration,
            };
            overlayBorder.BeginAnimation(OpacityProperty, overlayAnimation);
        }

        slideTransform.BeginAnimation(property, slideAnimation);
    }

    private void AnimateClose()
    {
        if (flyoutPanel is null || slideTransform is null)
        {
            Visibility = Visibility.Collapsed;
            RestoreFocus();
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
            return;
        }

        isAnimating = true;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        var easing = new CubicEase { EasingMode = EasingMode.EaseIn };

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

        // Animate overlay opacity
        if (overlayBorder is not null && ShowOverlay)
        {
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

        slideTransform.BeginAnimation(property, slideAnimation);
    }

    private double GetSlideStartValue()
        => Position switch
        {
            FlyoutPosition.Right => FlyoutWidth,
            FlyoutPosition.Left => -FlyoutWidth,
            FlyoutPosition.Bottom => FlyoutHeight,
            FlyoutPosition.Top => -FlyoutHeight,
            _ => FlyoutWidth,
        };

    private DependencyProperty GetSlideProperty()
        => Position switch
        {
            FlyoutPosition.Right or FlyoutPosition.Left => TranslateTransform.XProperty,
            FlyoutPosition.Top or FlyoutPosition.Bottom => TranslateTransform.YProperty,
            _ => TranslateTransform.XProperty,
        };

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
            Visibility = Visibility.Collapsed;

            if (overlayBorder is not null)
            {
                overlayBorder.Visibility = Visibility.Collapsed;
            }

            if (slideTransform is not null)
            {
                var startValue = GetSlideStartValue();
                var property = GetSlideProperty();

                if (property == TranslateTransform.XProperty)
                {
                    slideTransform.X = startValue;
                }
                else
                {
                    slideTransform.Y = startValue;
                }
            }
        }
        else
        {
            Visibility = Visibility.Visible;

            if (overlayBorder is not null && ShowOverlay)
            {
                overlayBorder.Visibility = Visibility.Visible;
                overlayBorder.Opacity = OverlayOpacity;
            }

            if (slideTransform is not null)
            {
                var property = GetSlideProperty();

                if (property == TranslateTransform.XProperty)
                {
                    slideTransform.X = 0;
                }
                else
                {
                    slideTransform.Y = 0;
                }
            }
        }
    }
}