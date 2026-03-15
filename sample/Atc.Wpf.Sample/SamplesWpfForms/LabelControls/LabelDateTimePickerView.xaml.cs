namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelDateTimePickerView
{
    public LabelDateTimePickerView()
    {
        InitializeComponent();

        DataContext = new LabelControlDemoViewModel();
    }
}