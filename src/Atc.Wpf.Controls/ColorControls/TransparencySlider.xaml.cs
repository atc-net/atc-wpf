namespace Atc.Wpf.Controls.ColorControls;

/// <summary>
/// Interaction logic for TransparencySlider.
/// </summary>
public partial class TransparencySlider
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(TransparencySlider),
        new FrameworkPropertyMetadata(Colors.Red));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register(
        nameof(Alpha),
        typeof(byte),
        typeof(TransparencySlider),
        new PropertyMetadata((byte)0, OnAlphaChanged));

    public byte Alpha
    {
        get => (byte)GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    public TransparencySlider()
    {
        InitializeComponent();

        AdornerColor = Colors.Black;
    }

    private static void OnAlphaChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TransparencySlider)d;

        control.AdornerVerticalPercent = (byte)e.NewValue / 255D;
    }

    protected override void OnAdornerPositionChanged(double verticalPercent)
    {
        Alpha = (byte)(verticalPercent * 255);
    }
}