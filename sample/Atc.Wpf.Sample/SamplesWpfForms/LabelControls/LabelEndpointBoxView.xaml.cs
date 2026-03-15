namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelEndpointBoxView
{
    public LabelEndpointBoxView()
    {
        InitializeComponent();

        DataContext = new LabelControlDemoViewModel();
    }
}