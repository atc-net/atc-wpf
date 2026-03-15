namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelDecimalXyBoxView
{
    public LabelDecimalXyBoxView()
    {
        InitializeComponent();

        DataContext = new LabelControlDemoViewModel();
    }
}