namespace Atc.Wpf.Controls.Sample.Samples.LabelControls;

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