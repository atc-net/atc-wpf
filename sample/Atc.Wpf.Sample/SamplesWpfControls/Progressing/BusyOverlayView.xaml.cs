namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class BusyOverlayView : IDisposable
{
    private CancellationTokenSource? cts;

    public BusyOverlayView()
    {
        InitializeComponent();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            cts?.Dispose();
        }
    }

    private async void OnStartJobClick(
        object sender,
        RoutedEventArgs e)
    {
        if (cts is not null)
        {
            return;
        }

        cts = new CancellationTokenSource();
        BtnStartJob.IsEnabled = false;
        JobBusyOverlay.IsBusy = true;

        try
        {
            await RunSimulatedJob(cts.Token).ConfigureAwait(true);
            TbJobStatus.Text = "Completed";
        }
        catch (OperationCanceledException)
        {
            TbJobStatus.Text = "Cancelled";
        }
        finally
        {
            JobBusyOverlay.IsBusy = false;
            BtnStartJob.IsEnabled = true;
            cts.Dispose();
            cts = null;
        }
    }

    private void OnCancelJobClick(
        object sender,
        RoutedEventArgs e)
    {
        cts?.Cancel();
    }

    private async Task RunSimulatedJob(CancellationToken cancellationToken)
    {
        const int totalSteps = 10;

        for (var i = 1; i <= totalSteps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            JobBusyOverlay.BusyContent = $"Processing step {i} of {totalSteps}...";

            await Task
                .Delay(800, cancellationToken)
                .ConfigureAwait(true);
        }
    }
}