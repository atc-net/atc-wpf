namespace Atc.Wpf.Controls.ColorEditing;

public partial class TransparencySlider
{
    [DependencyProperty(
        DefaultValue = nameof(Colors.Red),
        Flags = FrameworkPropertyMetadataOptions.None)]
    private Color color;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnAlphaChanged))]
    private byte alpha;

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