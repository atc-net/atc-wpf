namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCurrencyBoxView
{
    public LabelCurrencyBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();
    }
}