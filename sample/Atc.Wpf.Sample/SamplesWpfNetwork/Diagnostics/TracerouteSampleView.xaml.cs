namespace Atc.Wpf.Sample.SamplesWpfNetwork.Diagnostics;

public partial class TracerouteSampleView
{
    public TracerouteSampleView()
    {
        InitializeComponent();
        DataContext = new TracerouteViewModel();
    }
}