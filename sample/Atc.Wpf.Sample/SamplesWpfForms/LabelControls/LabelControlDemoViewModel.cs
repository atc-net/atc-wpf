namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelControlDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Is Mandatory", "Validation", 1)]
    [ObservableProperty]
    private bool isMandatory;

    [PropertyDisplay("Orientation", "Layout", 1)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    [PropertyDisplay("Label Text", "Content", 1)]
    [ObservableProperty]
    private string labelText = "Label";

    [PropertyDisplay("Watermark Text", "Content", 2)]
    [ObservableProperty]
    private string watermarkText = string.Empty;

    [PropertyDisplay("Information Text", "Content", 3)]
    [ObservableProperty]
    private string informationText = string.Empty;

    [PropertyDisplay("Show Clear Button", "Behavior", 1)]
    [ObservableProperty]
    private bool showClearTextButton;
}