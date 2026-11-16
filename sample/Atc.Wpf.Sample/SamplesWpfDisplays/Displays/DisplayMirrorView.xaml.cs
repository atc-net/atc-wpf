namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class DisplayMirrorView
{
    public DisplayMirrorView()
    {
        InitializeComponent();

        var viewModel = new DisplayMirrorDemoViewModel();
        DataContext = viewModel;

        // The transport, the loopback demo and the statistics timer run only while the page is shown.
        Loaded += (_, _) => viewModel.Resume();
        Unloaded += (_, _) => viewModel.Stop();
    }
}