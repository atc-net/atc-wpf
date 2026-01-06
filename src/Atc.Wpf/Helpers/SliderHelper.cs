namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class SliderHelper
{
    public static readonly DependencyProperty ThumbFillBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetThumbFillBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillBrushProperty);

    public static void SetThumbFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillBrushProperty, value);

    public static readonly DependencyProperty ThumbFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetThumbFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillHoverBrushProperty);

    public static void SetThumbFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillHoverBrushProperty, value);

    public static readonly DependencyProperty ThumbFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetThumbFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillPressedBrushProperty);

    public static void SetThumbFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillPressedBrushProperty, value);

    public static readonly DependencyProperty ThumbFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "ThumbFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetThumbFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(ThumbFillDisabledBrushProperty);

    public static void SetThumbFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ThumbFillDisabledBrushProperty, value);

    public static readonly DependencyProperty TrackFillBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackFillBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillBrushProperty);

    public static void SetTrackFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillBrushProperty, value);

    public static readonly DependencyProperty TrackFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillHoverBrushProperty);

    public static void SetTrackFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillHoverBrushProperty, value);

    public static readonly DependencyProperty TrackFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillPressedBrushProperty);

    public static void SetTrackFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillPressedBrushProperty, value);

    public static readonly DependencyProperty TrackFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "TrackFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(TrackFillDisabledBrushProperty);

    public static void SetTrackFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackFillDisabledBrushProperty, value);

    public static readonly DependencyProperty TrackValueFillBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackValueFillBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillBrushProperty);

    public static void SetTrackValueFillBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillBrushProperty, value);

    public static readonly DependencyProperty TrackValueFillHoverBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillHoverBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackValueFillHoverBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillHoverBrushProperty);

    public static void SetTrackValueFillHoverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillHoverBrushProperty, value);

    public static readonly DependencyProperty TrackValueFillPressedBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillPressedBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackValueFillPressedBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillPressedBrushProperty);

    public static void SetTrackValueFillPressedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillPressedBrushProperty, value);

    public static readonly DependencyProperty TrackValueFillDisabledBrushProperty = DependencyProperty.RegisterAttached(
        "TrackValueFillDisabledBrush",
        typeof(Brush),
        typeof(SliderHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetTrackValueFillDisabledBrush(UIElement element)
        => (Brush?)element.GetValue(TrackValueFillDisabledBrushProperty);

    public static void SetTrackValueFillDisabledBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(TrackValueFillDisabledBrushProperty, value);

    public static readonly DependencyProperty ChangeValueByProperty = DependencyProperty.RegisterAttached(
        "ChangeValueBy",
        typeof(MouseWheelChangeState),
        typeof(SliderHelper),
        new PropertyMetadata(MouseWheelChangeState.SmallChange));

    public static MouseWheelChangeState GetChangeValueBy(UIElement element)
        => (MouseWheelChangeState)element.GetValue(ChangeValueByProperty);

    public static void SetChangeValueBy(
        UIElement element,
        MouseWheelChangeState value)
        => element.SetValue(ChangeValueByProperty, value);

    public static readonly DependencyProperty EnableMouseWheelProperty = DependencyProperty.RegisterAttached(
        "EnableMouseWheel",
        typeof(MouseWheelState),
        typeof(SliderHelper),
        new PropertyMetadata(
            MouseWheelState.None,
            OnEnableMouseWheelChanged));

    public static MouseWheelState GetEnableMouseWheel(UIElement element)
        => (MouseWheelState)element.GetValue(EnableMouseWheelProperty);

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