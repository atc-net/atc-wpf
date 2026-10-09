// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class DisplayPicker : IDevicePickerHost<DisplayInfo>
{
    /// <summary>
    /// Occurs when the selected display changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<DisplayInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Occurs when the selected display is unplugged.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<DisplayInfo?>))]
    private static readonly RoutedEvent deviceLost;

    /// <summary>
    /// Occurs when a previously lost display reappears.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<DisplayInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    /// <summary>
    /// Occurs when the state of any tracked display changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(DisplayInfo),
        typeof(DisplayPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    /// <summary>
    /// Gets or sets the selected display.
    /// </summary>
    public DisplayInfo? Value
    {
        get => (DisplayInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showRefreshButton;

    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnAutoRefreshOnDeviceChangeChanged))]
    private bool autoRefreshOnDeviceChange;

    [DependencyProperty(DefaultValue = false)]
    private bool clearValueOnDisconnect;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRebindOnReconnect;

    [DependencyProperty(DefaultValue = false)]
    private bool autoSelectFirstAvailable;

    /// <summary>
    /// Gets or sets how often the display list is polled. <see langword="null"/> (default) keeps the service's own setting;
    /// a value is pushed to the service immediately.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnPollingIntervalChanged))]
    private TimeSpan? pollingInterval;

    /// <summary>
    /// Identifies the <see cref="ItemTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(DisplayPicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    /// <summary>
    /// Gets or sets the template used to render each display; when <see langword="null"/> the default template is used.
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ResolvedItemTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(DisplayPicker),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Gets the item template in effect: <see cref="ItemTemplate"/> when set, otherwise the default template.
    /// </summary>
    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SelectedStateMessage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedStateMessageProperty = DependencyProperty.Register(
        nameof(SelectedStateMessage),
        typeof(string),
        typeof(DisplayPicker),
        new PropertyMetadata(defaultValue: string.Empty, OnSelectedStateMessageChanged));

    /// <summary>
    /// Gets the state message for the selected display (for example "Disconnected"), or an empty string.
    /// </summary>
    public string SelectedStateMessage
    {
        get => (string)GetValue(SelectedStateMessageProperty);
        private set => SetValue(SelectedStateMessageProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="HasSelectedStateMessage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(HasSelectedStateMessage),
        typeof(bool),
        typeof(DisplayPicker),
        new PropertyMetadata(defaultValue: false));

    /// <summary>
    /// Gets a value indicating whether <see cref="SelectedStateMessage"/> is not empty.
    /// </summary>
    public bool HasSelectedStateMessage
    {
        get => (bool)GetValue(HasSelectedStateMessageProperty);
        private set => SetValue(HasSelectedStateMessageProperty, value);
    }

    /// <summary>
    /// When <c>true</c> (default) the selected device's state ("In use", "Disconnected")
    /// is shown inline below the ComboBox. Set to <c>false</c> when the picker is hosted
    /// inside a labelled wrapper that surfaces the message via <c>ValidationText</c> —
    /// otherwise the inline message grows the picker's height and pushes neighbouring
    /// controls down.
    /// </summary>
    public static readonly DependencyProperty ShowSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(ShowSelectedStateMessage),
        typeof(bool),
        typeof(DisplayPicker),
        new PropertyMetadata(defaultValue: true));

    /// <summary>
    /// Gets or sets a value indicating whether the selected display's state message is shown inline below the drop-down.
    /// </summary>
    public bool ShowSelectedStateMessage
    {
        get => (bool)GetValue(ShowSelectedStateMessageProperty);
        set => SetValue(ShowSelectedStateMessageProperty, value);
    }

    private readonly IDisplayService service;
    private readonly DevicePickerController<DisplayInfo> controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayPicker"/> class.
    /// </summary>
    public DisplayPicker()
        : this(new DisplayService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayPicker"/> class on an existing service,
    /// e.g. to share one device watcher between several pickers or to supply a test double.
    /// The picker does not dispose the service.
    /// </summary>
    public DisplayPicker(IDisplayService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        // Created before InitializeComponent: a style setter applied during initialization can already
        // raise property-changed callbacks that use the controller.
        controller = new DevicePickerController<DisplayInfo>(
            this,
            service.Displays,
            service.StartWatching,
            service.StopWatching,
            service.RefreshAsync);

        InitializeComponent();

        Displays = service.Displays;
        ApplyResolvedItemTemplate();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the displays known to the picker's service.
    /// </summary>
    public ObservableCollection<DisplayInfo> Displays { get; }

    /// <summary>
    /// Gets the service that enumerates and monitors the displays.
    /// </summary>
    public IDisplayService Service => service;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DisplayPickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DisplayPicker picker)
        {
            picker.ApplyResolvedItemTemplate();
        }
    }

    private static void OnAutoRefreshOnDeviceChangeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((DisplayPicker)d).controller.AutoRefreshOnDeviceChangeChanged();

    private static void OnPollingIntervalChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is TimeSpan value)
        {
            ((DisplayPicker)d).service.PollingInterval = value;
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

        ((DisplayPicker)d).OnValueChanged((DisplayInfo?)e.OldValue, (DisplayInfo?)e.NewValue);
    }

    private void OnValueChanged(
        DisplayInfo? oldValue,
        DisplayInfo? newValue)
        => controller.ValueChanged(oldValue, newValue);

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DisplayPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    void IDevicePickerHost<DisplayInfo>.SetSelectedStateMessage(string message)
        => SelectedStateMessage = message;

    void IDevicePickerHost<DisplayInfo>.RaiseValueChanged(
        DisplayInfo? oldValue,
        DisplayInfo? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<DisplayInfo?>(oldValue, newValue, ValueChangedEvent));

    void IDevicePickerHost<DisplayInfo>.RaiseDeviceLost(DisplayInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<DisplayInfo?>(device, device, DeviceLostEvent));

    void IDevicePickerHost<DisplayInfo>.RaiseDeviceReconnected(
        DisplayInfo device)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<DisplayInfo?>(oldValue: null, device, DeviceReconnectedEvent));

    void IDevicePickerHost<DisplayInfo>.RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        => RaiseEvent(new DeviceStateChangedRoutedEventArgs(DeviceStateChangedEvent, deviceId, oldState, newState));
}