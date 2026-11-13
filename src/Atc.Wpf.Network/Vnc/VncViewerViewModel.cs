namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// View model for the <see cref="VncViewerView"/>, exposing connect/disconnect commands,
/// connection status and the current remote screen frame.
/// </summary>
public sealed partial class VncViewerViewModel : ViewModelBase, IDisposable
{
    private const string HostKey = "VncHost";
    private const string PortKey = "VncPort";
    private const string PasswordKey = "VncPassword";

    private readonly VncConnectionService connectionService;
    private readonly Dispatcher dispatcher;
    private string? lastHost;
    private int lastPort;
    private string? lastPassword;
    private bool isDisconnecting;

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    [ObservableProperty(DependentPropertyNames = [nameof(Stretch), nameof(IsScrollable)])]
    private VncScaleMode scaleMode;

    [ObservableProperty]
    private bool viewOnly;

    [ObservableProperty(AfterChangedCallback = nameof(RaiseCommandsCanExecuteChanged))]
    private bool shareClipboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="VncViewerViewModel"/> class.
    /// </summary>
    public VncViewerViewModel()
        : this(new VncConnectionService())
    {
    }

    internal VncViewerViewModel(VncConnectionService connectionService)
    {
        this.connectionService = connectionService;
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        connectionService.ClipboardTextReceived += OnServiceClipboardTextReceived;
        connectionService.Connected += OnServiceConnected;
        connectionService.Disconnected += OnServiceDisconnected;
        connectionService.ConnectionFailed += OnServiceConnectionFailed;
        connectionService.FramebufferUpdated += OnServiceFramebufferUpdated;

        ConnectCommand = new RelayCommandAsync(ExecuteShowConnectDialogAsync, CanConnect);
        DisconnectCommand = new RelayCommandAsync(ExecuteDisconnectAsync, CanDisconnect);
        RetryCommand = new RelayCommandAsync(ExecuteRetryAsync, CanRetry);
        SendClipboardCommand = new RelayCommandAsync(ExecuteSendClipboardAsync, CanSendClipboard);
    }

    /// <summary>
    /// Gets the command that connects again to the last server, with the same port and password.
    /// </summary>
    public RelayCommandAsync RetryCommand { get; }

    /// <summary>
    /// Gets the command that sends the local clipboard text to the server, when <see cref="ShareClipboard"/> is on.
    /// </summary>
    public RelayCommandAsync SendClipboardCommand { get; }

    /// <summary>
    /// Gets a value indicating whether the last connection failed or was lost.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets how the image is stretched for <see cref="ScaleMode"/>.
    /// </summary>
    public Stretch Stretch
        => ScaleMode switch
        {
            VncScaleMode.Fill => Stretch.UniformToFill,
            VncScaleMode.Original => Stretch.None,
            _ => Stretch.Uniform,
        };

    /// <summary>
    /// Gets a value indicating whether the view scrolls the screen (<see cref="VncScaleMode.Original"/>).
    /// </summary>
    public bool IsScrollable
        => ScaleMode == VncScaleMode.Original;

    /// <summary>
    /// Gets or sets how server clipboard text is put on the local clipboard; by default <c>Clipboard.SetText</c>.
    /// </summary>
    public Action<string> ClipboardSetter { get; set; } = SetClipboardText;

    /// <summary>
    /// Gets or sets how the local clipboard text is read; by default <c>Clipboard.GetText</c>.
    /// </summary>
    public Func<string?> ClipboardGetter { get; set; } = GetClipboardText;

    /// <summary>
    /// Occurs when <see cref="CurrentFrame"/> has been updated; raised once per rendered batch of rectangles.
    /// </summary>
    public event EventHandler? FramebufferUpdated;

    /// <summary>
    /// Gets the command that shows the connect dialog and connects to the entered VNC server.
    /// </summary>
    public RelayCommandAsync ConnectCommand { get; }

    /// <summary>
    /// Gets the command that disconnects from the VNC server.
    /// </summary>
    public RelayCommandAsync DisconnectCommand { get; }

    /// <summary>
    /// Gets the bitmap holding the current remote screen image, or <see langword="null"/> when not connected.
    /// </summary>
    public WriteableBitmap? CurrentFrame
        => connectionService.CurrentFrame;

