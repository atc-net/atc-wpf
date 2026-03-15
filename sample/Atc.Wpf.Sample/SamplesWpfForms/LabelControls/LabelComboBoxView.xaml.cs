namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelComboBoxView
{
    public LabelComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}