namespace Atc.Wpf.Network.Tests.Scanner;

/// <summary>
/// An <see cref="IIPScanner"/> that scans nothing: it reports the results a test gives it.
/// </summary>
/// <param name="configuration">The scanner configuration the view model created.</param>
internal sealed class FakeIpScanner(IPScannerConfig configuration) : IIPScanner, IDisposable
{
    public event EventHandler<IPScannerProgressReport>? ProgressReporting;

    public IPScannerConfig Configuration { get; set; } = configuration;

    public IList<IPScanResult> ResultsToReport { get; } = new List<IPScanResult>();

    public IPAddress? ScannedStart { get; private set; }

    public IPAddress? ScannedEnd { get; private set; }

    public IPAddress? ScannedSingle { get; private set; }

    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets or sets what runs after the last progress report, before the scan returns,
    /// as IPScanner attaches the discovered services then.
    /// </summary>
    public Action? AfterLastReport { get; set; }

    public Task<IPScanResults> Scan(
        IPAddress ipAddress,
        CancellationToken cancellationToken = default)
    {
        ScannedSingle = ipAddress;
        return Task.FromResult(Report());
    }

    public Task<IPScanResults> ScanCidrRange(
        IPAddress ipAddress,
        byte cidrLength,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IPScanResults> ScanRange(
        IPAddress startIpAddress,
        IPAddress endIpAddress,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ScannedStart = startIpAddress;
        ScannedEnd = endIpAddress;
        return Task.FromResult(Report());
    }

    public void Dispose()
        => IsDisposed = true;

    private IPScanResults Report()
    {
        var results = new IPScanResults();
        for (var i = 0; i < ResultsToReport.Count; i++)
        {
            var result = ResultsToReport[i];
            ProgressReporting?.Invoke(
                this,
                new IPScannerProgressReport
                {
                    TasksToProcessCount = ResultsToReport.Count,
                    TasksProcessedCount = i + 1,
                    LatestUpdate = result,
                });
        }

        // Like IPScanner: one result per address, the last one reported.
        foreach (var result in ResultsToReport.GroupBy(x => x.IPAddress).Select(x => x.Last()))
        {
            results.CollectedResults.Add(result);
        }

        AfterLastReport?.Invoke();
        results.End = DateTime.Now;
        return results;
    }
}