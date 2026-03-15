namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTextInfoView
{
    public LabelTextInfoView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}