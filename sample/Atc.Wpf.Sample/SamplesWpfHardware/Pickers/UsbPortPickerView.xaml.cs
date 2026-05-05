namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class UsbPortPickerView
{
    public UsbPortPickerView()
    {
        InitializeComponent();
        DataContext = new UsbPortPickerDemoViewModel();
    }
}