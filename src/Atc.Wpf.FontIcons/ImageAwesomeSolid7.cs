namespace Atc.Wpf.FontIcons;

/// <summary>
/// An image that renders a Font Awesome 7 Free (solid) icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(ImageIconMembers))]
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class ImageAwesomeSolid7 : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Typeface used to generate FontAwesome 7 Solid icon.
    /// </summary>
    private static readonly Typeface FontAwesomeSolid7Typeface = new(
        ResourceFontHelper.GetAwesome7Free(),
        FontStyles.Normal,
        FontWeights.Black,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontAwesomeSolid7Type.None, PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private FontAwesomeSolid7Type icon;

    /// <summary>
    /// Creates an image source that renders the specified Font Awesome 7 Free (solid) icon with the given foreground brush and em-size.
    /// </summary>
    public static ImageSource CreateImageSource(
        FontAwesomeSolid7Type fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontAwesomeSolid7Typeface, iconChar, foregroundBrush, emSize);
    }

    /// <summary>
    /// Creates a drawing image that renders the specified Font Awesome 7 Free (solid) icon with the given foreground brush and em-size.
    /// </summary>
    public static DrawingImage CreateDrawingImage(
        FontAwesomeSolid7Type fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(FontAwesomeSolid7Typeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeSolid7 imageAwesome)
        {
            return;
        }

        d.SetCurrentValue(
            SourceProperty,
            FontIconHelper.GetIconImageSource(
                FontAwesomeSolid7Typeface,
                char.ConvertFromUtf32((int)imageAwesome.Icon),
                imageAwesome.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeSolid7 { Spin: true } imageAwesome ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageAwesome.StopSpin();
        imageAwesome.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeSolid7 imageAwesome)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            imageAwesome.BeginSpin();
        }
        else
        {
            imageAwesome.StopSpin();
            imageAwesome.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeSolid7 imageAwesome ||
            imageAwesome.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageAwesome.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeSolid7 imageAwesome ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageAwesome.SetFlipOrientation();
    }
}