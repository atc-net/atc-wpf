namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class ChipView
{
    public ChipView()
    {
        InitializeComponent();
        DataContext = new ChipDemoViewModel();
    }
}