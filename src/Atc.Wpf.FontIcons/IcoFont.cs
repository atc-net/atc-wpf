namespace Atc.Wpf.FontIcons;

/// <summary>
/// A text block that renders an IcoFont icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class IcoFont : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = IcoFontType.None, PropertyChangedCallback = nameof(OnIconChanged))]
    private IcoFontType icon;

    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0020:Cast value to correct type.", Justification = "Reviewed.")]
    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0022:Cast value to correct type.", Justification = "Reviewed.")]
    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetIcoFont());
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not IcoFont { Spin: true } icoFont ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        icoFont.StopSpin();
        icoFont.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not IcoFont icoFont)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            icoFont.BeginSpin();
        }
        else
        {
            icoFont.StopSpin();
            icoFont.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not IcoFont icoFont ||
            icoFont.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        icoFont.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not IcoFont icoFont || e.NewValue is not FlipOrientationType || e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        icoFont.SetFlipOrientation();
    }
}