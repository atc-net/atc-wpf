namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class AnalogClockView
{
    public AnalogClockView()
    {
        InitializeComponent();
        DataContext = new AnalogClockDemoViewModel();
    }
}