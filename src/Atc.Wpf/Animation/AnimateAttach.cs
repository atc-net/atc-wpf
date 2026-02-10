namespace Atc.Wpf.Animation;

/// <summary>
/// Provides attached properties for declarative XAML-based animations.
/// </summary>
[SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "OK - Fire-and-forget animation in XAML callbacks.")]
public static class AnimateAttach
{
    /// <summary>
    /// Identifies the OnLoaded attached property.
    /// When set to a non-None value, the animation runs when the element is loaded.
    /// </summary>
    public static readonly DependencyProperty OnLoadedProperty = DependencyProperty.RegisterAttached(
        "OnLoaded",
        typeof(AnimationKind),
        typeof(AnimateAttach),
        new PropertyMetadata(AnimationKind.None, OnOnLoadedChanged));

    /// <summary>
    /// Identifies the OnVisible attached property.
    /// When set to a non-None value, the animation runs each time the element becomes visible.
    /// </summary>
    public static readonly DependencyProperty OnVisibleProperty = DependencyProperty.RegisterAttached(
        "OnVisible",
        typeof(AnimationKind),
        typeof(AnimateAttach),
        new PropertyMetadata(AnimationKind.None, OnOnVisibleChanged));

    /// <summary>
    /// Identifies the Duration attached property.
    /// Specifies the animation duration in milliseconds.
    /// </summary>
    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached(
        "Duration",
        typeof(double),
        typeof(AnimateAttach),
        new PropertyMetadata(300d));

    /// <summary>
    /// Identifies the SlideDistance attached property.
    /// Specifies the slide distance in pixels for slide animations.
    /// </summary>
    public static readonly DependencyProperty SlideDistanceProperty = DependencyProperty.RegisterAttached(
        "SlideDistance",
        typeof(double),
        typeof(AnimateAttach),
        new PropertyMetadata(50d));

    /// <summary>
    /// Gets the OnLoaded animation kind for the specified element.
    /// </summary>
    public static AnimationKind GetOnLoaded(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (AnimationKind)obj.GetValue(OnLoadedProperty);
    }

    /// <summary>
    /// Sets the OnLoaded animation kind for the specified element.
    /// </summary>
    public static void SetOnLoaded(
        DependencyObject obj,
        AnimationKind value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(OnLoadedProperty, value);
    }

    /// <summary>
    /// Gets the OnVisible animation kind for the specified element.
    /// </summary>
    public static AnimationKind GetOnVisible(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (AnimationKind)obj.GetValue(OnVisibleProperty);
    }

    /// <summary>
    /// Sets the OnVisible animation kind for the specified element.
    /// </summary>
    public static void SetOnVisible(
        DependencyObject obj,
        AnimationKind value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(OnVisibleProperty, value);
    }

    /// <summary>
    /// Gets the Duration value in milliseconds for the specified element.
    /// </summary>
    public static double GetDuration(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (double)obj.GetValue(DurationProperty);
    }

    /// <summary>
    /// Sets the Duration value in milliseconds for the specified element.
    /// </summary>
    public static void SetDuration(
        DependencyObject obj,
        double value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets the SlideDistance value in pixels for the specified element.
    /// </summary>
    public static double GetSlideDistance(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (double)obj.GetValue(SlideDistanceProperty);
    }

    /// <summary>
    /// Sets the SlideDistance value in pixels for the specified element.
    /// </summary>
    public static void SetSlideDistance(
        DependencyObject obj,
        double value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(SlideDistanceProperty, value);
    }

    private static AnimationParameters BuildParameters(DependencyObject obj)
    {
        var duration = GetDuration(obj);
        var slideDistance = GetSlideDistance(obj);

        return new AnimationParameters
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            SlideDistance = slideDistance,
        };
    }

    private static void OnOnLoadedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
        {
            return;
        }

        var kind = (AnimationKind)e.NewValue;
        if (kind == AnimationKind.None)
        {
            return;
        }

        element.Loaded += OnElementLoaded;
    }

    private static void OnElementLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= OnElementLoaded;

        var kind = GetOnLoaded(element);
        var parameters = BuildParameters(element);

        element.AnimateAsync(kind, parameters);
    }

    private static void OnOnVisibleChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        var kind = (AnimationKind)e.NewValue;
        if (kind == AnimationKind.None)
        {
            return;
        }

        var descriptor = DependencyPropertyDescriptor.FromProperty(
            UIElement.VisibilityProperty,
            element.GetType());

        descriptor?.AddValueChanged(element, OnVisibilityChanged);
    }

    private static void OnVisibilityChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        if (element.Visibility != Visibility.Visible)
        {
            return;
        }

        var kind = GetOnVisible(element);
        var parameters = BuildParameters(element);

        element.AnimateAsync(kind, parameters);
    }
}