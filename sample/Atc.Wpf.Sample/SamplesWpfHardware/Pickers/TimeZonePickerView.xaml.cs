namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class TimeZonePickerView
{
    public TimeZonePickerView()
    {
        InitializeComponent();
        DataContext = new TimeZonePickerDemoViewModel();
    }
}