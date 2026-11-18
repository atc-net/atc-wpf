// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class AudioOutputPicker : IDevicePickerHost<AudioDeviceInfo>
{
    /// <summary>
    /// Occurs when the selected audio output device changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected audio output device is disconnected.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a previously lost audio output device reappears.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked audio output device changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly IAudioDeviceService service;
    private readonly DevicePickerController<AudioDeviceInfo> controller;

    /// <summary>
    /// Gets or sets the selected audio output device.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private AudioDeviceInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool showLivePreview;

    [DependencyProperty(DefaultValue = 120.0)]
    private double previewHeight;

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
    /// Initializes a new instance of the <see cref="AudioOutputPicker"/> class.
    /// </summary>
    public AudioOutputPicker()
        : this(new AudioDeviceService(AudioDeviceKind.Output))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioOutputPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public AudioOutputPicker(IAudioDeviceService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<AudioDeviceInfo>(
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
    /// Gets the audio output devices known to the picker's service.
    /// </summary>
    public ObservableCollection<AudioDeviceInfo> Devices { get; }

    /// <summary>
    /// Gets the service that enumerates and watches the audio output devices.
    /// </summary>
    public IAudioDeviceService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new AudioOutputPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is AudioOutputPicker picker)
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

        ((AudioOutputPicker)d).OnValueChanged((AudioDeviceInfo?)e.OldValue, (AudioDeviceInfo?)e.NewValue);
    }

    private void OnValueChanged(
        AudioDeviceInfo? oldValue,
        AudioDeviceInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is AudioOutputPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<AudioDeviceInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<AudioDeviceInfo>.RaiseValueChanged(
        AudioDeviceInfo? oldValue,
        AudioDeviceInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<AudioDeviceInfo>.RaiseDeviceLost(
        AudioDeviceInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<AudioDeviceInfo>.RaiseDeviceReconnected(
        AudioDeviceInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<AudioDeviceInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<AudioDeviceInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}