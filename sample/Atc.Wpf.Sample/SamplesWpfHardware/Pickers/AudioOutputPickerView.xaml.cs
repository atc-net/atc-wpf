namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class AudioOutputPickerView
{
    public AudioOutputPickerView()
    {
        InitializeComponent();
        DataContext = new AudioOutputPickerDemoViewModel();
    }
}