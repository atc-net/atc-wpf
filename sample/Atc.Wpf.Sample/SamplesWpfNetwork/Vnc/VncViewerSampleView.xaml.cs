namespace Atc.Wpf.Sample.SamplesWpfNetwork.Vnc;

public partial class VncViewerSampleView
{
    public VncViewerSampleView()
    {
        InitializeComponent();
        DataContext = new VncViewerSampleViewModel();
    }
}