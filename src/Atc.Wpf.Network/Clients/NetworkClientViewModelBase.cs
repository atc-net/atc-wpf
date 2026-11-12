namespace Atc.Wpf.Network.Clients;

/// <summary>
/// Base view model for a simple network client test tool: connect to a host and port, send text or hex,
/// and show what is sent and received in a <c>TerminalViewer</c>.
/// </summary>
public abstract partial class NetworkClientViewModelBase : ViewModelBase, IDisposable
{
    /// <summary>
    /// The prefix of error lines, for the terminal's <c>TermsError</c>.
    /// </summary>
    public const string ErrorPrefix = "[ERROR]";

    private readonly Dispatcher dispatcher;

    /// <summary>
    /// Gets or sets the host name or IP address.
    /// </summary>
    [ObservableProperty]
    private string? host;

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    [ObservableProperty]
    private int port;

    /// <summary>
    /// Gets or sets a value indicating whether the client is connected.
    /// </summary>
    [ObservableProperty]
    private bool isConnected;

    /// <summary>
    /// Gets or sets the text to send.
    /// </summary>
    [ObservableProperty]
    private string? sendText;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="SendText"/> is hexadecimal bytes, such as <c>48 65 6C</c>.
    /// </summary>
    [ObservableProperty]
    private bool sendAsHex;

    /// <summary>
    /// Gets or sets a value indicating whether received data is shown as hexadecimal bytes.
    /// </summary>
    [ObservableProperty]
    private bool showReceivedAsHex;

    /// <summary>
    /// Gets or sets the line ending added to sent text.
    /// </summary>
    [ObservableProperty]
    private TerminationType lineEnding;

    /// <summary>
    /// Gets or sets the error message of the last action.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkClientViewModelBase"/> class.
    /// </summary>
    protected NetworkClientViewModelBase()
    {
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        Output = WriteToTerminal;
    }

    /// <summary>
    /// Gets a value indicating whether the last action failed.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets the id of the <c>TerminalViewer</c> that shows this client's traffic.
    /// </summary>
    public string TerminalId { get; } = $"network-client-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the terms the <c>TerminalViewer</c> shows as errors.
    /// </summary>
    public IList<string> ErrorTerms { get; } = [ErrorPrefix];

