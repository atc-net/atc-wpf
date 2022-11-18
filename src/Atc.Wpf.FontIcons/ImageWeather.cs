namespace Atc.Wpf.FontIcons;

public class ImageWeather : Image, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Identifies the Foreground dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        nameof(Foreground),
        typeof(Brush),
        typeof(ImageWeather),
        new PropertyMetadata(Brushes.Black, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Icon dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(FontWeatherType),
        typeof(ImageWeather),
        new PropertyMetadata(FontWeatherType.None, OnIconPropertyChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinProperty = DependencyProperty.Register(
        nameof(Spin),
        typeof(bool),
        typeof(ImageWeather),
        new PropertyMetadata(false, OnSpinChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinDurationProperty = DependencyProperty.Register(
        nameof(SpinDuration),
        typeof(double),
        typeof(ImageWeather),
        new PropertyMetadata(1d, OnSpinDurationChanged, FontIconHelper.SpinDurationCoerceValue));

    /// <summary>
    /// Identifies the Rotation dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
        nameof(Rotation),
        typeof(double),
        typeof(ImageWeather),
        new PropertyMetadata(0d, OnRotationChanged, FontIconHelper.RotationCoerceValue));

    /// <summary>
    /// Identifies the FlipOrientation dependency property.
    /// </summary>
    public static readonly DependencyProperty FlipOrientationProperty = DependencyProperty.Register(
        nameof(FlipOrientation),
        typeof(FlipOrientationType),
        typeof(ImageWeather),
        new PropertyMetadata(FlipOrientationType.Normal, OnFlipOrientationChanged));

    /// <summary>
    /// Typeface used to generate FontWeather icon.
    /// </summary>
    private static readonly Typeface FontWeatherTypeface = new(
        ResourceFontHelper.GetWeather(),
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
    public FontWeatherType Icon
    {
        get => (FontWeatherType)GetValue(IconProperty);
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

    public static DrawingImage CreateImageSource(
        FontWeatherType fontIconType,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var iconChar = char.ConvertFromUtf32((int)fontIconType);
        return FontIconHelper.CreateImageSource(FontWeatherTypeface, iconChar, foregroundBrush, emSize);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageWeather imageWeather)
        {
            return;
        }

        d.SetCurrentValue(SourceProperty, CreateImageSource(imageWeather.Icon, imageWeather.Foreground));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageWeather { Spin: true } imageWeather ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageWeather.StopSpin();
        imageWeather.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageWeather imageWeather)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            imageWeather.BeginSpin();
        }
        else
        {
            imageWeather.StopSpin();
            imageWeather.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var imageWeather = d as ImageWeather;
        if (imageWeather?.Spin != true ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageWeather.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageWeather imageWeather ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        imageWeather.SetFlipOrientation();
    }
}