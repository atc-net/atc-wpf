namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class ProcessPickerView
{
    public ProcessPickerView()
    {
        InitializeComponent();
        DataContext = new ProcessPickerDemoViewModel();
    }
}