    /// <summary>
    /// Gets or sets the encoding of sent and received text; UTF-8 by default.
    /// </summary>
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets where output lines go; by default to the <c>TerminalViewer</c> with <see cref="TerminalId"/>.
    /// </summary>
    public Action<string> Output { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Parses hexadecimal bytes such as <c>48 65 6C</c>, <c>48-65-6C</c>, <c>0x48 0x65</c> or <c>48656C</c>.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns><c>true</c> when the text is whole bytes of hexadecimal digits.</returns>
    public static bool TryParseHex(
        string? text,
        out byte[] bytes)
    {
        bytes = [];
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var digits = new StringBuilder();
        foreach (var part in text.Split([' ', '-', ':', ',', ';', '\t'], StringSplitOptions.RemoveEmptyEntries))
        {
            digits.Append(part.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? part[2..] : part);
        }

        var hex = digits.ToString();
        if (hex.Length == 0 ||
            hex.Length % 2 != 0 ||
            !hex.All(char.IsAsciiHexDigit))
        {
            return false;
        }

        bytes = System.Convert.FromHexString(hex);
        return true;
    }

    /// <summary>
    /// Releases the client.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    /// <summary>
    /// Opens the connection.
    /// </summary>
    /// <param name="hostName">The host name or IP address.</param>
    /// <param name="portNumber">The port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> when connected.</returns>
    protected abstract Task<bool> OpenAsync(
        string hostName,
        int portNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sends bytes over the open connection.
    /// </summary>
    /// <param name="data">The bytes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the bytes are sent.</returns>
    protected abstract Task SendBytesAsync(
        byte[] data,
        CancellationToken cancellationToken);

    /// <summary>
    /// Closes the connection.
    /// </summary>
    /// <returns>A task that completes when the connection is closed.</returns>
    protected abstract Task CloseAsync();

    /// <summary>
    /// Shows received bytes. Safe to call from any thread.
    /// </summary>
    /// <param name="data">The bytes.</param>
    protected void OnDataReceived(byte[] data)
        => RunOnUi(() => Output(
            "← " + (ShowReceivedAsHex
                ? System.Convert.ToHexString(data).Chunk(2).Select(x => new string(x)).Aggregate((a, b) => $"{a} {b}")
                : TextEncoding.GetString(data).TrimEnd('\r', '\n'))));

    /// <summary>
    /// Handles the other side closing the connection. Safe to call from any thread.
    /// </summary>
    protected void OnRemoteDisconnected()
        => RunOnUi(() =>
        {
            if (IsConnected)
            {
                IsConnected = false;
                Output(Resources.Resources.ClientDisconnected);
            }
        });

    /// <summary>
    /// Shows an error the client reported. Safe to call from any thread.
    /// </summary>
    /// <param name="exception">The error.</param>
    protected void OnClientError(Exception exception)
        => RunOnUi(() => Output($"{ErrorPrefix} {exception?.Message}"));

    private bool CanConnect()
        => !string.IsNullOrWhiteSpace(Host) &&
           Port is > 0 and <= ushort.MaxValue &&
           !IsConnected &&
           !IsBusy;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task Connect(CancellationToken cancellationToken)
    {
        ErrorMessage = null;
        IsBusy = true;
        var endpoint = $"{Host!.Trim()}:{Port}";
        try
        {
            if (await OpenAsync(Host.Trim(), Port, cancellationToken).ConfigureAwait(true))
            {
                IsConnected = true;
                Output(string.Format(CultureInfo.CurrentCulture, Resources.Resources.ClientConnectedFormat1, endpoint));
            }
            else
            {
                ErrorMessage ??= string.Format(CultureInfo.CurrentCulture, Resources.Resources.ClientConnectFailedFormat1, endpoint);
                Output($"{ErrorPrefix} {ErrorMessage}");
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.ClientConnectFailedFormat1, $"{endpoint} ({ex.Message})");
            Output($"{ErrorPrefix} {ErrorMessage}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanDisconnect()
        => IsConnected;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Closing must always end as disconnected.")]
    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private async Task Disconnect()
    {
        try
        {
            await CloseAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Output($"{ErrorPrefix} {ex.Message}");
        }

        IsConnected = false;
        Output(Resources.Resources.ClientDisconnected);
    }

    private bool CanSend()
        => IsConnected && !string.IsNullOrEmpty(SendText);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(CancellationToken cancellationToken)
    {
        ErrorMessage = null;
        byte[] data;
        if (SendAsHex)
        {
            if (!TryParseHex(SendText, out data))
            {
                ErrorMessage = Resources.Resources.ClientInvalidHex;
                return;
            }
        }
        else
        {
            data = TextEncoding.GetBytes(SendText + GetLineEndingText(LineEnding));
        }

        try
        {
            await SendBytesAsync(data, cancellationToken).ConfigureAwait(true);
            Output("→ " + SendText);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = ex.Message;
            Output($"{ErrorPrefix} {ex.Message}");
        }
    }

    private static string GetLineEndingText(TerminationType terminationType)
        => terminationType switch
        {
            TerminationType.LineFeed => "\n",
            TerminationType.CarriageReturn => "\r",
            TerminationType.CarriageReturnLineFeed => "\r\n",
            TerminationType.EndOfText => "\u0003",
            TerminationType.EndOfTransmission => "\u0004",
            _ => string.Empty,
        };

    private void RunOnUi(Action action)
    {
        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            _ = dispatcher.BeginInvoke(action);
        }
    }

    private void WriteToTerminal(string line)
        => Messenger.Default.Send(new TerminalReceivedDataEventArgs([line]) { TerminalId = TerminalId });
}