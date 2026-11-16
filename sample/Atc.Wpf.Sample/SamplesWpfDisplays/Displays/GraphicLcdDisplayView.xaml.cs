namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class GraphicLcdDisplayView
{
    public GraphicLcdDisplayView()
    {
        InitializeComponent();

        var viewModel = new GraphicLcdDisplayDemoViewModel();
        DataContext = viewModel;

        // The frame stream and the replay tick on the dispatcher; stop them so they do no work and keep nothing alive after the page is left.
        Loaded += (_, _) => viewModel.Resume();
        Unloaded += (_, _) => viewModel.Stop();
    }
}