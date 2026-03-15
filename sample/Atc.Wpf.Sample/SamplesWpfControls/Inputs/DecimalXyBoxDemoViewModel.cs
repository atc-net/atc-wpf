namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class DecimalXyBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Decimal Places", "Appearance", 1)]
    [PropertyRange(0, 15, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int decimalPlaces = 2;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 2)]
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