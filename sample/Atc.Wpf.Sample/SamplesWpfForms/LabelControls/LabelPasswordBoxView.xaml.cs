namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelPasswordBoxView
{
    public LabelPasswordBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}