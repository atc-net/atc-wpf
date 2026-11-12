namespace Atc.Wpf.Network.Clients;

/// <summary>
/// View model for <see cref="TcpClientView"/>: a TCP client test tool with optional TLS and message framing.
/// </summary>
public partial class TcpClientViewModel : NetworkClientViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the connection uses TLS.
    /// </summary>
    [ObservableProperty]
    private bool useTls;

    /// <summary>
    /// Gets or sets how received data is split into messages.
    /// </summary>
    [ObservableProperty]
    private TcpMessageFraming messageFraming;

    /// <summary>
    /// Gets or sets the termination that ends a message with <see cref="TcpMessageFraming.TerminationType"/> framing.
    /// </summary>
    [ObservableProperty]
    private TerminationType framingTermination;

    /// <summary>
    /// Gets or sets how the client is created; by default an Atc.Network <c>TcpClient</c>.
    /// </summary>
    public Func<string, int, TcpClientConfig, ITcpClient> ClientFactory { get; set; } = (hostName, portNumber, config)
        => new TcpClient(hostName, portNumber, config);

    private ITcpClient? client;

    /// <inheritdoc />
    protected override async Task<bool> OpenAsync(
        string hostName,
        int portNumber,
        CancellationToken cancellationToken)
    {
        ReleaseClient();
        var config = new TcpClientConfig
        {
            MessageFraming = MessageFraming,
            TerminationType = FramingTermination,
            SslOptions = UseTls
                ? new System.Net.Security.SslClientAuthenticationOptions { TargetHost = hostName }
                : null,
        };

        client = ClientFactory(hostName, portNumber, config);
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