// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
[UseDependencyPropertyTemplate(typeof(DevicePickerMembers))]
public partial class WindowPicker : IDevicePickerHost<TopLevelWindowInfo>
{
    /// <summary>
    /// Occurs when the selected window changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<TopLevelWindowInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected window is destroyed.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<TopLevelWindowInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a lost window reappears with the same window handle.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<TopLevelWindowInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked window changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    private readonly IWindowService service;
    private readonly DevicePickerController<TopLevelWindowInfo> controller;

    /// <summary>
    /// Gets or sets the selected window.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private TopLevelWindowInfo? value;

    /// <summary>
    /// Gets or sets how often the window list is polled. <see langword="null"/> (default) keeps the service's own setting;
    /// a value is pushed to the service immediately.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnPollingIntervalChanged))]
    private TimeSpan? pollingInterval;

    /// <summary>
    /// Gets or sets whether only visible windows with a title are listed. <see langword="null"/> (default) keeps the service's own setting;
    /// a value is pushed to the service immediately.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnOnlyVisibleWithTitleChanged))]
    private bool? onlyVisibleWithTitle;

    /// <summary>
    /// Gets or sets the template used to display each window; when <see langword="null"/> the default template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the state message for the selected window, or an empty string when there is none.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnSelectedStateMessageChanged),
        SetterAccessModifier = PropertyAccessModifier.Private)]
    private string selectedStateMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the selected window's state message is shown inline below the drop-down.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectedStateMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowPicker"/> class.
    /// </summary>
    public WindowPicker()
        : this(new WindowService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public WindowPicker(IWindowService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<TopLevelWindowInfo>(
            this,
            service.Windows,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync,
            watchOnlyWhileOpen: true);

        InitializeComponent();

        // Listing every process/window is expensive, so the list is polled only while it is open.
        PartComboBox.DropDownOpened += OnDropDownOpened;
        PartComboBox.DropDownClosed += OnDropDownClosed;

        Windows = service.Windows;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the top-level windows known to the picker's service.
    /// </summary>
    public ObservableCollection<TopLevelWindowInfo> Windows { get; }

    /// <summary>
    /// Gets the service that enumerates and monitors the top-level windows.
    /// </summary>
    public IWindowService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new WindowPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowPicker picker)
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
            ((WindowPicker)d).service.PollingInterval = value;
        }
    }

    private static void OnOnlyVisibleWithTitleChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool value)
        {
            ((WindowPicker)d).service.OnlyVisibleWithTitle = value;
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

        ((WindowPicker)d).OnValueChanged((TopLevelWindowInfo?)e.OldValue, (TopLevelWindowInfo?)e.NewValue);
    }

    private void OnValueChanged(
        TopLevelWindowInfo? oldValue,
        TopLevelWindowInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<TopLevelWindowInfo>.SetSelectedStateMessage(
        string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<TopLevelWindowInfo>.RaiseValueChanged(
        TopLevelWindowInfo? oldValue,
        TopLevelWindowInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<TopLevelWindowInfo>.RaiseDeviceLost(
        TopLevelWindowInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<TopLevelWindowInfo>.RaiseDeviceReconnected(
        TopLevelWindowInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<TopLevelWindowInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));

    void IDevicePickerHost<TopLevelWindowInfo>.RaiseDeviceError(
        Exception exception,
        DevicePickerOperation operation)
        => RaiseEvent(new DeviceErrorRoutedEventArgs(DeviceErrorEvent, exception, operation));
}