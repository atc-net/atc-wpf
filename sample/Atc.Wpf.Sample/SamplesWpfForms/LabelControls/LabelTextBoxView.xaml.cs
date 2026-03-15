namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTextBoxView
{
    public LabelTextBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}