    /// <summary>
    /// Gets the width, in pixels, of the remote framebuffer, or 0 when not connected.
    /// </summary>
    public int FramebufferWidth
        => connectionService.FramebufferWidth;

    /// <summary>
    /// Gets the height, in pixels, of the remote framebuffer, or 0 when not connected.
    /// </summary>
    public int FramebufferHeight
        => connectionService.FramebufferHeight;

    /// <summary>
    /// Connects to a VNC server, marking the view model busy until the attempt completes.
    /// </summary>
    /// <param name="host">The host name or IP address of the VNC server.</param>
    /// <param name="port">The TCP port of the VNC server.</param>
    /// <param name="password">The optional password used for authentication.</param>
    /// <returns>A task that completes when the connection attempt has finished.</returns>
    [SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "Properties must be set before the await.")]
    public async Task ConnectAsync(
        string host,
        int port,
        string? password)
    {
        lastHost = host;
        lastPort = port;
        lastPassword = password;
        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = Resources.Resources.VncConnecting;
        RaiseCommandsCanExecuteChanged();

        await connectionService.ConnectAsync(host, port, password).ConfigureAwait(true);
    }

    /// <summary>
    /// Disconnects from the VNC server, marking the view model busy until it completes.
    /// </summary>
    /// <returns>A task that completes when the connection has been closed.</returns>
    [SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "Properties must be set before the await.")]
    public async Task DisconnectAsync()
    {
        IsBusy = true;
        isDisconnecting = true;
        StatusMessage = Resources.Resources.VncDisconnecting;
        RaiseCommandsCanExecuteChanged();

        try
        {
            await connectionService.DisconnectAsync().ConfigureAwait(true);
        }
        finally
        {
            isDisconnecting = false;
        }
    }

    /// <summary>
    /// Sends a pointer (mouse) event to the VNC server.
    /// </summary>
    /// <param name="buttonMask">The pressed-buttons bit mask (1 = left, 2 = middle, 4 = right).</param>
    /// <param name="x">The horizontal framebuffer coordinate.</param>
    /// <param name="y">The vertical framebuffer coordinate.</param>
    /// <returns>A task that completes when the event has been sent.</returns>
    public Task SendPointerEventAsync(
        byte buttonMask,
        int x,
        int y)
        => ViewOnly
            ? Task.CompletedTask
            : connectionService.SendPointerEventAsync(buttonMask, x, y);

    /// <summary>
    /// Sends a key press or release event to the VNC server.
    /// </summary>
    /// <param name="keySym">The X11 keysym of the key.</param>
    /// <param name="pressed"><see langword="true"/> for a key press; <see langword="false"/> for a key release.</param>
    /// <returns>A task that completes when the event has been sent.</returns>
    public Task SendKeyEventAsync(
        uint keySym,
        bool pressed)
        => ViewOnly
            ? Task.CompletedTask
            : connectionService.SendKeyEventAsync(keySym, pressed);

    /// <inheritdoc />
    public void Dispose()
    {
        connectionService.Connected -= OnServiceConnected;
        connectionService.Disconnected -= OnServiceDisconnected;
        connectionService.ConnectionFailed -= OnServiceConnectionFailed;
        connectionService.FramebufferUpdated -= OnServiceFramebufferUpdated;
        connectionService.ClipboardTextReceived -= OnServiceClipboardTextReceived;
        connectionService.Dispose();
        lastPassword = null;
    }

    private bool CanConnect()
        => !IsBusy && !IsConnected;

    private bool CanDisconnect()
        => !IsBusy && IsConnected;

    private bool CanRetry()
        => !IsBusy && !IsConnected && lastHost is not null;

    private Task ExecuteRetryAsync()
        => ConnectAsync(lastHost!, lastPort, lastPassword);

    private bool CanSendClipboard()
        => ShareClipboard && IsConnected;

    private Task ExecuteSendClipboardAsync()
    {
        var text = ClipboardGetter();
        return string.IsNullOrEmpty(text)
            ? Task.CompletedTask
            : connectionService.SendClipboardTextAsync(text);
    }

    private void RaiseCommandsCanExecuteChanged()
    {
        ConnectCommand.RaiseCanExecuteChanged();
        DisconnectCommand.RaiseCanExecuteChanged();
        RetryCommand.RaiseCanExecuteChanged();
        SendClipboardCommand.RaiseCanExecuteChanged();
    }

    private void OnServiceClipboardTextReceived(
        object? sender,
        VncClipboardEventArgs e)
    {
        if (!ShareClipboard)
        {
            return;
        }

        _ = dispatcher.BeginInvoke(() => ClipboardSetter(e.Text));
    }

    private static void SetClipboardText(string text)
        => ClipboardHelper.TrySetText(text);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Another program can hold the clipboard; nothing is sent then.")]
    private static string? GetClipboardText()
    {
        try
        {
            return System.Windows.Clipboard.ContainsText()
                ? System.Windows.Clipboard.GetText()
                : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task ExecuteShowConnectDialogAsync()
    {
        var ownerWindow = Application.Current?.MainWindow;
        if (ownerWindow is null)
        {
            return;
        }

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateConnectDialogControls());

        var dialogBox = new InputFormDialogBox(
            ownerWindow,
            Resources.Resources.VncConnectDialogTitle,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult is not true)
        {
            return;
        }

        var (host, port, password) = ReadConnectDialogResult(dialogBox.Data.GetKeyValues());
        await ConnectAsync(host, port, password).ConfigureAwait(true);
    }

    // The Name gives each field a stable identifier, so the result is read the same in every UI language.
    internal static List<ILabelControlBase> CreateConnectDialogControls()
        =>
        [
            new LabelTextBox
            {
                Name = HostKey,
                LabelText = Resources.Resources.VncHost,
                IsMandatory = true,
                MinLength = 1,
            },
            new LabelIntegerBox
            {
                Name = PortKey,
                LabelText = Resources.Resources.VncPort,
                IsMandatory = true,
                Value = 5900,
                Minimum = 1,
                Maximum = 65535,
            },
            new LabelPasswordBox
            {
                Name = PasswordKey,
                LabelText = Resources.Resources.VncPassword,
            },
        ];

    internal static (string Host, int Port, string? Password) ReadConnectDialogResult(
        Dictionary<string, object> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var host = data[HostKey]?.ToString() ?? string.Empty;
        var port = System.Convert.ToInt32(data[PortKey], CultureInfo.InvariantCulture);
        var password = data.TryGetValue(PasswordKey, out var value) ? value?.ToString() : null;
        return (host, port, password);
    }

    private Task ExecuteDisconnectAsync()
        => DisconnectAsync();

    // The service raises its events from the receive thread after ConfigureAwait(false), so the
    // state is posted to the UI thread without waiting for it. A synchronous failure (the client
    // answered at once) arrives on the UI thread already and is applied before ConnectAsync returns.
    private void OnUiThread(Action update)
    {
        if (dispatcher.CheckAccess())
        {
            update();
        }
        else
        {
            _ = dispatcher.BeginInvoke(update);
        }
    }

    private void OnServiceConnected(
        object? sender,
        EventArgs e)
    {
        OnUiThread(() =>
        {
            IsConnected = true;
            IsBusy = false;
            ErrorMessage = null;
            StatusMessage = Resources.Resources.VncConnected;
            RaiseCommandsCanExecuteChanged();
        });
    }

    private void OnServiceDisconnected(
        object? sender,
        EventArgs e)
    {
        OnUiThread(() =>
        {
            IsConnected = false;
            IsBusy = false;
            StatusMessage = Resources.Resources.VncConnectionLost;
            if (!isDisconnecting)
            {
                ErrorMessage = Resources.Resources.VncConnectionLost;
            }

            RaiseCommandsCanExecuteChanged();
        });
    }

    private void OnServiceConnectionFailed(
        object? sender,
        VncConnectionFailedEventArgs e)
    {
        OnUiThread(() =>
        {
            IsConnected = false;
            IsBusy = false;
            StatusMessage = e.Message;
            ErrorMessage = e.Message;
            RaiseCommandsCanExecuteChanged();
        });
    }

    private void OnServiceFramebufferUpdated(
        object? sender,
        EventArgs e)
    {
        FramebufferUpdated?.Invoke(this, EventArgs.Empty);
    }
}