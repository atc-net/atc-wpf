namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class EPaperDisplayView
{
    public EPaperDisplayView()
    {
        InitializeComponent();

        var viewModel = new EPaperDisplayDemoViewModel();
        DataContext = viewModel;

        // The frame stream and the replay tick on the dispatcher; stop them so they do no work and keep nothing alive after the page is left.
        Loaded += (_, _) => viewModel.Resume();
        Unloaded += (_, _) => viewModel.Stop();
    }
}