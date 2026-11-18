namespace Atc.Wpf.FontIcons;

/// <summary>
/// An image that renders a Bootstrap (GlyphIcons Halflings) icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(ImageIconMembers))]
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class ImageBootstrap : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Typeface used to generate FontBootstrap icon.
    /// </summary>
    private static readonly Typeface FontBootstrapTypeface = new(
        ResourceFontHelper.GetBootstrap(),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontBootstrapType.None, PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private FontBootstrapType icon;

    /// <summary>
    /// Creates an image source that renders the specified Bootstrap (GlyphIcons Halflings) icon with the given foreground brush and em-size.
    /// </summary>
    public static ImageSource CreateImageSource(
        FontBootstrapType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontBootstrapTypeface, iconChar, foregroundBrush, emSize);
    }

    /// <summary>
    /// Creates a drawing image that renders the specified Bootstrap (GlyphIcons Halflings) icon with the given foreground brush and em-size.
    /// </summary>
    public static DrawingImage CreateDrawingImage(
        FontBootstrapType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(FontBootstrapTypeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageBootstrap imageBootstrap)
        {
            return;
        }

        d.SetCurrentValue(
            SourceProperty,
            FontIconHelper.GetIconImageSource(
                FontBootstrapTypeface,
                char.ConvertFromUtf32((int)imageBootstrap.Icon),
                imageBootstrap.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageBootstrap { Spin: true } imageBootstrap ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageBootstrap.StopSpin();
        imageBootstrap.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(d);

        if (d is not ImageBootstrap imageBootstrap)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            imageBootstrap.BeginSpin();
        }
        else
        {
            imageBootstrap.StopSpin();
            imageBootstrap.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageBootstrap imageBootstrap)
        {
            return;
        }

        if (imageBootstrap.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageBootstrap.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageBootstrap imageBootstrap ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageBootstrap.SetFlipOrientation();
    }
}