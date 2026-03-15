namespace Atc.Wpf.Sample.SamplesWpfControls.Pickers;

public partial class PickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Title", "Content", 1)]
    [ObservableProperty]
    private string title = string.Empty;

    [PropertyDisplay("Watermark Text", "Content", 2)]
    [ObservableProperty]
    private string watermarkText = string.Empty;

    [PropertyDisplay("Show Clear Button", "Behavior", 1)]
    [ObservableProperty]
    private bool showClearTextButton;
}