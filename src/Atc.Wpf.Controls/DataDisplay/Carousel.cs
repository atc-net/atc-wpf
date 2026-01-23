namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A carousel control for rotating display of images or content with navigation,
/// dot indicators, auto-play, transition animations, and infinite loop support.
/// </summary>
/// <remarks>
/// Features:
/// <list type="bullet">
///   <item>Navigation arrows (optional)</item>
///   <item>Dot indicators (optional)</item>
///   <item>Auto-play with configurable interval</item>
///   <item>Pause on hover</item>
///   <item>Multiple transition types (Slide, Fade, SlideAndFade)</item>
///   <item>Infinite loop option</item>
///   <item>Drag/swipe navigation</item>
///   <item>Keyboard navigation</item>
/// </list>
/// </remarks>
[TemplatePart(Name = PartPreviousButton, Type = typeof(Button))]
[TemplatePart(Name = PartNextButton, Type = typeof(Button))]
[TemplatePart(Name = PartIndicatorsPanel, Type = typeof(Panel))]
[TemplatePart(Name = PartContentCanvas, Type = typeof(Canvas))]
[TemplatePart(Name = PartCurrentContent, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartNextContent, Type = typeof(ContentPresenter))]
public sealed partial class Carousel : Selector
{
    private const string PartPreviousButton = "PART_PreviousButton";
    private const string PartNextButton = "PART_NextButton";
    private const string PartIndicatorsPanel = "PART_IndicatorsPanel";
    private const string PartContentCanvas = "PART_ContentCanvas";
    private const string PartCurrentContent = "PART_CurrentContent";
    private const string PartNextContent = "PART_NextContent";

    private Button? previousButton;
    private Button? nextButton;
    private Panel? indicatorsPanel;
    private Canvas? contentCanvas;
    private ContentPresenter? currentContent;
    private ContentPresenter? nextContent;

    private DispatcherTimer? autoPlayTimer;
    private bool isTransitioning;
    private Point dragStartPoint;
    private bool isDragging;

    /// <summary>
    /// Gets or sets whether navigation arrows are shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showNavigationArrows;

    /// <summary>
    /// Gets or sets whether dot indicators are shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showIndicators;

    /// <summary>
    /// Gets or sets the position of the indicators.
    /// </summary>
    [DependencyProperty(DefaultValue = IndicatorPosition.Bottom)]
    private IndicatorPosition indicatorPosition;

    /// <summary>
    /// Gets or sets whether auto-play is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnAutoPlayChanged))]
    private bool autoPlay;

    /// <summary>
    /// Gets or sets the auto-play interval in milliseconds.
    /// </summary>
    [DependencyProperty(DefaultValue = 5000.0, PropertyChangedCallback = nameof(OnAutoPlayIntervalChanged))]
    private double autoPlayInterval;

    /// <summary>
    /// Gets or sets whether auto-play pauses when the mouse hovers over the control.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool pauseOnHover;

    /// <summary>
    /// Gets or sets whether the carousel loops infinitely.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isInfiniteLoop;

    /// <summary>
    /// Gets or sets the transition animation type.
    /// </summary>
    [DependencyProperty(DefaultValue = CarouselTransitionType.Slide)]
    private CarouselTransitionType transitionType;

    /// <summary>
    /// Gets or sets the transition duration in milliseconds.
    /// </summary>
    [DependencyProperty(DefaultValue = 300.0)]
    private double transitionDuration;

    /// <summary>
    /// Gets or sets the brush used for active indicators.
    /// </summary>
    [DependencyProperty]
    private Brush? indicatorActiveBrush;

    /// <summary>
    /// Gets or sets the brush used for inactive indicators.
    /// </summary>
    [DependencyProperty]
    private Brush? indicatorInactiveBrush;

    /// <summary>
    /// Gets or sets the size of the indicators.
    /// </summary>
    [DependencyProperty(DefaultValue = 10.0)]
    private double indicatorSize;

    /// <summary>
    /// Gets or sets the spacing between indicators.
    /// </summary>
    [DependencyProperty(DefaultValue = 8.0)]
    private double indicatorSpacing;

