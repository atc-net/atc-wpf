namespace Atc.Wpf.Sample.SamplesWpfForms.Pickers;

public partial class TimeZonePickerView
{
    public TimeZonePickerView()
    {
        InitializeComponent();
        DataContext = new TimeZonePickerDemoViewModel();
    }
}