namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class NetworkAdapterPickerView
{
    public NetworkAdapterPickerView()
    {
        InitializeComponent();
        DataContext = new NetworkAdapterPickerDemoViewModel();
    }
}