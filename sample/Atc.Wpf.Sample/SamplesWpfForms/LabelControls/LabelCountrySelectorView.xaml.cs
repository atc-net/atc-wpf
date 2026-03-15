namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCountrySelectorView
{
    public LabelCountrySelectorView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}