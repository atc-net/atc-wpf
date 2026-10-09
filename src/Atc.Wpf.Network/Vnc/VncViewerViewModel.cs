namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// View model for the <see cref="VncViewerView"/>, exposing connect/disconnect commands,
/// connection status and the current remote screen frame.
/// </summary>
public sealed partial class VncViewerViewModel : ViewModelBase, IDisposable
{
    private readonly VncConnectionService connectionService = new();

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="VncViewerViewModel"/> class.
    /// </summary>
    public VncViewerViewModel()
    {
        connectionService.Connected += OnServiceConnected;
        connectionService.Disconnected += OnServiceDisconnected;
        connectionService.ConnectionFailed += OnServiceConnectionFailed;
        connectionService.FramebufferUpdated += OnServiceFramebufferUpdated;

        ConnectCommand = new RelayCommandAsync(ExecuteShowConnectDialogAsync, CanConnect);
        DisconnectCommand = new RelayCommandAsync(ExecuteDisconnectAsync, CanDisconnect);
    }

    /// <summary>
    /// Occurs when a region of <see cref="CurrentFrame"/> has been updated.
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
        IsBusy = true;
        StatusMessage = Resources.Resources.VncConnecting;
        ConnectCommand.RaiseCanExecuteChanged();
        DisconnectCommand.RaiseCanExecuteChanged();

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
        StatusMessage = Resources.Resources.VncDisconnecting;
        ConnectCommand.RaiseCanExecuteChanged();
        DisconnectCommand.RaiseCanExecuteChanged();

        await connectionService.DisconnectAsync().ConfigureAwait(true);
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
        => connectionService.SendPointerEventAsync(buttonMask, x, y);

    /// <summary>
    /// Sends a key press or release event to the VNC server.
    /// </summary>
    /// <param name="keySym">The X11 keysym of the key.</param>
    /// <param name="pressed"><see langword="true"/> for a key press; <see langword="false"/> for a key release.</param>
    /// <returns>A task that completes when the event has been sent.</returns>
    public Task SendKeyEventAsync(
        uint keySym,
        bool pressed)
        => connectionService.SendKeyEventAsync(keySym, pressed);

    /// <inheritdoc />
    public void Dispose()
    {
        connectionService.Connected -= OnServiceConnected;
        connectionService.Disconnected -= OnServiceDisconnected;
        connectionService.ConnectionFailed -= OnServiceConnectionFailed;
        connectionService.FramebufferUpdated -= OnServiceFramebufferUpdated;
        connectionService.Dispose();
    }

    private bool CanConnect()
        => !IsBusy && !IsConnected;

    private bool CanDisconnect()
        => !IsBusy && IsConnected;

    private async Task ExecuteShowConnectDialogAsync()
    {
        var ownerWindow = Application.Current.MainWindow;
        if (ownerWindow is null)
        {
            return;
        }

        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = Resources.Resources.VncHost,
                IsMandatory = true,
                MinLength = 1,
            },
            new LabelIntegerBox
            {
                LabelText = Resources.Resources.VncPort,
                IsMandatory = true,
                Value = 5900,
                Minimum = 1,
                Maximum = 65535,
            },
            new LabelTextBox
            {
                LabelText = Resources.Resources.VncPassword,
            },
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            ownerWindow,
            Resources.Resources.VncConnectDialogTitle,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult is not true)
        {
            return;
        }

        var data = dialogBox.Data.GetKeyValues();
        var host = data[Resources.Resources.VncHost]?.ToString() ?? string.Empty;
        var port = System.Convert.ToInt32(data[Resources.Resources.VncPort], CultureInfo.InvariantCulture);
        var password = data[Resources.Resources.VncPassword]?.ToString();

        await ConnectAsync(host, port, password).ConfigureAwait(true);
    }

    private Task ExecuteDisconnectAsync()
        => DisconnectAsync();

    private void OnServiceConnected(
        object? sender,
        EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IsConnected = true;
            IsBusy = false;
            StatusMessage = Resources.Resources.VncConnected;
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        });
    }

    private void OnServiceDisconnected(
        object? sender,
        EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IsConnected = false;
            IsBusy = false;
            StatusMessage = Resources.Resources.VncConnectionLost;
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        });
    }

    private void OnServiceConnectionFailed(
        object? sender,
        VncConnectionFailedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IsConnected = false;
            IsBusy = false;
            StatusMessage = e.Message;
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        });
    }

    private void OnServiceFramebufferUpdated(
        object? sender,
        EventArgs e)
    {
        FramebufferUpdated?.Invoke(this, EventArgs.Empty);
    }
}