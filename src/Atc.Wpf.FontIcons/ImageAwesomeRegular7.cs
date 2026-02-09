namespace Atc.Wpf.FontIcons;

public sealed class ImageAwesomeRegular7 : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Identifies the Foreground dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        nameof(Foreground),
        typeof(Brush),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(Brushes.Black, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Icon dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(FontAwesomeRegular7Type),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(FontAwesomeRegular7Type.None, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinProperty = DependencyProperty.Register(
        nameof(Spin),
        typeof(bool),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(false, OnSpinChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinDurationProperty = DependencyProperty.Register(
        nameof(SpinDuration),
        typeof(double),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(1d, OnSpinDurationChanged, FontIconHelper.SpinDurationCoerceValue));

    /// <summary>
    /// Identifies the Rotation dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
        nameof(Rotation),
        typeof(double),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(0d, OnRotationChanged, FontIconHelper.RotationCoerceValue));

    /// <summary>
    /// Identifies the FlipOrientation dependency property.
    /// </summary>
    public static readonly DependencyProperty FlipOrientationProperty = DependencyProperty.Register(
        nameof(FlipOrientation),
        typeof(FlipOrientationType),
        typeof(ImageAwesomeRegular7),
        new PropertyMetadata(FlipOrientationType.Normal, OnFlipOrientationChanged));

    /// <summary>
    /// Typeface used to generate FontAwesome 7 Regular icon.
    /// </summary>
    private static readonly Typeface FontAwesomeRegular7Typeface = new(
        ResourceFontHelper.GetAwesome7Free(),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    /// <summary>
    /// Gets or sets the foreground brush of the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    public FontAwesomeRegular7Type Icon
    {
        get => (FontAwesomeRegular7Type)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the current spin (angle) animation of the icon.
    /// </summary>
    public bool Spin
    {
        get => (bool)GetValue(SpinProperty);
        set => SetValue(SpinProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the spinning animation (in seconds). This will stop and start the spin animation.
    /// </summary>
    public double SpinDuration
    {
        get => (double)GetValue(SpinDurationProperty);
        set => SetValue(SpinDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the current rotation (angle).
    /// </summary>
    public double Rotation
    {
        get => (double)GetValue(RotationProperty);
        set => SetValue(RotationProperty, value);
    }

    /// <summary>
    /// Gets or sets the current orientation (horizontal, vertical).
    /// </summary>
    public FlipOrientationType FlipOrientation
    {
        get => (FlipOrientationType)GetValue(FlipOrientationProperty);
        set => SetValue(FlipOrientationProperty, value);
    }

    public static ImageSource CreateImageSource(
        FontAwesomeRegular7Type fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontAwesomeRegular7Typeface, iconChar, foregroundBrush, emSize);
    }

    public static DrawingImage CreateDrawingImage(
        FontAwesomeRegular7Type fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateDrawingImage(FontAwesomeRegular7Typeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeRegular7 imageAwesome)
        {
            return;
        }

        d.SetCurrentValue(SourceProperty, CreateImageSource(imageAwesome.Icon, imageAwesome.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageAwesomeRegular7 { Spin: true } imageAwesome ||
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
        if (d is not ImageAwesomeRegular7 imageAwesome)
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
        if (d is not ImageAwesomeRegular7 imageAwesome ||
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
        if (d is not ImageAwesomeRegular7 imageAwesome ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageAwesome.SetFlipOrientation();
    }
}