namespace Atc.Wpf.Sample.SamplesWpfComponents.Monitoring;

public partial class ApplicationMonitorView : IDisposable
{
    private readonly ApplicationMonitorDemoViewModel viewModel;

    public ApplicationMonitorView()
    {
        InitializeComponent();

        viewModel = new ApplicationMonitorDemoViewModel();
        DataContext = viewModel;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        viewModel.Dispose();
    }
}