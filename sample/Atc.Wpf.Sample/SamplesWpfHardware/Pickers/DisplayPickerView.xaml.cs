namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class DisplayPickerView
{
    public DisplayPickerView()
    {
        InitializeComponent();
        DataContext = new DisplayPickerDemoViewModel();
    }
}