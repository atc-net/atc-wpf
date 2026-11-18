namespace Atc.Wpf.FontIcons;

/// <summary>
/// An image that renders a Font Awesome 5 Free (regular) icon, with support for spinning, rotation and flipping.
/// </summary>
[UseDependencyPropertyTemplate(typeof(ImageIconMembers))]
[UseDependencyPropertyTemplate(typeof(IconTransformMembers))]
public sealed partial class ImageAwesomeRegular : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Typeface used to generate FontAwesome icon.
    /// </summary>
    private static readonly Typeface FontAwesomeRegularTypeface = new(
        ResourceFontHelper.GetAwesomeRegular(),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValue = FontAwesomeRegularType.None, PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private FontAwesomeRegularType icon;

    /// <summary>
    /// Creates an image source that renders the specified Font Awesome 5 Free (regular) icon with the given foreground brush and em-size.
    /// </summary>
    public static ImageSource CreateImageSource(
        FontAwesomeRegularType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(
            FontAwesomeRegularTypeface,
            iconChar,
            foregroundBrush,
            emSize);
    }

    /// <summary>
    /// Creates a drawing image that renders the specified Font Awesome 5 Free (regular) icon with the given foreground brush and em-size.
    /// </summary>
    public static DrawingImage CreateDrawingImage(
        FontAwesomeRegularType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(
            FontAwesomeRegularTypeface,
            iconChar,
            foregroundBrush,
            emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeRegular imageAwesome)
        {
            return;
        }

        d.SetCurrentValue(
            SourceProperty,
            FontIconHelper.GetIconImageSource(
                FontAwesomeRegularTypeface,
                char.ConvertFromUtf32((int)imageAwesome.Icon),
                imageAwesome.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeRegular { Spin: true } imageAwesome ||
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
        if (d is not ImageAwesomeRegular imageAwesome)
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
        if (d is not ImageAwesomeRegular imageAwesome ||
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
        if (d is not ImageAwesomeRegular imageAwesome ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageAwesome.SetFlipOrientation();
    }
}