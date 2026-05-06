namespace Atc.Wpf.Sample.SamplesWpfForms.Pickers;

public partial class TimeZonePickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Watermark Text", "Content", 1)]
    [ObservableProperty]
    private string watermarkText = "Select time zone…";
}