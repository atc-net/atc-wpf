namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// One router (or the target) on the route, as shown by <see cref="TracerouteView"/>.
/// </summary>
public partial class TracerouteHopViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the name of the router, when it was looked up.
    /// </summary>
    [ObservableProperty]
    private string? hostName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TracerouteHopViewModel"/> class.
    /// </summary>
    /// <param name="hop">The hop.</param>
    public TracerouteHopViewModel(TracerouteHop hop)
    {
        ArgumentNullException.ThrowIfNull(hop);

        HopNumber = hop.HopNumber;
        IpAddress = hop.IPAddress;
        RoundtripTime = hop.RoundtripTime;
        Status = hop.Status;
        IsDestination = hop.IsDestination;
    }

    /// <summary>
    /// Gets the hop number: 1, 2, 3, …
    /// </summary>
    public int HopNumber { get; }

    /// <summary>
    /// Gets the address that answered, or <c>null</c> when nothing answered in time.
    /// </summary>
    public IPAddress? IpAddress { get; }

    /// <summary>
    /// Gets the answer time, or <c>null</c> when nothing answered.
    /// </summary>
    public TimeSpan? RoundtripTime { get; }

    /// <summary>
    /// Gets the answer time as text, such as <c>21 ms</c>, or <c>*</c> when nothing answered.
    /// </summary>
    public string RoundtripTimeText
        => RoundtripTime is null
            ? "*"
            : string.Create(CultureInfo.CurrentCulture, $"{(long)RoundtripTime.Value.TotalMilliseconds} ms");

    /// <summary>
    /// Gets the ping status of the hop.
    /// </summary>
    public IPStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the target itself answered.
    /// </summary>
    public bool IsDestination { get; }
}