namespace Atc.Wpf.Components.Flyouts;

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
[TemplatePart(Name = PartContentPresenter, Type = typeof(ContentPresenter))]
[DefaultStyleKey]
public sealed partial class Flyout : ContentControl
{
    private const string PartContentPresenter = "PART_ContentPresenter";

    [TemplatePartField("PART_Overlay")]
    private Border? overlayBorder;

    [TemplatePartField("PART_FlyoutPanel")]
    private Border? flyoutPanel;

    [TemplatePartField("PART_CloseButton")]
    private Button? closeButton;

    [TemplatePartField("PART_PinButton")]
    private ToggleButton? pinButton;

    [TemplatePartField("PART_ResizeGrip")]
    private Thumb? resizeGrip;

    private TranslateTransform? slideTransform;
    private ScaleTransform? scaleTransform;

    private bool isAnimating;

    // Set while a light dismiss (overlay click or Escape) closes the flyout: it has already raised Closing
    // (with IsLightDismiss), so the IsOpen change must not raise it a second time.
    private bool isClosingEventRaised;

    // Set while a cancelled close puts IsOpen back to true, so that restore does not run the open sequence.
    private bool isRestoringIsOpen;
    private IInputElement? previouslyFocusedElement;
    private Window? parentWindow;

    /// <summary>
    /// Occurs before the flyout opens. Can be cancelled.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<FlyoutOpeningEventArgs>),
        GenerateRaiseMethod = true)]
    private static readonly RoutedEvent opening;

    /// <summary>
    /// Occurs after the flyout has fully opened.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble, GenerateRaiseMethod = true)]
    private static readonly RoutedEvent opened;

    /// <summary>
    /// Occurs before the flyout closes. Can be cancelled.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<FlyoutClosingEventArgs>),
        GenerateRaiseMethod = true)]
    private static readonly RoutedEvent closing;

    /// <summary>
    /// Occurs after the flyout has fully closed.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble, GenerateRaiseMethod = true)]
    private static readonly RoutedEvent closed;

    // The open flyouts on this UI thread in the order they were opened. The last open entry in a window is
    // that window's topmost flyout, the only one Escape acts on. Weak references, so a flyout that is never
    // closed does not stay alive; entries are also removed on close.
    [ThreadStatic]
    private static List<WeakReference<Flyout>>? openFlyoutsInOpenOrder;

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
    /// Gets or sets the header border brush (separator line under the header).
    /// </summary>
    [DependencyProperty]
    private Brush? headerBorderBrush;

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
    /// Gets or sets whether focus is trapped within the flyout when open.
    /// When enabled, Tab/Shift+Tab cycles through focusable elements within the flyout.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isFocusTrapEnabled;

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

        ResolveTemplateParts();
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

        TryCloseOnEscape(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);

        if (e.Handled || !IsOpen || !IsFocusTrapEnabled)
        {
            return;
        }

        if (e.Key == Key.Tab)
        {
            HandleTabNavigation(e);
        }
    }

    private void HandleTabNavigation(KeyEventArgs e)
    {
        if (flyoutPanel is null)
        {
            return;
        }

        var focusableElements = GetFocusableElements(flyoutPanel);
        if (focusableElements.Count == 0)
        {
            return;
        }

        var currentFocused = Keyboard.FocusedElement as UIElement;
        var isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        var currentIndex = currentFocused is not null
            ? focusableElements.IndexOf(currentFocused)
            : -1;

        int nextIndex;
        if (isShiftPressed)
        {
            // Shift+Tab: Move backwards
            nextIndex = currentIndex <= 0
                ? focusableElements.Count - 1
                : currentIndex - 1;
        }
        else
        {
            // Tab: Move forwards
            nextIndex = currentIndex >= focusableElements.Count - 1 || currentIndex < 0
                ? 0
                : currentIndex + 1;
        }

        if (nextIndex >= 0 && nextIndex < focusableElements.Count)
        {
            _ = focusableElements[nextIndex].Focus();
            e.Handled = true;
        }
    }

    private static List<UIElement> GetFocusableElements(DependencyObject parent)
    {
        var focusableElements = new List<UIElement>();
        GetFocusableElementsRecursive(parent, focusableElements);
        return focusableElements;
    }

    private static void GetFocusableElementsRecursive(
        DependencyObject parent,
        List<UIElement> focusableElements)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is UIElement element &&
                element.Visibility == Visibility.Visible &&
                element.IsEnabled &&
                element.Focusable &&
                KeyboardNavigation.GetIsTabStop(element))
            {
                focusableElements.Add(element);
            }

            GetFocusableElementsRecursive(child, focusableElements);
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
        => TryCloseOnEscape(e);

    /// <summary>
    /// Escape only acts on the topmost (most recently opened) open flyout in the window. When that flyout
    /// cannot be closed by Escape (CloseOnEscape is false, it is pinned, or it is still animating), Escape
    /// does nothing: it is not passed on to the flyouts below it.
    /// </summary>
    private void TryCloseOnEscape(KeyEventArgs e)
    {
        if (e.Handled ||
            e.Key != Key.Escape ||
            !IsOpen ||
            !IsTopmostOpenFlyout())
        {
            return;
        }

        if (CloseOnEscape && !isAnimating && !IsPinned)
        {
            CloseWithLightDismiss();
            e.Handled = true;
        }
    }

    private void RegisterAsOpen()
    {
        UnregisterAsOpen();
        GetOpenFlyoutsInOpenOrder().Add(new WeakReference<Flyout>(this));
    }

    private static List<WeakReference<Flyout>> GetOpenFlyoutsInOpenOrder()
        => openFlyoutsInOpenOrder ??= [];

    private void UnregisterAsOpen()
        => openFlyoutsInOpenOrder?.RemoveAll(reference =>
            !reference.TryGetTarget(out var flyout) || ReferenceEquals(flyout, this));

    private bool IsTopmostOpenFlyout()
    {
        var scope = GetInputScope(this);
        if (scope is null || openFlyoutsInOpenOrder is null)
        {
            return true;
        }

        for (var i = openFlyoutsInOpenOrder.Count - 1; i >= 0; i--)
        {
            if (openFlyoutsInOpenOrder[i].TryGetTarget(out var flyout) &&
                flyout.IsOpen &&
                ReferenceEquals(GetInputScope(flyout), scope))
            {
                return ReferenceEquals(flyout, this);
            }
        }

        return true;
    }

    private static DependencyObject? GetInputScope(Flyout flyout)
        => (DependencyObject?)Window.GetWindow(flyout) ?? PresentationSource.FromVisual(flyout)?.RootVisual;

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

        RaiseClosing(args);

        if (args.Cancel)
        {
            return;
        }

        isClosingEventRaised = true;
        try
        {
            IsOpen = false;
        }
        finally
        {
            isClosingEventRaised = false;
        }
    }

    private static void OnIsOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Flyout flyout || flyout.isRestoringIsOpen)
        {
            return;
        }

        var newIsOpen = (bool)e.NewValue;

        if (newIsOpen)
        {
            flyout.RegisterAsOpen();
            flyout.OnOpening();
        }
        else
        {
            flyout.OnClosing();
        }
    }

    private void OnOpening()
    {
        var args = new FlyoutOpeningEventArgs(OpeningEvent, this);
        RaiseOpening(args);

        if (args.Cancel)
        {
            IsOpen = false;
            return;
        }

        // Store current focus
        previouslyFocusedElement = Keyboard.FocusedElement;

        // Show and animate. A flyout that has never been shown is collapsed and has no template yet,
        // so apply it now: without its parts the first open would skip the animation.
        Visibility = Visibility.Visible;
        ApplyTemplate();
        AnimateOpen();
    }

    private void OnClosing()
    {
        if (isClosingEventRaised)
        {
            UnregisterAsOpen();
            AnimateClose();
            return;
        }

        var args = new FlyoutClosingEventArgs(ClosingEvent, this)
        {
            IsLightDismiss = false,
        };

        RaiseClosing(args);

        if (args.Cancel)
        {
            // Put IsOpen back without re-running the open sequence (no Opening, no open animation);
            // the flyout never left the open order, so it stays topmost.
            isRestoringIsOpen = true;
            try
            {
                SetCurrentValue(IsOpenProperty, value: true);
            }
            finally
            {
                isRestoringIsOpen = false;
            }

            return;
        }

        UnregisterAsOpen();
        AnimateClose();
    }

    private void AnimateOpen()
    {
        if (flyoutPanel is null)
        {
            Visibility = Visibility.Visible;
            RaiseOpened();
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
            RaiseOpened();
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
            RaiseOpened();
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

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Animation logic is clearer when kept together")]
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

        if (flyoutPanel is null)
        {
            isAnimating = false;
            RaiseOpened();
            _ = Focus();
            return;
        }

        scaleTransform.ScaleX = 0.9;
        scaleTransform.ScaleY = 0.9;
        flyoutPanel.Opacity = 0;

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
            RaiseOpened();
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
            RaiseClosed();
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
            RaiseClosed();
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
            RaiseClosed();
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
            RaiseClosed();
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
            RaiseClosed();
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