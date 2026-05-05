namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class UsbCameraPickerView
{
    public UsbCameraPickerView()
    {
        InitializeComponent();
        DataContext = new UsbCameraPickerDemoViewModel();
    }
}