namespace Atc.Wpf.FontIcons;

/// <summary>
/// A text block that renders a Weather Icons icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(IconTransformMembers), Exclude = ["SpinDuration", "Rotation"])]
public sealed partial class FontWeather : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontWeatherType.None, PropertyChangedCallback = nameof(OnIconChanged))]
    private FontWeatherType icon;

    /// <summary>
    /// Gets or sets the duration of the spinning animation (in seconds). This will stop and start the spin animation.
    /// </summary>
    [DependencyProperty(DefaultValue = 1d, PropertyChangedCallback = nameof(OnSpinDurationChanged), CoerceValueCallback = nameof(CoerceSpinDuration))]
    private double spinDuration;

    /// <summary>
    /// Gets or sets the current rotation (angle).
    /// </summary>
    [DependencyProperty(DefaultValue = 0d, PropertyChangedCallback = nameof(OnRotationChanged), CoerceValueCallback = nameof(CoerceRotation))]
    private double rotation;

    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0020:Cast value to correct type.", Justification = "Reviewed.")]
    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0022:Cast value to correct type.", Justification = "Reviewed.")]
    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetWeather());
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontWeather { Spin: true } fontWeather ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontWeather.StopSpin();
        fontWeather.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontWeather fontWeather)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            fontWeather.BeginSpin();
        }
        else
        {
            fontWeather.StopSpin();
            fontWeather.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(d);

        if (d is not FontWeather fontWeather ||
            fontWeather.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontWeather.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontWeather fontWeather ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontWeather.SetFlipOrientation();
    }

    private static object CoerceSpinDuration(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : value;
    }

    [SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "OK.")]
    private static object CoerceRotation(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : val > 360
                ? 360d
                : value;
    }
}