namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class EndpointBoxView
{
    public EndpointBoxView()
    {
        InitializeComponent();
        DataContext = new EndpointBoxDemoViewModel();
    }
}