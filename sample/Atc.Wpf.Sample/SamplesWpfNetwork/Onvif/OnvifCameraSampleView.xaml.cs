namespace Atc.Wpf.Sample.SamplesWpfNetwork.Onvif;

public partial class OnvifCameraSampleView
{
    public OnvifCameraSampleView()
    {
        InitializeComponent();
        DataContext = new OnvifCameraSampleViewModel();
    }
}