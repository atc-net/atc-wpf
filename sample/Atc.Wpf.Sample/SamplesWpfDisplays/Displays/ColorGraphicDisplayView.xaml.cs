namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class ColorGraphicDisplayView
{
    public ColorGraphicDisplayView()
    {
        InitializeComponent();

        var viewModel = new ColorGraphicDisplayDemoViewModel();

        // Before the data context is set the Buffer binding has no source, so the control shows its own buffer.
        viewModel.AttachControlBuffer(Display.Buffer);
        DataContext = viewModel;

        // The frame stream and the replay tick on the dispatcher; stop them so they do no work and keep nothing alive after the page is left.
        Loaded += (_, _) => viewModel.Resume();
        Unloaded += (_, _) => viewModel.Stop();
    }
}