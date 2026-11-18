// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class UsbCameraPicker : IDevicePickerHost<UsbCameraInfo>
{
    /// <summary>
    /// Occurs when the selected camera changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected camera is disconnected.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a previously lost camera reappears.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked camera changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly IUsbCameraService service;
    private readonly DevicePickerController<UsbCameraInfo> controller;

    /// <summary>
    /// Gets or sets the selected camera.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private UsbCameraInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool showLivePreview;

    [DependencyProperty(DefaultValue = 240.0)]
    private double previewHeight;

    /// <summary>
    /// Gets or sets the preferred resolution and frame rate for the live preview; when no exact match exists the device default is used.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private UsbCameraFormat? preferredFormat;

    /// <summary>
    /// Gets or sets the template used to display each camera; when <see langword="null"/> the default template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the state message for the selected camera (for example "In use" or "Disconnected"), or an empty string.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnSelectedStateMessageChanged),
        SetterAccessModifier = PropertyAccessModifier.Private)]
    private string selectedStateMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the selected camera's state message is shown inline below the drop-down.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectedStateMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbCameraPicker"/> class.
    /// </summary>
    public UsbCameraPicker()
        : this(new UsbCameraService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbCameraPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public UsbCameraPicker(IUsbCameraService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<UsbCameraInfo>(
            this,
            service.Cameras,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync);

        InitializeComponent();

        PartLivePreview.FormatsAvailable += OnPreviewFormatsAvailable;

        Cameras = service.Cameras;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the cameras known to the picker's service.
    /// </summary>
    public ObservableCollection<UsbCameraInfo> Cameras { get; }

    /// <summary>
    /// Gets the service that enumerates and watches the cameras.
    /// </summary>
    public IUsbCameraService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new UsbCameraPickerAutomationPeer(this);

    private void OnPreviewFormatsAvailable(
        object? sender,
        CameraFormatsAvailableEventArgs e)
    {
        if (Value is not null)
        {
            Value.SupportedFormats = e.Formats;
        }
    }

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbCameraPicker picker)
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
        => await controller.LoadedAsync();

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

        ((UsbCameraPicker)d).OnValueChanged((UsbCameraInfo?)e.OldValue, (UsbCameraInfo?)e.NewValue);
    }

    private void OnValueChanged(
        UsbCameraInfo? oldValue,
        UsbCameraInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbCameraPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<UsbCameraInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<UsbCameraInfo>.RaiseValueChanged(
        UsbCameraInfo? oldValue,
        UsbCameraInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<UsbCameraInfo>.RaiseDeviceLost(UsbCameraInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<UsbCameraInfo>.RaiseDeviceReconnected(
        UsbCameraInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<UsbCameraInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<UsbCameraInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}