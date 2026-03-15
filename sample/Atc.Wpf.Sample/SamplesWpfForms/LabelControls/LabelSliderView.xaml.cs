namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelSliderView
{
    public LabelSliderView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}