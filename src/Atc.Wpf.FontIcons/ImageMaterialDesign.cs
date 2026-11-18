namespace Atc.Wpf.FontIcons;

/// <summary>
/// An image that renders a Material Design icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(ImageIconMembers))]
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class ImageMaterialDesign : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// The font material design typeface.
    /// </summary>
    private static readonly Typeface FontMaterialDesignTypeface = new(
        ResourceFontHelper.GetMaterialDesign(),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontMaterialDesignType.None, PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private FontMaterialDesignType icon;

    /// <summary>
    /// Creates an image source that renders the specified Material Design icon with the given foreground brush and em-size.
    /// </summary>
    public static ImageSource CreateImageSource(
        FontMaterialDesignType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontMaterialDesignTypeface, iconChar, foregroundBrush, emSize);
    }

    /// <summary>
    /// Creates a drawing image that renders the specified Material Design icon with the given foreground brush and em-size.
    /// </summary>
    public static DrawingImage CreateDrawingImage(
        FontMaterialDesignType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(FontMaterialDesignTypeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageMaterialDesign imageMaterialDesign)
        {
            return;
        }

        d.SetCurrentValue(
            SourceProperty,
            FontIconHelper.GetIconImageSource(
                FontMaterialDesignTypeface,
                char.ConvertFromUtf32((int)imageMaterialDesign.Icon),
                imageMaterialDesign.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageMaterialDesign { Spin: true } imageMaterialDesign ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageMaterialDesign.StopSpin();
        imageMaterialDesign.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageMaterialDesign imageMaterialDesign)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            imageMaterialDesign.BeginSpin();
        }
        else
        {
            imageMaterialDesign.StopSpin();
            imageMaterialDesign.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageMaterialDesign imageMaterialDesign ||
            imageMaterialDesign.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageMaterialDesign.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageMaterialDesign imageMaterialDesign ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageMaterialDesign.SetFlipOrientation();
    }
}