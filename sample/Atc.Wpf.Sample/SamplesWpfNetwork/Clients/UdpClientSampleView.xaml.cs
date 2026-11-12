namespace Atc.Wpf.Sample.SamplesWpfNetwork.Clients;

public partial class UdpClientSampleView
{
    public UdpClientSampleView()
    {
        InitializeComponent();
        DataContext = new UdpClientViewModel { Port = 5005 };
    }
}