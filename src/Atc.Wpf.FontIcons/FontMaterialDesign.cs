namespace Atc.Wpf.FontIcons;

/// <summary>
/// A text block that renders a Material Design icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(IconTransformMembers), Exclude = ["SpinDuration", "Rotation"])]
public sealed partial class FontMaterialDesign : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontMaterialDesignType.None, PropertyChangedCallback = nameof(OnIconChanged))]
    private FontMaterialDesignType icon;

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
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetMaterialDesign());
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign { Spin: true } fontMaterialDesign ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.StopSpin();
        fontMaterialDesign.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            fontMaterialDesign.BeginSpin();
        }
        else
        {
            fontMaterialDesign.StopSpin();
            fontMaterialDesign.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign ||
            fontMaterialDesign.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.SetFlipOrientation();
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