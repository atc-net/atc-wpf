namespace Atc.Wpf.Network.Vnc;

public sealed partial class VncViewerViewModel : ViewModelBase, IDisposable
{
    private readonly VncConnectionService connectionService = new();

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public VncViewerViewModel()
    {
        connectionService.Connected += OnServiceConnected;
        connectionService.Disconnected += OnServiceDisconnected;
        connectionService.ConnectionFailed += OnServiceConnectionFailed;
        connectionService.FramebufferUpdated += OnServiceFramebufferUpdated;

        ConnectCommand = new RelayCommandAsync(ExecuteShowConnectDialogAsync, CanConnect);
        DisconnectCommand = new RelayCommandAsync(ExecuteDisconnectAsync, CanDisconnect);
    }

    public event EventHandler? FramebufferUpdated;

    public RelayCommandAsync ConnectCommand { get; }

    public RelayCommandAsync DisconnectCommand { get; }

    public WriteableBitmap? CurrentFrame
        => connectionService.CurrentFrame;

    public int FramebufferWidth
        => connectionService.FramebufferWidth;

    public int FramebufferHeight
        => connectionService.FramebufferHeight;

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

    [SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "Properties must be set before the await.")]
    public async Task DisconnectAsync()
    {
        IsBusy = true;
        StatusMessage = Resources.Resources.VncDisconnecting;
        ConnectCommand.RaiseCanExecuteChanged();
        DisconnectCommand.RaiseCanExecuteChanged();

        await connectionService.DisconnectAsync().ConfigureAwait(true);
    }

    public Task SendPointerEventAsync(
        byte buttonMask,
        int x,
        int y)
        => connectionService.SendPointerEventAsync(buttonMask, x, y);

    public Task SendKeyEventAsync(
        uint keySym,
        bool pressed)
        => connectionService.SendKeyEventAsync(keySym, pressed);

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