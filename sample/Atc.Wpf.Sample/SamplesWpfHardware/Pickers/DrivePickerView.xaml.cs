namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class DrivePickerView
{
    public DrivePickerView()
    {
        InitializeComponent();
        DataContext = new DrivePickerDemoViewModel();
    }
}