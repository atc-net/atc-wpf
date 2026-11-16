namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// A single seven-segment display digit (0-9) with an optional colon and dot, a glow (blur) layer
/// and an animated transition between digits.
/// </summary>
public partial class SevenSegment
{
    private const double LitOpacity = 1.0;
    private const string DefaultNumberBrushKey = "AtcApps.Brushes.Accent";

    /// <summary>Identifies the <see cref="Segments"/> dependency property.</summary>
    public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
        nameof(Segments),
        typeof(SevenSegmentParts?),
        typeof(SevenSegment),
        new PropertyMetadata(
            defaultValue: null,
            OnSegmentsChanged));

    private static readonly SevenSegmentParts[] DigitParts =
    [
        SevenSegmentParts.All & ~SevenSegmentParts.Middle,
        SevenSegmentParts.TopRight | SevenSegmentParts.BottomRight,
        SevenSegmentParts.Top | SevenSegmentParts.TopRight | SevenSegmentParts.Middle | SevenSegmentParts.BottomLeft | SevenSegmentParts.Bottom,
        SevenSegmentParts.Top | SevenSegmentParts.TopRight | SevenSegmentParts.Middle | SevenSegmentParts.BottomRight | SevenSegmentParts.Bottom,
        SevenSegmentParts.TopLeft | SevenSegmentParts.TopRight | SevenSegmentParts.Middle | SevenSegmentParts.BottomRight,
        SevenSegmentParts.Top | SevenSegmentParts.TopLeft | SevenSegmentParts.Middle | SevenSegmentParts.BottomRight | SevenSegmentParts.Bottom,
        SevenSegmentParts.All & ~SevenSegmentParts.TopRight,
        SevenSegmentParts.Top | SevenSegmentParts.TopRight | SevenSegmentParts.BottomRight,
        SevenSegmentParts.All,
        SevenSegmentParts.All & ~SevenSegmentParts.BottomLeft,
    ];

    private bool isColonPulsing;

    /// <summary>
    /// The digit to show (0-9), or <see langword="null"/> to show no lit segments.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnSegmentsChanged),
        ValidateValueCallback = nameof(IsValidDigit))]
    private int? digit;

    /// <summary>
    /// The brush used for the segments.
    /// </summary>
    [DependencyProperty]
    private Brush? numberColor;

    /// <summary>
    /// The opacity of an unlit segment (0 hides unlit segments).
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.05,
        PropertyChangedCallback = nameof(OnSegmentsChanged))]
    private double numberMinimumOpacity;

    /// <summary>
    /// The blur radius of the glow layer drawn behind the segments (0 turns the glow off).
    /// </summary>
    [DependencyProperty(DefaultValue = 5)]
    private int blurRadius;

    /// <summary>
    /// The brush used for the colon and the dot.
    /// </summary>
    [DependencyProperty]
    private Brush? colonColor;

    /// <summary>
    /// Whether <see cref="PulseColon"/> makes the colon fade in and out.
    /// When <see langword="false"/> the colon is shown at full opacity.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnAnimateColonChanged))]
    private bool animateColon;

    /// <summary>
    /// Whether the colon is shown to the right of the digit.
    /// </summary>
    [DependencyProperty]
    private bool showColon;

    /// <summary>
    /// Whether the dot is shown to the lower right of the digit.
    /// </summary>
    [DependencyProperty]
    private bool showDot;

    /// <summary>
    /// Whether a change of digit fades the segments instead of switching them at once.
    /// </summary>
    [DependencyProperty]
    private bool playAnimation;

    /// <summary>
    /// The duration of the fade between two digits when <see cref="PlayAnimation"/> is on.
    /// </summary>
    [DependencyProperty(DefaultValue = "TimeSpan.FromMilliseconds(50)")]
    private TimeSpan animationDuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SevenSegment"/> class.
    /// </summary>
    public SevenSegment()
    {
        InitializeComponent();

        SetResourceReference(NumberColorProperty, DefaultNumberBrushKey);
        SetResourceReference(ColonColorProperty, DefaultNumberBrushKey);

        UpdateSegments(animate: false);
    }

    /// <summary>
    /// Gets or sets the segments to light, overriding <see cref="Digit"/>; <see langword="null"/> (the default) shows <see cref="Digit"/>.
    /// The decimal point is not a segment: use <see cref="ShowDot"/>.
    /// </summary>
    public SevenSegmentParts? Segments
    {
        get => (SevenSegmentParts?)GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    /// <summary>
    /// Gets the segments that are lit: <see cref="Segments"/> when it is set, otherwise those of the current <see cref="Digit"/>.
    /// </summary>
    public SevenSegmentParts LitSegments => Segments ?? GetSegments(Digit);

    /// <summary>
    /// Gets the segments that are lit for a digit.
    /// </summary>
    /// <param name="digit">The digit (0-9), or <see langword="null"/> for none.</param>
    /// <returns>The lit segments.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The digit is not between 0 and 9.</exception>
    public static SevenSegmentParts GetSegments(int? digit)
    {
        if (digit is null)
        {
            return SevenSegmentParts.None;
        }

        if (!IsValidDigit(digit))
        {
            throw new ArgumentOutOfRangeException(nameof(digit), digit, "The digit must be between 0 and 9.");
        }

        return DigitParts[digit.Value];
    }

    /// <summary>
    /// Sets the digit to show.
    /// </summary>
    /// <param name="number">The digit (0-9).</param>
    /// <exception cref="ArgumentOutOfRangeException">The number is not between 0 and 9.</exception>
    public void SetNumber(int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, 9);

        SetCurrentValue(DigitProperty, number);
    }

    /// <summary>
    /// Fades the colon in and out once (over one second) when <see cref="AnimateColon"/> is on.
    /// A call while a pulse is running is ignored.
    /// </summary>
    public void PulseColon()
    {
        if (!AnimateColon || isColonPulsing)
        {
            return;
        }

        isColonPulsing = true;

        var first = true;
        foreach (var element in GetColonElements())
        {
            var animation = CreateColonPulseAnimation();
            if (first)
            {
                animation.Completed += OnColonPulseCompleted;
                first = false;
            }

            element.BeginAnimation(OpacityProperty, animation);
        }
    }

    internal double GetSegmentOpacity(SevenSegmentParts part)
        => GetSegmentElements(part)[0].Opacity;

    internal double GetColonOpacity()
        => Colon1Rectangle.Opacity;

    private static bool IsValidDigit(object? value)
        => value is null || value is >= 0 and <= 9;

    private static void OnSegmentsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SevenSegment)d;
        control.UpdateSegments(control.PlayAnimation);
    }

    private static void OnAnimateColonChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SevenSegment)d;
        if (control.AnimateColon)
        {
            return;
        }

        control.isColonPulsing = false;
        foreach (var element in control.GetColonElements())
        {
            element.BeginAnimation(OpacityProperty, animation: null);
            element.SetCurrentValue(OpacityProperty, LitOpacity);
        }
    }

    private static DoubleAnimationUsingKeyFrames CreateColonPulseAnimation()
    {
        var animation = new DoubleAnimationUsingKeyFrames();
        AddColonKeyFrame(animation, 0.05, TimeSpan.Zero);
        AddColonKeyFrame(animation, LitOpacity, TimeSpan.FromMilliseconds(500));
        AddColonKeyFrame(animation, 0.05, TimeSpan.FromSeconds(1));
        return animation;
    }

    private static void AddColonKeyFrame(
        DoubleAnimationUsingKeyFrames animation,
        double value,
        TimeSpan keyTime)
        => animation.KeyFrames.Add(
            new EasingDoubleKeyFrame(
                value,
                KeyTime.FromTimeSpan(keyTime),
                new CircleEase { EasingMode = EasingMode.EaseOut }));

    private void OnColonPulseCompleted(
        object? sender,
        EventArgs e)
        => isColonPulsing = false;

    private UIElement[] GetColonElements()
        =>
        [
            Colon1Rectangle,
            Colon2Rectangle,
            Colon1BlurRectangle,
            Colon2BlurRectangle,
        ];

    private UIElement[] GetSegmentElements(SevenSegmentParts part)
        => part switch
        {
            SevenSegmentParts.Top => [TopPath, TopBlurPath],
            SevenSegmentParts.TopRight => [TopRightPath, TopRightBlurPath],
            SevenSegmentParts.BottomRight => [BottomRightPath, BottomRightBlurPath],
            SevenSegmentParts.Bottom => [BottomPath, BottomBlurPath],
            SevenSegmentParts.BottomLeft => [BottomLeftPath, BottomLeftBlurPath],
            SevenSegmentParts.TopLeft => [TopLeftPath, TopLeftBlurPath],
            SevenSegmentParts.Middle => [MiddlePath, MiddleBlurPath],
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, message: null),
        };

    private void UpdateSegments(bool animate)
    {
        var lit = LitSegments;
        var duration = new Duration(AnimationDuration);

        foreach (var part in new[]
                 {
                     SevenSegmentParts.Top,
                     SevenSegmentParts.TopRight,
                     SevenSegmentParts.BottomRight,
                     SevenSegmentParts.Bottom,
                     SevenSegmentParts.BottomLeft,
                     SevenSegmentParts.TopLeft,
                     SevenSegmentParts.Middle,
                 })
        {
            var opacity = lit.HasFlag(part)
                ? LitOpacity
                : NumberMinimumOpacity;

            foreach (var element in GetSegmentElements(part))
            {
                if (animate)
                {
                    element.BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, duration));
                }
                else
                {
                    element.BeginAnimation(OpacityProperty, animation: null);
                    element.SetCurrentValue(OpacityProperty, opacity);
                }
            }
        }
    }
}