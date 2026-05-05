namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class AudioInputPickerView
{
    public AudioInputPickerView()
    {
        InitializeComponent();
        DataContext = new AudioInputPickerDemoViewModel();
    }
}