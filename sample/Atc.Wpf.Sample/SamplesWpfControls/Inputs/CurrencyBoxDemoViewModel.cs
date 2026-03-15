namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class CurrencyBoxDemoViewModel : ViewModelBase
{
    private double? value = 1234.56;

    [PropertyDisplay("Minimum", "Range", 1)]
    [PropertyRange(-100000, 0, 1000)]
    [ObservableProperty]
    private double minimum;

    [PropertyDisplay("Maximum", "Range", 2)]
    [PropertyRange(0, 1000000, 1000)]
    [ObservableProperty]
    private double maximum = 100000;

    [PropertyDisplay("Interval", "Range", 3)]
    [PropertyRange(0.01, 100, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double interval = 1;

    [PropertyDisplay("Decimal Places", "Appearance", 1)]
    [PropertyRange(0, 4, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int decimalPlaces = 2;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 2)]
    [ObservableProperty]
    private bool hideUpDownButtons;

    [PropertyDisplay("Is Read Only", "Behavior", 1)]
    [ObservableProperty]
    private bool isReadOnly;

    [PropertyDisplay("Value", "Value", 1)]
    [PropertyRange(-100000, 100000, 1)]
    public double? Value
    {
        get => value;
        set => Set(ref this.value, value);
    }
}