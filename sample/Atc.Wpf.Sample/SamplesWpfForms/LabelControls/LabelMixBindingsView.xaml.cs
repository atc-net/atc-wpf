namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelMixBindingsView
{
    public LabelMixBindingsView()
    {
        InitializeComponent();

        DataContext = new LabelMixBindingsViewModel();
    }
}