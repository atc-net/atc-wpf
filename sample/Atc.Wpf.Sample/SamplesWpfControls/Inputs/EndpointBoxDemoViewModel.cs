namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class EndpointBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Host", "Value", 1)]
    [ObservableProperty]
    private string host = "localhost";

    [PropertyDisplay("Port", "Value", 2)]
    [PropertyRange(1, 65535, 1)]
    [ObservableProperty]
    private int port = 8080;

    [PropertyDisplay("Show Clear Button", "Appearance", 1)]
    [ObservableProperty]
    private bool showClearTextButton = true;

    [PropertyDisplay("Hide Up/Down Buttons", "Appearance", 2)]
    [ObservableProperty]
    private bool hideUpDownButtons = true;

    [PropertyDisplay("Watermark Text", "Text", 1)]
    [ObservableProperty]
    private string watermarkText = "localhost";
}