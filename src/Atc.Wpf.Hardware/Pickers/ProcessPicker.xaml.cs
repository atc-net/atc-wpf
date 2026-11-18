// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class ProcessPicker : IDevicePickerHost<RunningProcessInfo>
{
    /// <summary>
    /// Occurs when the selected process changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<RunningProcessInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected process exits.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<RunningProcessInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a lost process reappears with the same process ID.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<RunningProcessInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked process changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly IProcessService service;
    private readonly DevicePickerController<RunningProcessInfo> controller;

    /// <summary>
    /// Gets or sets the selected process.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private RunningProcessInfo? value;

    /// <summary>
    /// Gets or sets how often the process list is polled. <see langword="null"/> (default) keeps the service's own setting;
    /// a value is pushed to the service immediately.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnPollingIntervalChanged))]
    private TimeSpan? pollingInterval;

    /// <summary>
    /// Gets or sets whether only processes with a main window are listed. <see langword="null"/> (default) keeps the service's own setting;
    /// a value is pushed to the service immediately.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnOnlyWithMainWindowChanged))]
    private bool? onlyWithMainWindow;

    /// <summary>
    /// Gets or sets the template used to display each process; when <see langword="null"/> the default template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the state message for the selected process, or an empty string when there is none.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnSelectedStateMessageChanged),
        SetterAccessModifier = PropertyAccessModifier.Private)]
    private string selectedStateMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the selected process's state message is shown inline below the drop-down.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectedStateMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessPicker"/> class.
    /// </summary>
    public ProcessPicker()
        : this(new ProcessService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public ProcessPicker(IProcessService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<RunningProcessInfo>(
            this,
            service.Processes,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync,
            watchOnlyWhileOpen: true);

        InitializeComponent();

        // Listing every process/window is expensive, so the list is polled only while it is open.
        PartComboBox.DropDownOpened += OnDropDownOpened;
        PartComboBox.DropDownClosed += OnDropDownClosed;

        Processes = service.Processes;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the running processes known to the picker's service.
    /// </summary>
    public ObservableCollection<RunningProcessInfo> Processes { get; }

    /// <summary>
    /// Gets the service that enumerates and monitors the running processes.
    /// </summary>
    public IProcessService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new ProcessPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ProcessPicker picker)
        {
            picker.ApplyResolvedItemTemplate();
        }
    }

    partial void OnAutoRefreshOnDeviceChangeChanged(
        bool oldValue,
        bool newValue)
        => controller.AutoRefreshOnDeviceChangeChanged();

    private static void OnPollingIntervalChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is TimeSpan value)
        {
            ((ProcessPicker)d).service.PollingInterval = value;
        }
    }

    private static void OnOnlyWithMainWindowChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool value)
        {
            ((ProcessPicker)d).service.OnlyWithMainWindow = value;
        }
    }

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

    private async void OnDropDownOpened(
        object? sender,
        EventArgs e)
        => await controller.DropDownOpenedAsync();

    private void OnDropDownClosed(
        object? sender,
        EventArgs e)
        => controller.DropDownClosed();

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

        ((ProcessPicker)d).OnValueChanged((RunningProcessInfo?)e.OldValue, (RunningProcessInfo?)e.NewValue);
    }

    private void OnValueChanged(
        RunningProcessInfo? oldValue,
        RunningProcessInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ProcessPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<RunningProcessInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<RunningProcessInfo>.RaiseValueChanged(
        RunningProcessInfo? oldValue,
        RunningProcessInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<RunningProcessInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<RunningProcessInfo>.RaiseDeviceLost(
        RunningProcessInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<RunningProcessInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<RunningProcessInfo>.RaiseDeviceReconnected(
        RunningProcessInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<RunningProcessInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<RunningProcessInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<RunningProcessInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}