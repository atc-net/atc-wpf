namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

public partial class LabelMixBindingsView
{
    public LabelMixBindingsView()
    {
        InitializeComponent();

        DataContext = new LabelMixBindingsViewModel();
    }
}