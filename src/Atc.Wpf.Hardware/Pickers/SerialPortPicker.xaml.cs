// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class SerialPortPicker : IDevicePickerHost<SerialPortInfo>
{
    /// <summary>
    /// Occurs when the selected serial port changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected serial port is disconnected.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a previously lost serial port reappears.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked serial port changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly ISerialPortService service;
    private readonly DevicePickerController<SerialPortInfo> controller;

    /// <summary>
    /// Gets or sets the selected serial port.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private SerialPortInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool detectInUseState;

    /// <summary>
    /// Gets or sets the template used to display each port; when <see langword="null"/> the default template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the state message for the selected port (for example "In use" or "Disconnected"), or an empty string.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnSelectedStateMessageChanged),
        SetterAccessModifier = PropertyAccessModifier.Private)]
    private string selectedStateMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the selected port's state message is shown inline below the drop-down.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectedStateMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialPortPicker"/> class.
    /// </summary>
    public SerialPortPicker()
        : this(new SerialPortService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialPortPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public SerialPortPicker(ISerialPortService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<SerialPortInfo>(
            this,
            service.Ports,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync);

        InitializeComponent();

        Ports = service.Ports;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the serial ports known to the picker's service.
    /// </summary>
    public ObservableCollection<SerialPortInfo> Ports { get; }

    /// <summary>
    /// Gets the service that enumerates and watches the serial ports.
    /// </summary>
    public ISerialPortService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new SerialPortPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is SerialPortPicker picker)
        {
            picker.ApplyResolvedItemTemplate();
        }
    }

    partial void OnAutoRefreshOnDeviceChangeChanged(
        bool oldValue,
        bool newValue)
        => controller.AutoRefreshOnDeviceChangeChanged();

    private void ApplyResolvedItemTemplate()
        => ResolvedItemTemplate = ItemTemplate ?? (DataTemplate)Resources["DefaultItemTemplate"];

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        await controller.LoadedAsync();

        if (DetectInUseState && Value is not null)
        {
            await ProbeInUseAsync(Value);
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => controller.Unloaded();

    private async void OnRefreshClick(
        object sender,
        RoutedEventArgs e)
        => await controller.RefreshAsync();

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        ((SerialPortPicker)d).OnValueChanged((SerialPortInfo?)e.OldValue, (SerialPortInfo?)e.NewValue);
    }

    private void OnValueChanged(
        SerialPortInfo? oldValue,
        SerialPortInfo? newValue)
    {
        controller.ValueChanged(oldValue, newValue);

        if (DetectInUseState && newValue is not null)
        {
            _ = ProbeInUseAsync(newValue);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Probing is best-effort and must not crash the picker.")]
    private async Task ProbeInUseAsync(SerialPortInfo port)
    {
        try
        {
            var inUse = await service.ProbeInUseAsync(port);
            if (inUse && port.State is not DeviceState.Disconnected)
            {
                port.State = DeviceState.InUse;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SerialPortPicker probe failed for {port.PortName}: {ex.Message}");
        }
    }

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is SerialPortPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<SerialPortInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<SerialPortInfo>.RaiseValueChanged(
        SerialPortInfo? oldValue,
        SerialPortInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<SerialPortInfo>.RaiseDeviceLost(
        SerialPortInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<SerialPortInfo>.RaiseDeviceReconnected(
        SerialPortInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<SerialPortInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<SerialPortInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}