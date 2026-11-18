namespace Atc.Wpf.FontIcons;

/// <summary>
/// An image that renders an IcoFont icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(ImageIconMembers))]
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class ImageIcoFont : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Typeface used to generate FontWeather icon.
    /// </summary>
    private static readonly Typeface IcoFontTypeface = new(
        ResourceFontHelper.GetIcoFont(),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = IcoFontType.None, PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private IcoFontType icon;

    /// <summary>
    /// Creates an image source that renders the specified IcoFont icon with the given foreground brush and em-size.
    /// </summary>
    public static ImageSource CreateImageSource(
        IcoFontType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(IcoFontTypeface, iconChar, foregroundBrush, emSize);
    }

    /// <summary>
    /// Creates a drawing image that renders the specified IcoFont icon with the given foreground brush and em-size.
    /// </summary>
    public static DrawingImage CreateDrawingImage(
        IcoFontType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(IcoFontTypeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageIcoFont imageIcoFont)
        {
            return;
        }

        d.SetCurrentValue(
            SourceProperty,
            FontIconHelper.GetIconImageSource(
                IcoFontTypeface,
                char.ConvertFromUtf32((int)imageIcoFont.Icon),
                imageIcoFont.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageIcoFont { Spin: true } imageIcoFont ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageIcoFont.StopSpin();
        imageIcoFont.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageIcoFont imageIcoFont)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            imageIcoFont.BeginSpin();
        }
        else
        {
            imageIcoFont.StopSpin();
            imageIcoFont.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageIcoFont imageIcoFont ||
            imageIcoFont.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageIcoFont.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageIcoFont imageIcoFont || e.NewValue is not FlipOrientationType || e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageIcoFont.SetFlipOrientation();
    }
}