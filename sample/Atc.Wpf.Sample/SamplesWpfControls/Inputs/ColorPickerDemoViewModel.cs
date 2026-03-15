namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class ColorPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Color", "Value", 1)]
    [ObservableProperty]
    private Color colorValue = Colors.Green;
}