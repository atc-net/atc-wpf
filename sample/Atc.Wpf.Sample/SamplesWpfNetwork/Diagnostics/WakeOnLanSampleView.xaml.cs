namespace Atc.Wpf.Sample.SamplesWpfNetwork.Diagnostics;

public partial class WakeOnLanSampleView
{
    public WakeOnLanSampleView()
    {
        InitializeComponent();
        DataContext = new WakeOnLanViewModel();
    }
}