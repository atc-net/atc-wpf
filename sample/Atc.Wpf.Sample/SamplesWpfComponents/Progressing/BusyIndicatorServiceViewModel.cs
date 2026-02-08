namespace Atc.Wpf.Sample.SamplesWpfComponents.Progressing;

public sealed partial class BusyIndicatorServiceViewModel : ViewModelBase
{
    private readonly IBusyIndicatorService busyService = new BusyIndicatorService();

    [ObservableProperty]
    private string resultLog = string.Empty;

    [RelayCommand]
    private async Task ShowThreeSeconds()
    {
        var token = busyService.Show("Working for 3 seconds...", "SampleRegion");
        AppendLog("Show: started 3s timer");
        await Task.Delay(3000).ConfigureAwait(true);
        busyService.Hide(token);
        AppendLog("Hide: 3s timer complete");
    }

    [RelayCommand]
    private async Task ShowProgress()
    {
        var token = busyService.Show("Loading data...", "SampleRegion");
        AppendLog("Show: manual progress started");

        for (var i = 0; i <= 100; i += 10)
        {
            busyService.Report(token, BusyInfo.FromProgress("Loading data", i));
            await Task.Delay(300).ConfigureAwait(true);
        }

        busyService.Hide(token);
        AppendLog("Hide: manual progress complete");
    }

    [RelayCommand]
    private async Task RunAsyncTask()
    {
        AppendLog("RunAsync: started");

        await busyService.RunAsync(
            async (_, ct) =>
            {
                await Task.Delay(2000, ct).ConfigureAwait(false);
            },
            message: "Running async task...",
            regionName: "SampleRegion").ConfigureAwait(true);

        AppendLog("RunAsync: finished");
    }

    [RelayCommand]
    private async Task RunWithProgress()
    {
        AppendLog("RunAsync+Progress: started");

        await busyService.RunAsync(
            async (progress, ct) =>
            {
                for (var i = 0; i <= 100; i += 5)
                {
                    progress.Report(BusyInfo.FromProgress("Importing records", i));
                    await Task.Delay(100, ct).ConfigureAwait(false);
                }
            },
            message: "Importing...",
            regionName: "SampleRegion").ConfigureAwait(true);

        AppendLog("RunAsync+Progress: finished");
    }

    [RelayCommand]
    private async Task CancellableTask()
    {
        AppendLog("Cancellable: started");

        await busyService.RunAsync(
            async (progress, ct) =>
            {
                var step = 0;
                while (!ct.IsCancellationRequested)
                {
                    progress.Report(BusyInfo.FromMessage($"Step {++step}... (click Cancel to stop)"));
                    await Task.Delay(500, ct).ConfigureAwait(false);
                }
            },
            message: "Running cancellable task...",
            regionName: "SampleRegion",
            allowCancellation: true).ConfigureAwait(true);

        AppendLog("Cancellable: cancelled");
    }

    private BusyToken? manualToken;

    [RelayCommand]
    private void ShowManual()
    {
        manualToken = busyService.Show("Manual busy state active", "SampleRegion");
        AppendLog("ShowManual: overlay shown");
    }

    [RelayCommand]
    private void HideManual()
    {
        if (manualToken is not null)
        {
            busyService.Hide(manualToken);
            manualToken = null;
            AppendLog("HideManual: overlay hidden");
        }
    }

    private void AppendLog(string entry)
        => ResultLog = $"[{DateTime.Now:T}] {entry}{Environment.NewLine}{ResultLog}";
}