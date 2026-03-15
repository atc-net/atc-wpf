namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class IntegerXyBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 1)]
    [ObservableProperty]
    private bool hideUpDownButtons;

    [PropertyDisplay("Prefix Text X", "Text", 1)]
    [ObservableProperty]
    private string prefixTextX = string.Empty;

    [PropertyDisplay("Prefix Text Y", "Text", 2)]
    [ObservableProperty]
    private string prefixTextY = string.Empty;

    [PropertyDisplay("Suffix Text", "Text", 3)]
    [ObservableProperty]
    private string suffixText = string.Empty;
}