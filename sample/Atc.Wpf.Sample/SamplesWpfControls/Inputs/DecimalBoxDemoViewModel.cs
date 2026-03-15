namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class DecimalBoxDemoViewModel : ViewModelBase
{
    private double? value = 3.14;

    [PropertyDisplay("Minimum", "Range", 1)]
    [PropertyRange(-10000, 0, 100)]
    [ObservableProperty]
    private double minimum;

    [PropertyDisplay("Maximum", "Range", 2)]
    [PropertyRange(0, 10000, 100)]
    [ObservableProperty]
    private double maximum = 1000;

    [PropertyDisplay("Interval", "Range", 3)]
    [PropertyRange(0.01, 10, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double interval = 0.1;

    [PropertyDisplay("Decimal Places", "Appearance", 1)]
    [PropertyRange(0, 15, 1)]
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
    [PropertyRange(-1000, 1000, 0.1)]
    public double? Value
    {
        get => value;
        set => Set(ref this.value, value);
    }
}