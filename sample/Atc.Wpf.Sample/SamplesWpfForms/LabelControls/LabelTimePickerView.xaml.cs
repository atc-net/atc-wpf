namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTimePickerView
{
    public LabelTimePickerView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}