namespace Atc.Wpf.FontIcons;

public class ImageBootstrap : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Identifies the Foreground dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        nameof(Foreground),
        typeof(Brush),
        typeof(ImageBootstrap),
        new PropertyMetadata(Brushes.Black, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Icon dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(FontBootstrapType),
        typeof(ImageBootstrap),
        new PropertyMetadata(FontBootstrapType.None, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinProperty = DependencyProperty.Register(
        nameof(Spin),
        typeof(bool),
        typeof(ImageBootstrap),
        new PropertyMetadata(false, OnSpinChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinDurationProperty = DependencyProperty.Register(
        nameof(SpinDuration),
        typeof(double),
        typeof(ImageBootstrap),
        new PropertyMetadata(1d, OnSpinDurationChanged, FontIconHelper.SpinDurationCoerceValue));

    /// <summary>
    /// Identifies the Rotation dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
        nameof(Rotation),
        typeof(double),
        typeof(ImageBootstrap),
        new PropertyMetadata(0d, OnRotationChanged, FontIconHelper.RotationCoerceValue));

    /// <summary>
    /// Identifies the FlipOrientation dependency property.
    /// </summary>
    public static readonly DependencyProperty FlipOrientationProperty = DependencyProperty.Register(
        nameof(FlipOrientation),
        typeof(FlipOrientationType),
        typeof(ImageBootstrap),
        new PropertyMetadata(FlipOrientationType.Normal, OnFlipOrientationChanged));

    /// <summary>
    /// Typeface used to generate FontBootstrap icon.
    /// </summary>
    private static readonly Typeface FontBootstrapTypeface = new(
        ResourceFontHelper.GetBootstrap(),
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
    public FontBootstrapType Icon
    {
        get => (FontBootstrapType)GetValue(IconProperty);
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
        FontBootstrapType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontBootstrapTypeface, iconChar, foregroundBrush, emSize);
    }

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

        d.SetCurrentValue(SourceProperty, CreateImageSource(imageBootstrap.Icon, imageBootstrap.Foreground));
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

        if (!imageBootstrap.Spin ||
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