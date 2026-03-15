namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class ThicknessBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Minimum", "Range", 1)]
    [PropertyRange(-100, 0, 1)]
    [ObservableProperty]
    private double minimum;

    [PropertyDisplay("Maximum", "Range", 2)]
    [PropertyRange(0, 1000, 10)]
    [ObservableProperty]
    private double maximum = 100;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 1)]
    [ObservableProperty]
    private bool hideUpDownButtons;
}