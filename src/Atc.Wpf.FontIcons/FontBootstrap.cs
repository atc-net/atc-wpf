namespace Atc.Wpf.FontIcons;

/// <summary>
/// A text block that renders a Bootstrap (GlyphIcons Halflings) icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class FontBootstrap : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontBootstrapType.None, PropertyChangedCallback = nameof(OnIconChanged))]
    private FontBootstrapType icon;

    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0020:Cast value to correct type.", Justification = "Reviewed.")]
    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0022:Cast value to correct type.", Justification = "Reviewed.")]
    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetBootstrap());
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontBootstrap { Spin: true } fontBootstrap ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontBootstrap.StopSpin();
        fontBootstrap.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontBootstrap fontBootstrap)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            fontBootstrap.BeginSpin();
        }
        else
        {
            fontBootstrap.StopSpin();
            fontBootstrap.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontBootstrap fontBootstrap ||
            fontBootstrap.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontBootstrap.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontBootstrap fontBootstrap ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontBootstrap.SetFlipOrientation();
    }
}