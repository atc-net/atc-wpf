namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="Slider"/>, such as thumb and track brushes and mouse wheel value changes.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class SliderHelper
{
    /// <summary>Identifies the ThumbFillBrush attached property.</summary>
    public static readonly DependencyProperty ThumbFillBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ThumbFillBrush attached property.</summary>
    public static Brush? GetThumbFillBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillBrushProperty);

    /// <summary>Sets the value of the ThumbFillBrush attached property.</summary>
    public static void SetThumbFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillBrushProperty, value);

    /// <summary>Identifies the ThumbFillHoverBrush attached property.</summary>
    public static readonly DependencyProperty ThumbFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ThumbFillHoverBrush attached property.</summary>
    public static Brush? GetThumbFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillHoverBrushProperty);

    /// <summary>Sets the value of the ThumbFillHoverBrush attached property.</summary>
    public static void SetThumbFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillHoverBrushProperty, value);

    /// <summary>Identifies the ThumbFillPressedBrush attached property.</summary>
    public static readonly DependencyProperty ThumbFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ThumbFillPressedBrush attached property.</summary>
    public static Brush? GetThumbFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillPressedBrushProperty);

    /// <summary>Sets the value of the ThumbFillPressedBrush attached property.</summary>
    public static void SetThumbFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillPressedBrushProperty, value);

    /// <summary>Identifies the ThumbFillDisabledBrush attached property.</summary>
    public static readonly DependencyProperty ThumbFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ThumbFillDisabledBrush attached property.</summary>
    public static Brush? GetThumbFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillDisabledBrushProperty);

    /// <summary>Sets the value of the ThumbFillDisabledBrush attached property.</summary>
    public static void SetThumbFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillDisabledBrushProperty, value);

    /// <summary>Identifies the TrackFillBrush attached property.</summary>
    public static readonly DependencyProperty TrackFillBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackFillBrush attached property.</summary>
    public static Brush? GetTrackFillBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillBrushProperty);

    /// <summary>Sets the value of the TrackFillBrush attached property.</summary>
    public static void SetTrackFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillBrushProperty, value);

    /// <summary>Identifies the TrackFillHoverBrush attached property.</summary>
    public static readonly DependencyProperty TrackFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackFillHoverBrush attached property.</summary>
    public static Brush? GetTrackFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillHoverBrushProperty);

    /// <summary>Sets the value of the TrackFillHoverBrush attached property.</summary>
    public static void SetTrackFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillHoverBrushProperty, value);

    /// <summary>Identifies the TrackFillPressedBrush attached property.</summary>
    public static readonly DependencyProperty TrackFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackFillPressedBrush attached property.</summary>
    public static Brush? GetTrackFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillPressedBrushProperty);

    /// <summary>Sets the value of the TrackFillPressedBrush attached property.</summary>
    public static void SetTrackFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillPressedBrushProperty, value);

    /// <summary>Identifies the TrackFillDisabledBrush attached property.</summary>
    public static readonly DependencyProperty TrackFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackFillDisabledBrush attached property.</summary>
    public static Brush? GetTrackFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillDisabledBrushProperty);

    /// <summary>Sets the value of the TrackFillDisabledBrush attached property.</summary>
    public static void SetTrackFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillDisabledBrushProperty, value);

    /// <summary>Identifies the TrackValueFillBrush attached property.</summary>
    public static readonly DependencyProperty TrackValueFillBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackValueFillBrush attached property.</summary>
    public static Brush? GetTrackValueFillBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillBrushProperty);

    /// <summary>Sets the value of the TrackValueFillBrush attached property.</summary>
    public static void SetTrackValueFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillBrushProperty, value);

    /// <summary>Identifies the TrackValueFillHoverBrush attached property.</summary>
    public static readonly DependencyProperty TrackValueFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackValueFillHoverBrush attached property.</summary>
    public static Brush? GetTrackValueFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillHoverBrushProperty);

    /// <summary>Sets the value of the TrackValueFillHoverBrush attached property.</summary>
    public static void SetTrackValueFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillHoverBrushProperty, value);

    /// <summary>Identifies the TrackValueFillPressedBrush attached property.</summary>
    public static readonly DependencyProperty TrackValueFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackValueFillPressedBrush attached property.</summary>
    public static Brush? GetTrackValueFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillPressedBrushProperty);

    /// <summary>Sets the value of the TrackValueFillPressedBrush attached property.</summary>
    public static void SetTrackValueFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillPressedBrushProperty, value);

    /// <summary>Identifies the TrackValueFillDisabledBrush attached property.</summary>
    public static readonly DependencyProperty TrackValueFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the TrackValueFillDisabledBrush attached property.</summary>
    public static Brush? GetTrackValueFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillDisabledBrushProperty);

    /// <summary>Sets the value of the TrackValueFillDisabledBrush attached property.</summary>
    public static void SetTrackValueFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillDisabledBrushProperty, value);

    /// <summary>Identifies the ChangeValueBy attached property.</summary>
    public static readonly DependencyProperty ChangeValueByProperty = DependencyProperty.RegisterAttached(
        "ChangeValueBy",
        typeof(MouseWheelChangeState),
        typeof(SliderHelper),
        new PropertyMetadata(MouseWheelChangeState.SmallChange));

    /// <summary>Gets the value of the ChangeValueBy attached property.</summary>
    public static MouseWheelChangeState GetChangeValueBy(UIElement element)
        => (MouseWheelChangeState)element.GetValue(ChangeValueByProperty);

    /// <summary>Sets the value of the ChangeValueBy attached property.</summary>
    public static void SetChangeValueBy(
        UIElement element,
        MouseWheelChangeState value)
        => element.SetValue(ChangeValueByProperty, value);

    /// <summary>Identifies the EnableMouseWheel attached property.</summary>
    public static readonly DependencyProperty EnableMouseWheelProperty = DependencyProperty.RegisterAttached(
        "EnableMouseWheel",
        typeof(MouseWheelState),
        typeof(SliderHelper),
        new PropertyMetadata(
            MouseWheelState.None,
            OnEnableMouseWheelChanged));

    /// <summary>Gets the value of the EnableMouseWheel attached property.</summary>
    public static MouseWheelState GetEnableMouseWheel(UIElement element)
        => (MouseWheelState)element.GetValue(EnableMouseWheelProperty);

    /// <summary>Sets the value of the EnableMouseWheel attached property.</summary>
    public static void SetEnableMouseWheel(
        UIElement element,
        MouseWheelState value)
        => element.SetValue(EnableMouseWheelProperty, value);

    private static void OnEnableMouseWheelChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue || d is not Slider slider)
        {
            return;
        }

        slider.PreviewMouseWheel -= OnSliderPreviewMouseWheel;
        if ((MouseWheelState)e.NewValue != MouseWheelState.None)
        {
            slider.PreviewMouseWheel += OnSliderPreviewMouseWheel;
        }
    }

    internal static object ConstrainToRange(
        RangeBase rangeBase,
        double value)
    {
        var minimum = rangeBase.Minimum;
        if (value < minimum)
        {
            return minimum;
        }

        var maximum = rangeBase.Maximum;
        return value > maximum
            ? maximum
            : value;
    }

    private static void OnSliderPreviewMouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (sender is not Slider slider ||
            (!slider.IsFocused && !MouseWheelState.MouseHover.Equals(slider.GetValue(EnableMouseWheelProperty))))
        {
            return;
        }

        var changeType = (MouseWheelChangeState)slider.GetValue(ChangeValueByProperty);
        var difference = changeType == MouseWheelChangeState.LargeChange
            ? slider.LargeChange
            : slider.SmallChange;

        var sliderValue = e.Delta > 0
            ? slider.Value + difference
            : slider.Value - difference;

        var newValue = ConstrainToRange(slider, sliderValue);

        slider.SetCurrentValue(RangeBase.ValueProperty, newValue);

        e.Handled = true;
    }
}