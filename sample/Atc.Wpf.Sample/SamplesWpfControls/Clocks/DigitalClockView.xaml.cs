namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class DigitalClockView
{
    public DigitalClockView()
    {
        InitializeComponent();
        DataContext = new DigitalClockDemoViewModel();
    }
}