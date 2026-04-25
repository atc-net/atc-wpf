namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelFontPickerView
{
    public LabelFontPickerView()
    {
        InitializeComponent();

        DataContext = new LabelControlDemoViewModel();
    }
}