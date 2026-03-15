namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class PixelSizeBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Width", "Value", 1)]
    [PropertyRange(1, 7680, 1)]
    [ObservableProperty]
    private int valueWidth = 1920;

    [PropertyDisplay("Height", "Value", 2)]
    [PropertyRange(1, 4320, 1)]
    [ObservableProperty]
    private int valueHeight = 1080;

    [PropertyDisplay("Maximum", "Range", 1)]
    [PropertyRange(100, 15360, 100)]
    [ObservableProperty]
    private int maximum = 7680;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 1)]
    [ObservableProperty]
    private bool hideUpDownButtons;
}