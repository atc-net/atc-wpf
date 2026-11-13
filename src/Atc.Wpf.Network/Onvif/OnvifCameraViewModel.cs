namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// ViewModel for reading one ONVIF camera: its model and its RTSP streams.
/// </summary>
public partial class OnvifCameraViewModel : ViewModelBase
{
    private readonly IOnvifCameraReader reader;

    /// <summary>
    /// Gets or sets the ONVIF device service URL.
    /// </summary>
    [ObservableProperty]
    private string? deviceServiceUrl;

    private NetworkCredential? readCredential;
    private Uri? readUrl;

    /// <summary>
    /// Gets or sets the user name of the camera's ONVIF account.
    /// </summary>
    [ObservableProperty]
    private string? userName;

    /// <summary>
    /// Gets or sets the password of the camera's ONVIF account. It is never logged or shown.
    /// </summary>
    [ObservableProperty]
    private string? password;

    /// <summary>
    /// Gets or sets the manufacturer.
    /// </summary>
    [ObservableProperty]
    private string? manufacturer;

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    [ObservableProperty]
    private string? model;

    /// <summary>
    /// Gets or sets the firmware version.
    /// </summary>
    [ObservableProperty]
    private string? firmwareVersion;

    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    [ObservableProperty]
    private string? serialNumber;

    /// <summary>
    /// Gets or sets the selected stream.
    /// </summary>
    [ObservableProperty]
    private OnvifStreamInfo? selectedStream;

    /// <summary>
    /// Gets or sets the error message of the last read.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnvifCameraViewModel"/> class that reads cameras with <c>OnvifClient</c>.
    /// </summary>
    public OnvifCameraViewModel()
        : this(new OnvifCameraReader())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnvifCameraViewModel"/> class.
    /// </summary>
    /// <param name="reader">Reads the camera.</param>
    public OnvifCameraViewModel(IOnvifCameraReader reader)
        => this.reader = reader;

    /// <summary>
    /// Occurs when the user chooses a stream.
    /// </summary>
    public event EventHandler<OnvifStreamSelectedEventArgs>? StreamSelected;

    /// <summary>
    /// Gets a value indicating whether the last read failed.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets the camera's streams.
    /// </summary>
    public ObservableCollection<OnvifStreamInfo> Streams { get; } = [];

    /// <summary>
    /// Gets the camera's network interfaces.
    /// </summary>
    public ObservableCollection<OnvifNetworkInterface> NetworkInterfaces { get; } = [];

    /// <summary>
    /// Takes the ONVIF device service URL of a host found by the network scanner.
    /// </summary>
    /// <param name="host">The host.</param>
    public void UseHost(NetworkHostViewModel host)
    {
        ArgumentNullException.ThrowIfNull(host);

        DeviceServiceUrl = host.OnvifDeviceServiceUrl?.ToString();
    }

    private bool CanRead()
        => TryGetDeviceServiceUrl(out _);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanRead))]
    private async Task Read(CancellationToken cancellationToken)
    {
        ClearResult();
        if (!TryGetDeviceServiceUrl(out var url))
        {
            return;
        }

        IsBusy = true;
        var credential = new NetworkCredential(UserName ?? string.Empty, Password ?? string.Empty);
        try
        {
            var details = await reader.Read(url, credential, cancellationToken).ConfigureAwait(true);

            Manufacturer = details.Information.Manufacturer;
            Model = details.Information.Model;
            FirmwareVersion = details.Information.FirmwareVersion;
            SerialNumber = details.Information.SerialNumber;
            foreach (var stream in details.Streams)
            {
                Streams.Add(stream);
            }

            foreach (var networkInterface in details.NetworkInterfaces)
            {
                NetworkInterfaces.Add(networkInterface);
            }

            SelectedStream = Streams.FirstOrDefault();
            readUrl = url;
            readCredential = credential;
        }
        catch (OperationCanceledException)
        {
            // Cancelled by the user; nothing to show.
        }
        catch (OnvifException ex) when (ex.IsNotAuthorized)
        {
            ErrorMessage = Resources.Resources.OnvifNotAuthorized;
        }
        catch (TimeoutException)
        {
            ErrorMessage = Resources.Resources.OnvifTimeout;
        }
        catch (Exception ex)
        {
            // Atc.Network never puts the password in its exceptions.
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.OnvifErrorFormat1, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanUseStream()
        => SelectedStream is not null && readUrl is not null && readCredential is not null;

    [RelayCommand(CanExecute = nameof(CanUseStream))]
    private void UseStream()
        => StreamSelected?.Invoke(
            this,
            new OnvifStreamSelectedEventArgs(readUrl!, SelectedStream!, readCredential!, Manufacturer, Model));

    private bool CanCopyStreamUri()
        => SelectedStream is not null;

    [RelayCommand(CanExecute = nameof(CanCopyStreamUri))]
    private void CopyStreamUri()
        => ClipboardHelper.TrySetText(SelectedStream!.StreamUri.ToString());

    private void ClearResult()
    {
        ErrorMessage = null;
        Manufacturer = null;
        Model = null;
        FirmwareVersion = null;
        SerialNumber = null;
        SelectedStream = null;
        Streams.Clear();
        NetworkInterfaces.Clear();
        readUrl = null;
        readCredential = null;
    }

    private bool TryGetDeviceServiceUrl(out Uri url)
    {
        if (Uri.TryCreate(DeviceServiceUrl, UriKind.Absolute, out var parsed) &&
            (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps))
        {
            url = parsed;
            return true;
        }

        url = null!;
        return false;
    }
}