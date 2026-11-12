namespace Atc.Wpf.Sample.SamplesWpfNetwork.Discovery;

public partial class ServiceDiscoverySampleView
{
    public ServiceDiscoverySampleView()
    {
        InitializeComponent();
        DataContext = new ServiceDiscoverySampleViewModel();
    }
}