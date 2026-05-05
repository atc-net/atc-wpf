namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class SerialPortPickerView
{
    public SerialPortPickerView()
    {
        InitializeComponent();
        DataContext = new SerialPortPickerDemoViewModel();
    }
}