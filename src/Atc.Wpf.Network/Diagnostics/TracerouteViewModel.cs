namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// ViewModel that traces the route to a host, as <c>tracert</c> does.
/// </summary>
public partial class TracerouteViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the target: a host name or an IP address.
    /// </summary>
    [ObservableProperty]
    private string? target;

    /// <summary>
    /// Gets or sets the most routers to go through.
    /// </summary>
    [ObservableProperty]
    private int maxHops = TracerouteHelper.DefaultMaxHops;

    /// <summary>
    /// Gets or sets how long each hop waits for an answer, in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutPerHopInMs = TracerouteHelper.DefaultTimeoutPerHopInMs;

    /// <summary>
    /// Gets or sets a value indicating whether the routers' names are looked up.
    /// </summary>
    [ObservableProperty]
    private bool resolveHostNames = true;

    /// <summary>
    /// Gets or sets the error message of the last trace.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Gets a value indicating whether the last trace failed.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets the hops of the route.
    /// </summary>
    public ObservableCollection<TracerouteHopViewModel> Hops { get; } = [];

    /// <summary>
    /// Gets or sets how the route is traced; by default <see cref="TracerouteHelper.Trace"/>.
    /// </summary>
    public Func<IPAddress, int, TimeSpan, CancellationToken, IAsyncEnumerable<TracerouteHop>> Tracer { get; set; }
        = (target, maxHops, timeout, cancellationToken) => TracerouteHelper.Trace(target, maxHops, timeout, cancellationToken);

    /// <summary>
    /// Gets or sets how a host name is turned into an address; by default with DNS.
    /// </summary>
    public Func<string, CancellationToken, Task<IPAddress?>> AddressResolver { get; set; } = ResolveAddress;

    /// <summary>
    /// Gets or sets how a router's name is looked up; by default with reverse DNS.
    /// </summary>
    public Func<IPAddress, CancellationToken, Task<string?>> HostNameResolver { get; set; } = DnsLookupHelper.GetHostname;

    private bool CanTrace()
        => !string.IsNullOrWhiteSpace(Target) && !IsBusy;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanTrace), SupportsCancellation = true)]
    private async Task Trace(CancellationToken cancellationToken)
    {
        Hops.Clear();
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            var targetText = Target!.Trim();
            var address = await AddressResolver(targetText, cancellationToken).ConfigureAwait(true);
            if (address is null)
            {
                ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.UnknownHostFormat1, targetText);
                return;
            }

            var hops = Tracer(address, MaxHops, TimeSpan.FromMilliseconds(TimeoutPerHopInMs), cancellationToken);
            await foreach (var hop in hops.ConfigureAwait(true))
            {
                var row = new TracerouteHopViewModel(hop);
                Hops.Add(row);
                if (ResolveHostNames && hop.IPAddress is not null)
                {
                    row.HostName = await HostNameResolver(hop.IPAddress, cancellationToken).ConfigureAwait(true);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Stopped by the user; keep the hops found so far.
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.TracerouteErrorFormat1, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "An unknown host is reported as no address.")]
    private static async Task<IPAddress?> ResolveAddress(
        string host,
        CancellationToken cancellationToken)
    {
        if (IPAddress.TryParse(host, out var address))
        {
            return address;
        }

        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken).ConfigureAwait(false);
            return addresses.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ??
                   addresses.FirstOrDefault();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return null;
        }
    }
}