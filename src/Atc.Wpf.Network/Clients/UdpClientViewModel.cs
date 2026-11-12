namespace Atc.Wpf.Network.Clients;

/// <summary>
/// View model for <see cref="UdpClientView"/>: a UDP client test tool.
/// </summary>
public partial class UdpClientViewModel : NetworkClientViewModelBase
{
    /// <summary>
    /// Gets or sets how the client is created; by default an Atc.Network <c>UdpClient</c>.
    /// </summary>
    public Func<IPAddress, int, IUdpClient> ClientFactory { get; set; } = (address, portNumber)
        => new UdpClient(address, portNumber);

    /// <summary>
    /// Gets or sets how the host is turned into an address; by default with DNS.
    /// </summary>
    public Func<string, CancellationToken, Task<IPAddress?>> AddressResolver { get; set; } = ResolveAddress;

    private IUdpClient? client;

    /// <inheritdoc />
    protected override async Task<bool> OpenAsync(
        string hostName,
        int portNumber,
        CancellationToken cancellationToken)
    {
        ReleaseClient();
        var address = await AddressResolver(hostName, cancellationToken).ConfigureAwait(true);
        if (address is null)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.UnknownHostFormat1, hostName);
            return false;
        }

        client = ClientFactory(address, portNumber);
        client.DataReceived += OnDataReceived;
        client.Disconnected += OnRemoteDisconnected;
        client.ErrorOccurred += OnErrorOccurred;

        var connected = await client.Connect(cancellationToken).ConfigureAwait(true);
        if (!connected)
        {
            ReleaseClient();
        }

        return connected;
    }

    /// <inheritdoc />
    protected override Task SendBytesAsync(
        byte[] data,
        CancellationToken cancellationToken)
        => client?.Send(data, cancellationToken) ?? Task.CompletedTask;

    /// <inheritdoc />
    protected override async Task CloseAsync()
    {
        if (client is not null)
        {
            await client.Disconnect().ConfigureAwait(true);
        }

        ReleaseClient();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ReleaseClient();
        }

        base.Dispose(disposing);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "An unknown host is reported as no address.")]
    private static async Task<IPAddress?> ResolveAddress(
        string hostName,
        CancellationToken cancellationToken)
    {
        if (IPAddress.TryParse(hostName, out var address))
        {
            return address;
        }

        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostName, cancellationToken).ConfigureAwait(false);
            return addresses.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ??
                   addresses.FirstOrDefault();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return null;
        }
    }

    private void OnErrorOccurred(
        object? sender,
        NetworkErrorEventArgs e)
        => OnClientError(e.Exception);

    private void ReleaseClient()
    {
        if (client is null)
        {
            return;
        }

        client.DataReceived -= OnDataReceived;
        client.Disconnected -= OnRemoteDisconnected;
        client.ErrorOccurred -= OnErrorOccurred;
        client.Dispose();
        client = null;
    }
}