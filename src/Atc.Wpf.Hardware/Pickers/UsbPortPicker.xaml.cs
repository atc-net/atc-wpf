// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class UsbPortPicker : IDevicePickerHost<UsbDeviceInfo>
{
    /// <summary>
    /// Occurs when the selected USB device changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected USB device is disconnected.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a previously lost USB device reappears.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked USB device changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly IUsbDeviceService service;
    private readonly DevicePickerController<UsbDeviceInfo> controller;

    /// <summary>
    /// Gets or sets the selected USB device.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private UsbDeviceInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool detectInUseState;

    [DependencyProperty(DefaultValue = UsbDeviceClassFilter.None, PropertyChangedCallback = nameof(OnClassFilterChanged))]
    private UsbDeviceClassFilter classFilter;

    /// <summary>
    /// Gets or sets the template used to display each device; when <see langword="null"/> the default template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the state message for the selected device (for example "In use" or "Disconnected"), or an empty string.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnSelectedStateMessageChanged),
        SetterAccessModifier = PropertyAccessModifier.Private)]
    private string selectedStateMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the selected device's state message is shown inline below the drop-down.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectedStateMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbPortPicker"/> class.
    /// </summary>
    public UsbPortPicker()
        : this(new UsbDeviceService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbPortPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public UsbPortPicker(IUsbDeviceService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<UsbDeviceInfo>(
            this,
            service.Devices,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync);

        InitializeComponent();

        Devices = service.Devices;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the USB devices known to the picker's service.
    /// </summary>
    public ObservableCollection<UsbDeviceInfo> Devices { get; }

    /// <summary>
    /// Gets the service that enumerates and watches the USB devices.
    /// </summary>
    public IUsbDeviceService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new UsbPortPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbPortPicker picker)
        {
            picker.ApplyResolvedItemTemplate();
        }
    }

    partial void OnAutoRefreshOnDeviceChangeChanged(
        bool oldValue,
        bool newValue)
        => controller.AutoRefreshOnDeviceChangeChanged();

    private static void OnClassFilterChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is UsbDeviceClassFilter filter)
        {
            ((UsbPortPicker)d).service.ClassFilter = filter;
        }
    }

    private void ApplyResolvedItemTemplate()
        => ResolvedItemTemplate = ItemTemplate ?? (DataTemplate)Resources["DefaultItemTemplate"];

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        service.ClassFilter = ClassFilter;

        await controller.LoadedAsync();
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

        ((UsbPortPicker)d).OnValueChanged((UsbDeviceInfo?)e.OldValue, (UsbDeviceInfo?)e.NewValue);
    }

    private void OnValueChanged(
        UsbDeviceInfo? oldValue,
        UsbDeviceInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbPortPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<UsbDeviceInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<UsbDeviceInfo>.RaiseValueChanged(
        UsbDeviceInfo? oldValue,
        UsbDeviceInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<UsbDeviceInfo>.RaiseDeviceLost(UsbDeviceInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<UsbDeviceInfo>.RaiseDeviceReconnected(
        UsbDeviceInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<UsbDeviceInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<UsbDeviceInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}