namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class IntegerBoxDemoViewModel : ViewModelBase
{
    private double? value = 42;

    [PropertyDisplay("Minimum", "Range", 1)]
    [PropertyRange(-10000, 0, 100)]
    [ObservableProperty]
    private double minimum;

    [PropertyDisplay("Maximum", "Range", 2)]
    [PropertyRange(0, 10000, 100)]
    [ObservableProperty]
    private double maximum = 1000;

    [PropertyDisplay("Interval", "Range", 3)]
    [PropertyRange(1, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double interval = 1;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 1)]
    [ObservableProperty]
    private bool hideUpDownButtons;

    [PropertyDisplay("Is Read Only", "Behavior", 1)]
    [ObservableProperty]
    private bool isReadOnly;

    [PropertyDisplay("Switch Up/Down", "Appearance", 2)]
    [ObservableProperty]
    private bool switchUpDownButtons;

    [PropertyDisplay("Prefix Text", "Text", 1)]
    [ObservableProperty]
    private string prefixText = string.Empty;

    [PropertyDisplay("Suffix Text", "Text", 2)]
    [ObservableProperty]
    private string suffixText = string.Empty;

    [PropertyDisplay("Value", "Value", 1)]
    [PropertyRange(-1000, 1000, 1)]
    public double? Value
    {
        get => value;
        set => Set(ref this.value, value);
    }
}