namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// ViewModel that asks an NTP server for the time, and shows how far the local clock is off.
/// </summary>
public partial class NtpTimeViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the NTP server.
    /// </summary>
    [ObservableProperty]
    private string? server = NtpHelper.DefaultHost;

    /// <summary>
    /// Gets or sets the server's time (UTC) when it answered.
    /// </summary>
    [ObservableProperty]
    private DateTime? serverTimeUtc;

    /// <summary>
    /// Gets or sets how far the local clock is behind the server (positive) or ahead of it (negative).
    /// </summary>
    [ObservableProperty]
    private TimeSpan? offset;

    /// <summary>
    /// Gets or sets the round trip time to the server.
    /// </summary>
    [ObservableProperty]
    private TimeSpan? roundTripDelay;

    /// <summary>
    /// Gets or sets the server's stratum (1 = connected to a reference clock).
    /// </summary>
    [ObservableProperty]
    private int? stratum;

    /// <summary>
    /// Gets or sets the address of the server that answered.
    /// </summary>
    [ObservableProperty]
    private string? serverAddress;

    /// <summary>
    /// Gets or sets the error message of the last query.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Gets a value indicating whether the last query failed.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets or sets how the server is asked; by default with <see cref="NtpHelper"/>.
    /// </summary>
    public Func<string, CancellationToken, Task<NtpResult>> TimeQuery { get; set; } = (host, cancellationToken)
        => NtpHelper.GetTime(host, timeout: null, cancellationToken);

    private bool CanQuery()
        => !string.IsNullOrWhiteSpace(Server) && !IsBusy;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanQuery))]
    private async Task Query(CancellationToken cancellationToken)
    {
        ErrorMessage = null;
        ServerTimeUtc = null;
        Offset = null;
        RoundTripDelay = null;
        Stratum = null;
        ServerAddress = null;
        IsBusy = true;
        try
        {
            var result = await TimeQuery(Server!.Trim(), cancellationToken).ConfigureAwait(true);
            ServerTimeUtc = result.UtcTime;
            Offset = result.Offset;
            RoundTripDelay = result.RoundTripDelay;
            Stratum = result.Stratum;
            ServerAddress = result.Server.ToString();
        }
        catch (TimeoutException)
        {
            ErrorMessage = Resources.Resources.NtpTimeout;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.NtpErrorFormat1, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}