namespace Atc.Wpf.Sample.SamplesWpfNetwork.Clients;

public partial class TcpClientSampleView
{
    public TcpClientSampleView()
    {
        InitializeComponent();
        DataContext = new TcpClientViewModel { Port = 4001 };
    }
}