namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class TimeZonePickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Watermark Text", "Content", 1)]
    [ObservableProperty]
    private string watermarkText = "Select time zone…";
}