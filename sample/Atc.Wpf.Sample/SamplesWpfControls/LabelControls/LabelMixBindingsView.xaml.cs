namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

/// <summary>
/// Interaction logic for LabelMixBindingsView.
/// </summary>
public partial class LabelMixBindingsView
{
    public LabelMixBindingsView()
    {
        InitializeComponent();

        DataContext = new LabelMixBindingsViewModel();
    }
}