    /// <summary>
    /// Gets or sets the corner radius for the carousel container.
    /// </summary>
    [DependencyProperty]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Gets or sets whether drag/swipe navigation is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isDragEnabled;

    /// <summary>
    /// Gets or sets the minimum drag distance (as a percentage of width) to trigger navigation.
    /// </summary>
    [DependencyProperty(DefaultValue = 0.2)]
    private double dragThreshold;

    /// <summary>
    /// Occurs after a slide change completes.
    /// </summary>
    public event EventHandler<CarouselSlideChangedEventArgs>? SlideChanged;

    /// <summary>
    /// Occurs before a slide change begins. Can be canceled.
    /// </summary>
    public event EventHandler<CarouselSlideChangingEventArgs>? SlideChanging;

    static Carousel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Carousel),
            new FrameworkPropertyMetadata(typeof(Carousel)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Carousel"/> class.
    /// </summary>
    public Carousel()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UnhookEvents();

        previousButton = GetTemplateChild(PartPreviousButton) as Button;
        nextButton = GetTemplateChild(PartNextButton) as Button;
        indicatorsPanel = GetTemplateChild(PartIndicatorsPanel) as Panel;
        contentCanvas = GetTemplateChild(PartContentCanvas) as Canvas;
        currentContent = GetTemplateChild(PartCurrentContent) as ContentPresenter;
        nextContent = GetTemplateChild(PartNextContent) as ContentPresenter;

        HookEvents();
        UpdateIndicators();
        UpdateContent();
    }

    /// <summary>
    /// Navigates to the previous slide.
    /// </summary>
    public void Previous()
    {
        if (isTransitioning || Items.Count == 0)
        {
            return;
        }

        var newIndex = SelectedIndex - 1;
        if (newIndex < 0)
        {
            newIndex = IsInfiniteLoop ? Items.Count - 1 : 0;
        }

        if (newIndex != SelectedIndex)
        {
            NavigateToIndex(newIndex, isForward: false);
        }
    }

    /// <summary>
    /// Navigates to the next slide.
    /// </summary>
    public void Next()
    {
        if (isTransitioning || Items.Count == 0)
        {
            return;
        }

        var newIndex = SelectedIndex + 1;
        if (newIndex >= Items.Count)
        {
            newIndex = IsInfiniteLoop ? 0 : Items.Count - 1;
        }

        if (newIndex != SelectedIndex)
        {
            NavigateToIndex(newIndex, isForward: true);
        }
    }

    /// <summary>
    /// Navigates to a specific slide by index.
    /// </summary>
    /// <param name="index">The zero-based index of the slide to navigate to.</param>
    public void GoToSlide(int index)
    {
        if (isTransitioning || Items.Count == 0)
        {
            return;
        }

        if (index < 0)
        {
            index = 0;
        }
        else if (index >= Items.Count)
        {
            index = Items.Count - 1;
        }

        if (index != SelectedIndex)
        {
            NavigateToIndex(index, isForward: index > SelectedIndex);
        }
    }

    /// <inheritdoc />
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnSelectionChanged(e);
        UpdateIndicators();
        UpdateContent();
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);
        UpdateIndicators();

        if (SelectedIndex < 0 && Items.Count > 0)
        {
            SelectedIndex = 0;
        }

        UpdateContent();
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

        switch (e.Key)
        {
            case Key.Left:
                Previous();
                e.Handled = true;
                break;

            case Key.Right:
                Next();
                e.Handled = true;
                break;

            case Key.Home:
                GoToSlide(0);
                e.Handled = true;
                break;

            case Key.End:
                GoToSlide(Items.Count - 1);
                e.Handled = true;
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        if (PauseOnHover && AutoPlay)
        {
            StopAutoPlay();
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        if (PauseOnHover && AutoPlay && !isDragging)
        {
            StartAutoPlay();
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseLeftButtonDown(e);

        if (!IsDragEnabled || isTransitioning)
        {
            return;
        }

        dragStartPoint = e.GetPosition(this);
        isDragging = true;
        _ = CaptureMouse();
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseLeftButtonUp(e);

        if (!isDragging)
        {
            return;
        }

        isDragging = false;
        ReleaseMouseCapture();

        var currentPoint = e.GetPosition(this);
        var dragDistance = currentPoint.X - dragStartPoint.X;
        var threshold = ActualWidth * DragThreshold;

        if (System.Math.Abs(dragDistance) >= threshold)
        {
            if (dragDistance > 0)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }

        if (PauseOnHover && AutoPlay)
        {
            StartAutoPlay();
        }
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        // Could add drag preview animation here in the future
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (AutoPlay)
        {
            StartAutoPlay();
        }

        UpdateIndicators();
        UpdateContent();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        StopAutoPlay();
    }

    private void UnhookEvents()
    {
        if (previousButton is not null)
        {
            previousButton.Click -= OnPreviousButtonClick;
        }

        if (nextButton is not null)
        {
            nextButton.Click -= OnNextButtonClick;
        }
    }

    private void HookEvents()
    {
        if (previousButton is not null)
        {
            previousButton.Click += OnPreviousButtonClick;
        }

        if (nextButton is not null)
        {
            nextButton.Click += OnNextButtonClick;
        }
    }

    private void OnPreviousButtonClick(
        object sender,
        RoutedEventArgs e)
        => Previous();

    private void OnNextButtonClick(
        object sender,
        RoutedEventArgs e)
        => Next();

    private void NavigateToIndex(
        int newIndex,
        bool isForward)
    {
        var oldIndex = SelectedIndex;

        // Raise SlideChanging event (cancelable)
        var changingArgs = new CarouselSlideChangingEventArgs(oldIndex, newIndex);
        SlideChanging?.Invoke(this, changingArgs);

        if (changingArgs.Cancel)
        {
            return;
        }

        if (TransitionType == CarouselTransitionType.None ||
            currentContent is null ||
            nextContent is null ||
            contentCanvas is null)
        {
            SelectedIndex = newIndex;
            SlideChanged?.Invoke(this, new CarouselSlideChangedEventArgs(oldIndex, newIndex));
            return;
        }

        isTransitioning = true;
        SetupNextContent(newIndex, isForward);

        var duration = TimeSpan.FromMilliseconds(TransitionDuration);
        AnimateTransition(isForward, duration);
        ScheduleTransitionCompletion(newIndex, oldIndex, duration);
    }

    private void SetupNextContent(
        int newIndex,
        bool isForward)
    {
        if (nextContent is null || contentCanvas is null)
        {
            return;
        }

        nextContent.Content = GetItemContent(newIndex);
        nextContent.Opacity = TransitionType is CarouselTransitionType.Fade or CarouselTransitionType.SlideAndFade ? 0 : 1;

        var startPosition = isForward ? contentCanvas.ActualWidth : -contentCanvas.ActualWidth;

        if (TransitionType is CarouselTransitionType.Slide or CarouselTransitionType.SlideAndFade)
        {
            Canvas.SetLeft(nextContent, startPosition);
        }
        else
        {
            Canvas.SetLeft(nextContent, 0);
        }
    }

    private void AnimateTransition(
        bool isForward,
        TimeSpan duration)
    {
        if (currentContent is null || nextContent is null || contentCanvas is null)
        {
            return;
        }

        var width = contentCanvas.ActualWidth;
        var startPosition = isForward ? width : -width;

        if (TransitionType is CarouselTransitionType.Slide or CarouselTransitionType.SlideAndFade)
        {
            AnimateSlide(currentContent, 0, isForward ? -width : width, duration);
            AnimateSlide(nextContent, startPosition, 0, duration);
        }

        if (TransitionType is CarouselTransitionType.Fade or CarouselTransitionType.SlideAndFade)
        {
            AnimateFade(currentContent, 1, 0, duration);
            AnimateFade(nextContent, 0, 1, duration);
        }
    }

    private void ScheduleTransitionCompletion(
        int newIndex,
        int oldIndex,
        TimeSpan duration)
    {
        var timer = new DispatcherTimer
        {
            Interval = duration,
        };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            CompleteTransition(newIndex, oldIndex);
        };
        timer.Start();
    }

    private void AnimateSlide(
        ContentPresenter presenter,
        double from,
        double to,
        TimeSpan duration)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = duration,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        };

        presenter.BeginAnimation(Canvas.LeftProperty, animation);
    }

    private static void AnimateFade(
        ContentPresenter presenter,
        double from,
        double to,
        TimeSpan duration)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = duration,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        };

        presenter.BeginAnimation(OpacityProperty, animation);
    }

    private void CompleteTransition(
        int newIndex,
        int oldIndex)
    {
        if (currentContent is not null && nextContent is not null)
        {
            currentContent.BeginAnimation(Canvas.LeftProperty, null);
            currentContent.BeginAnimation(OpacityProperty, null);
            nextContent.BeginAnimation(Canvas.LeftProperty, null);
            nextContent.BeginAnimation(OpacityProperty, null);

            Canvas.SetLeft(currentContent, 0);
            Canvas.SetLeft(nextContent, 0);
            currentContent.Opacity = 1;
            nextContent.Opacity = 1;
        }

        SelectedIndex = newIndex;
        isTransitioning = false;

        SlideChanged?.Invoke(this, new CarouselSlideChangedEventArgs(oldIndex, newIndex));
    }

    private object? GetItemContent(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            return null;
        }

        var item = Items[index];

        // If using ItemTemplate, let the ContentPresenter handle it
        if (ItemTemplate is not null)
        {
            return item;
        }

        // If item is already a UIElement, use it directly
        if (item is UIElement)
        {
            return item;
        }

        // Otherwise return the item and let default binding handle it
        return item;
    }

    private void UpdateContent()
    {
        if (currentContent is null)
        {
            return;
        }

        currentContent.Content = GetItemContent(SelectedIndex);
        currentContent.ContentTemplate = ItemTemplate;

        if (nextContent is not null)
        {
            nextContent.ContentTemplate = ItemTemplate;
        }
    }

    private void UpdateIndicators()
    {
        if (indicatorsPanel is null)
        {
            return;
        }

        indicatorsPanel.Children.Clear();

        for (var i = 0; i < Items.Count; i++)
        {
            var indicator = CreateIndicator(i);
            indicatorsPanel.Children.Add(indicator);
        }
    }

    private FrameworkElement CreateIndicator(int index)
    {
        var indicator = new Ellipse
        {
            Width = IndicatorSize,
            Height = IndicatorSize,
            Fill = index == SelectedIndex
                ? (IndicatorActiveBrush ?? Brushes.White)
                : (IndicatorInactiveBrush ?? new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))),
            Cursor = Cursors.Hand,
            Tag = index,
            Margin = new Thickness(IndicatorSpacing / 2, 0, IndicatorSpacing / 2, 0),
        };

        indicator.MouseLeftButtonDown += OnIndicatorClick;

        return indicator;
    }

    private void OnIndicatorClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int index })
        {
            GoToSlide(index);
        }
    }

    private void StartAutoPlay()
    {
        if (autoPlayTimer is not null)
        {
            autoPlayTimer.Stop();
        }

        autoPlayTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(AutoPlayInterval),
        };
        autoPlayTimer.Tick += OnAutoPlayTick;
        autoPlayTimer.Start();
    }

    private void StopAutoPlay()
    {
        if (autoPlayTimer is not null)
        {
            autoPlayTimer.Stop();
            autoPlayTimer.Tick -= OnAutoPlayTick;
            autoPlayTimer = null;
        }
    }

    private void OnAutoPlayTick(
        object? sender,
        EventArgs e)
    {
        if (!isTransitioning)
        {
            Next();
        }
    }

    private static void OnAutoPlayChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Carousel carousel && carousel.IsLoaded)
        {
            if ((bool)e.NewValue)
            {
                carousel.StartAutoPlay();
            }
            else
            {
                carousel.StopAutoPlay();
            }
        }
    }

    private static void OnAutoPlayIntervalChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Carousel { autoPlayTimer: not null } carousel)
        {
            carousel.autoPlayTimer.Interval = TimeSpan.FromMilliseconds((double)e.NewValue);
        }
    }
}