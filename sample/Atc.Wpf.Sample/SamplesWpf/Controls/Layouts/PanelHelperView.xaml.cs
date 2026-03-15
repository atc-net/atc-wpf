namespace Atc.Wpf.Sample.SamplesWpf.Controls.Layouts;

public partial class PanelHelperView
{
    public PanelHelperView()
    {
        InitializeComponent();
        DataContext = new PanelHelperDemoViewModel();
    }
}