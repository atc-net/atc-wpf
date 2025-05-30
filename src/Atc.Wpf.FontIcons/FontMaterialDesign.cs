namespace Atc.Wpf.FontIcons;

public sealed class FontMaterialDesign : TextBlock, ISpinable, IRotatable, IFlippable
{
    /// <summary>
    /// Identifies the Icon dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(FontMaterialDesignType),
        typeof(FontMaterialDesign),
        new PropertyMetadata(FontMaterialDesignType.None, OnIconChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinProperty = DependencyProperty.Register(
        nameof(Spin),
        typeof(bool),
        typeof(FontMaterialDesign),
        new PropertyMetadata(false, OnSpinChanged));

    /// <summary>
    /// Identifies the Spin dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinDurationProperty = DependencyProperty.Register(
        nameof(SpinDuration),
        typeof(double),
        typeof(FontMaterialDesign),
        new PropertyMetadata(1d, OnSpinDurationChanged, CoerceSpinDuration));

    /// <summary>
    /// Identifies the Rotation dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
        nameof(Rotation),
        typeof(double),
        typeof(FontMaterialDesign),
        new PropertyMetadata(0d, OnRotationChanged, CoerceRotation));

    /// <summary>
    /// Identifies the FlipOrientation dependency property.
    /// </summary>
    public static readonly DependencyProperty FlipOrientationProperty = DependencyProperty.Register(
        nameof(FlipOrientation),
        typeof(FlipOrientationType),
        typeof(FontMaterialDesign),
        new PropertyMetadata(FlipOrientationType.Normal, OnFlipOrientationChanged));

    /// <summary>
    /// Gets or sets the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    public FontMaterialDesignType Icon
    {
        get => (FontMaterialDesignType)GetValue(IconProperty);
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

    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0020:Cast value to correct type.", Justification = "Reviewed.")]
    [SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0022:Cast value to correct type.", Justification = "Reviewed.")]
    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetCurrentValue(FontFamilyProperty, ResourceFontHelper.GetMaterialDesign());
        d.SetCurrentValue(TextAlignmentProperty, TextAlignment.Center);
        d.SetCurrentValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    private static void OnSpinDurationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign { Spin: true } fontMaterialDesign ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.StopSpin();
        fontMaterialDesign.BeginSpin();
    }

    private static void OnSpinChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            fontMaterialDesign.BeginSpin();
        }
        else
        {
            fontMaterialDesign.StopSpin();
            fontMaterialDesign.SetRotation();
        }
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign ||
            fontMaterialDesign.Spin ||
            e.NewValue is not double ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.SetRotation();
    }

    private static void OnFlipOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FontMaterialDesign fontMaterialDesign ||
            e.NewValue is not FlipOrientationType ||
            e.NewValue.Equals(e.OldValue))
        {
            return;
        }

        fontMaterialDesign.SetFlipOrientation();
    }

    private static object CoerceSpinDuration(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : value;
    }

    [SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "OK.")]
    private static object CoerceRotation(
        DependencyObject d, object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : val > 360
                ? 360d
                : value;
    }
}