namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="Slider"/>, such as thumb and track brushes and mouse wheel value changes.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class SliderHelper
{
    /// <summary>Gets or sets the value of the ThumbFillBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? thumbFillBrush;

    /// <summary>Gets or sets the value of the ThumbFillHoverBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? thumbFillHoverBrush;

    /// <summary>Gets or sets the value of the ThumbFillPressedBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? thumbFillPressedBrush;

    /// <summary>Gets or sets the value of the ThumbFillDisabledBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? thumbFillDisabledBrush;

    /// <summary>Gets or sets the value of the TrackFillBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackFillBrush;

    /// <summary>Gets or sets the value of the TrackFillHoverBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackFillHoverBrush;

    /// <summary>Gets or sets the value of the TrackFillPressedBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackFillPressedBrush;

    /// <summary>Gets or sets the value of the TrackFillDisabledBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackFillDisabledBrush;

    /// <summary>Gets or sets the value of the TrackValueFillBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackValueFillBrush;

    /// <summary>Gets or sets the value of the TrackValueFillHoverBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackValueFillHoverBrush;

    /// <summary>Gets or sets the value of the TrackValueFillPressedBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackValueFillPressedBrush;

    /// <summary>Gets or sets the value of the TrackValueFillDisabledBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? trackValueFillDisabledBrush;

    /// <summary>Gets or sets the value of the ChangeValueBy attached property.</summary>
    [AttachedProperty(DefaultValue = MouseWheelChangeState.SmallChange, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static MouseWheelChangeState changeValueBy;

    /// <summary>Gets or sets the value of the EnableMouseWheel attached property.</summary>
    [AttachedProperty(
        DefaultValue = MouseWheelState.None,
        PropertyChangedCallback = nameof(OnEnableMouseWheelChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static MouseWheelState enableMouseWheel;

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