namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// A dual-thumb slider control for selecting a range of values.
/// </summary>
[TemplatePart(Name = "PART_Track", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_RangeHighlight", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_LowerThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "PART_UpperThumb", Type = typeof(Thumb))]
public sealed partial class RangeSlider : Control
{
    private FrameworkElement? track;
    private FrameworkElement? rangeHighlight;
    private Thumb? lowerThumb;
    private Thumb? upperThumb;

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    [DependencyProperty(DefaultValue = 0.0, PropertyChangedCallback = nameof(OnRangePropertyChanged))]
    private double minimum;

    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    [DependencyProperty(DefaultValue = 100.0, PropertyChangedCallback = nameof(OnRangePropertyChanged))]
    private double maximum;

    /// <summary>
    /// The lower (start) value of the selected range.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        PropertyChangedCallback = nameof(OnRangeStartChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private double rangeStart;

    /// <summary>
    /// The upper (end) value of the selected range.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 100.0,
        PropertyChangedCallback = nameof(OnRangeEndChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private double rangeEnd;

    /// <summary>
    /// The step increment for value changes.
    /// </summary>
    [DependencyProperty(DefaultValue = 1.0)]
    private double step;

    /// <summary>
    /// Whether to show value tooltips while dragging.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showValueToolTips;

    /// <summary>
    /// The format string for tooltip values.
    /// </summary>
    [DependencyProperty(DefaultValue = "N0")]
    private string toolTipFormat;

    /// <summary>
    /// The brush used for the track background.
    /// </summary>
    [DependencyProperty]
    private Brush? trackBackground;

    /// <summary>
    /// The brush used for the range highlight between thumbs.
    /// </summary>
    [DependencyProperty]
    private Brush? rangeHighlightBrush;

    /// <summary>
    /// The brush used for the thumb controls.
    /// </summary>
    [DependencyProperty]
    private Brush? thumbBrush;

    /// <summary>
    /// The brush used for the thumb controls on hover.
    /// </summary>
    [DependencyProperty]
    private Brush? thumbHoverBrush;

    /// <summary>
    /// The brush used for the thumb controls when pressed.
    /// </summary>
    [DependencyProperty]
    private Brush? thumbPressedBrush;

    /// <summary>
    /// The size of the thumb controls.
    /// </summary>
    [DependencyProperty(DefaultValue = 20.0)]
    private double thumbSize;

    /// <summary>
    /// The height of the track.
    /// </summary>
    [DependencyProperty(DefaultValue = 4.0)]
    private double trackHeight;

    /// <summary>
    /// Occurs when the range values change.
    /// </summary>
    public event RoutedEventHandler? RangeChanged;

    static RangeSlider()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(RangeSlider),
            new FrameworkPropertyMetadata(typeof(RangeSlider)));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UnhookThumbEvents();

        track = GetTemplateChild("PART_Track") as FrameworkElement;
        rangeHighlight = GetTemplateChild("PART_RangeHighlight") as FrameworkElement;
        lowerThumb = GetTemplateChild("PART_LowerThumb") as Thumb;
        upperThumb = GetTemplateChild("PART_UpperThumb") as Thumb;

        HookThumbEvents();
        UpdateThumbPositions();
    }

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdateThumbPositions();
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

        // Check which thumb has keyboard focus
        var focusedElement = Keyboard.FocusedElement;
        var isLowerFocused = ReferenceEquals(focusedElement, lowerThumb);
        var isUpperFocused = ReferenceEquals(focusedElement, upperThumb);

        // If the control itself has focus, default to lower thumb
        if (!isLowerFocused && !isUpperFocused && ReferenceEquals(focusedElement, this))
        {
            isLowerFocused = true;
        }

        if (!isLowerFocused && !isUpperFocused)
        {
            return;
        }

        HandleKeyboardNavigation(e, isLowerFocused, isUpperFocused);
    }

    private void HandleKeyboardNavigation(
        KeyEventArgs e,
        bool isLowerFocused,
        bool isUpperFocused)
    {
        var delta = Step;
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            delta *= 10;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.Down:
                HandleDecreaseKey(isLowerFocused, isUpperFocused, delta);
                e.Handled = true;
                break;

            case Key.Right:
            case Key.Up:
                HandleIncreaseKey(isLowerFocused, isUpperFocused, delta);
                e.Handled = true;
                break;

            case Key.Home:
                HandleHomeKey(isLowerFocused, isUpperFocused);
                e.Handled = true;
                break;

            case Key.End:
                HandleEndKey(isLowerFocused, isUpperFocused);
                e.Handled = true;
                break;
        }
    }

    private void HandleDecreaseKey(
        bool isLowerFocused,
        bool isUpperFocused,
        double delta)
    {
        if (isLowerFocused)
        {
            RangeStart = System.Math.Max(Minimum, RangeStart - delta);
        }
        else if (isUpperFocused)
        {
            RangeEnd = System.Math.Max(RangeStart, RangeEnd - delta);
        }
    }

    private void HandleIncreaseKey(
        bool isLowerFocused,
        bool isUpperFocused,
        double delta)
    {
        if (isLowerFocused)
        {
            RangeStart = System.Math.Min(RangeEnd, RangeStart + delta);
        }
        else if (isUpperFocused)
        {
            RangeEnd = System.Math.Min(Maximum, RangeEnd + delta);
        }
    }

    private void HandleHomeKey(
        bool isLowerFocused,
        bool isUpperFocused)
    {
        if (isLowerFocused)
        {
            RangeStart = Minimum;
        }
        else if (isUpperFocused)
        {
            RangeEnd = RangeStart;
        }
    }

    private void HandleEndKey(
        bool isLowerFocused,
        bool isUpperFocused)
    {
        if (isLowerFocused)
        {
            RangeStart = RangeEnd;
        }
        else if (isUpperFocused)
        {
            RangeEnd = Maximum;
        }
    }

    private void UnhookThumbEvents()
    {
        if (lowerThumb is not null)
        {
            lowerThumb.DragStarted -= OnLowerThumbDragStarted;
            lowerThumb.DragDelta -= OnLowerThumbDragDelta;
        }

        if (upperThumb is not null)
        {
            upperThumb.DragStarted -= OnUpperThumbDragStarted;
            upperThumb.DragDelta -= OnUpperThumbDragDelta;
        }
    }

    private void HookThumbEvents()
    {
        if (lowerThumb is not null)
        {
            lowerThumb.DragStarted += OnLowerThumbDragStarted;
            lowerThumb.DragDelta += OnLowerThumbDragDelta;
        }

        if (upperThumb is not null)
        {
            upperThumb.DragStarted += OnUpperThumbDragStarted;
            upperThumb.DragDelta += OnUpperThumbDragDelta;
        }
    }

    private void OnLowerThumbDragStarted(
        object sender,
        DragStartedEventArgs e)
    {
        lowerThumb?.Focus();
    }

    private void OnUpperThumbDragStarted(
        object sender,
        DragStartedEventArgs e)
    {
        upperThumb?.Focus();
    }

    private void OnLowerThumbDragDelta(
        object sender,
        DragDeltaEventArgs e)
    {
        if (track is null)
        {
            return;
        }

        var trackWidth = track.ActualWidth - ThumbSize;
        if (trackWidth <= 0)
        {
            return;
        }

        var valueDelta = (e.HorizontalChange / trackWidth) * (Maximum - Minimum);
        var newValue = RangeStart + valueDelta;
        newValue = SnapToStep(newValue);
        newValue = System.Math.Max(Minimum, System.Math.Min(RangeEnd, newValue));

        RangeStart = newValue;
    }

    private void OnUpperThumbDragDelta(
        object sender,
        DragDeltaEventArgs e)
    {
        if (track is null)
        {
            return;
        }

        var trackWidth = track.ActualWidth - ThumbSize;
        if (trackWidth <= 0)
        {
            return;
        }

        var valueDelta = (e.HorizontalChange / trackWidth) * (Maximum - Minimum);
        var newValue = RangeEnd + valueDelta;
        newValue = SnapToStep(newValue);
        newValue = System.Math.Max(RangeStart, System.Math.Min(Maximum, newValue));

        RangeEnd = newValue;
    }

    private double SnapToStep(double value)
    {
        if (Step <= 0)
        {
            return value;
        }

        var steps = System.Math.Round((value - Minimum) / Step);
        return Minimum + (steps * Step);
    }

    private void UpdateThumbPositions()
    {
        if (track is null || lowerThumb is null || upperThumb is null || rangeHighlight is null)
        {
            return;
        }

        var trackWidth = track.ActualWidth - ThumbSize;
        if (trackWidth <= 0)
        {
            return;
        }

        var range = Maximum - Minimum;
        if (range <= 0)
        {
            return;
        }

        var lowerPercent = (RangeStart - Minimum) / range;
        var upperPercent = (RangeEnd - Minimum) / range;

        var lowerPos = lowerPercent * trackWidth;
        var upperPos = upperPercent * trackWidth;

        Canvas.SetLeft(lowerThumb, lowerPos);
        Canvas.SetLeft(upperThumb, upperPos);

        Canvas.SetLeft(rangeHighlight, lowerPos + (ThumbSize / 2));
        rangeHighlight.Width = System.Math.Max(0, upperPos - lowerPos);
    }

    private void OnRangeChanged()
        => RangeChanged?.Invoke(this, new RoutedEventArgs());

    private static void OnRangePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is RangeSlider slider)
        {
            slider.CoerceRangeValues();
            slider.UpdateThumbPositions();
        }
    }

    private static void OnRangeStartChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is RangeSlider slider)
        {
            slider.CoerceRangeValues();
            slider.UpdateThumbPositions();
            slider.OnRangeChanged();
        }
    }

    private static void OnRangeEndChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is RangeSlider slider)
        {
            slider.CoerceRangeValues();
            slider.UpdateThumbPositions();
            slider.OnRangeChanged();
        }
    }

    private void CoerceRangeValues()
    {
        if (RangeStart < Minimum)
        {
            RangeStart = Minimum;
        }

        if (RangeStart > Maximum)
        {
            RangeStart = Maximum;
        }

        if (RangeEnd < RangeStart)
        {
            RangeEnd = RangeStart;
        }

        if (RangeEnd > Maximum)
        {
            RangeEnd = Maximum;
        }
    }
}