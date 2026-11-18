namespace Atc.Wpf.FontIcons;

/// <summary>
/// A text block that renders a Font Awesome 7 Free (solid) icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class FontAwesomeSolid7 : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontAwesomeSolid7Type.None, PropertyChangedCallback = nameof(OnIconChanged))]
    private FontAwesomeSolid7Type icon;

    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0020:Cast value to correct type.", Justification = "Reviewed.")]
    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0022:Cast value to correct type.", Justification = "Reviewed.")]
    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetAwesome7Free());
        d.SetCurrentValue(FontWeightProperty, FontWeights.Black);
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontAwesomeSolid7 { Spin: true } fontAwesome ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontAwesome.StopSpin();
        fontAwesome.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontAwesomeSolid7 fontAwesome)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            fontAwesome.BeginSpin();
        }
        else
        {
            fontAwesome.StopSpin();
            fontAwesome.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontAwesomeSolid7 fontAwesome ||
            fontAwesome.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontAwesome.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontAwesomeSolid7 fontAwesome ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontAwesome.SetFlipOrientation();
    }
}