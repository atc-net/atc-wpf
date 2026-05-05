namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class BluetoothDevicePickerView
{
    public BluetoothDevicePickerView()
    {
        InitializeComponent();
        DataContext = new BluetoothDevicePickerDemoViewModel();
    }
}