namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class WindowPickerView
{
    public WindowPickerView()
    {
        InitializeComponent();
        DataContext = new WindowPickerDemoViewModel();
    }
}