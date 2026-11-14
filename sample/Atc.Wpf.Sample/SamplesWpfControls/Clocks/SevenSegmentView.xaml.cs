namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class SevenSegmentView
{
    public SevenSegmentView()
    {
        InitializeComponent();
        DataContext = new SevenSegmentDemoViewModel();
    }
}