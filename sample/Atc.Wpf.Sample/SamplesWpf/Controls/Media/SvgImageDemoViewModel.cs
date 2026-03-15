namespace Atc.Wpf.Sample.SamplesWpf.Controls.Media;

public partial class SvgImageDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Width", "Size", 1)]
    [PropertyRange(10, 1000, 10)]
    [ObservableProperty]
    private int imageWidth = 400;

    [PropertyDisplay("Height", "Size", 2)]
    [PropertyRange(10, 1000, 10)]
    [ObservableProperty]
    private int imageHeight = 400;

    [PropertyDisplay("Control Size Type", "Layout", 1)]
    [ObservableProperty]
    private ControlSizeType controlSizeType = ControlSizeType.ContentToSizeStretch;

    [PropertyDisplay("Background Color", "Appearance", 1)]
    [ObservableProperty]
    private Color backgroundColor = Colors.Transparent;
}