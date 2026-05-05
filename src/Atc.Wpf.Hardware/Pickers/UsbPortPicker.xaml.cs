// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class UsbPortPicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent valueChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent deviceLost;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbDeviceInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(UsbDeviceInfo),
        typeof(UsbPortPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    public UsbDeviceInfo? Value
    {
        get => (UsbDeviceInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showRefreshButton;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRefreshOnDeviceChange;

    [DependencyProperty(DefaultValue = false)]
    private bool detectInUseState;

    [DependencyProperty(DefaultValue = false)]
    private bool clearValueOnDisconnect;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRebindOnReconnect;

    [DependencyProperty(DefaultValue = false)]
    private bool autoSelectFirstAvailable;

    [DependencyProperty(DefaultValue = UsbDeviceClassFilter.None)]
    private UsbDeviceClassFilter classFilter;

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(UsbPortPicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(UsbPortPicker),
        new PropertyMetadata(defaultValue: null));

    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    public static readonly DependencyProperty SelectedStateMessageProperty = DependencyProperty.Register(
        nameof(SelectedStateMessage),
        typeof(string),
        typeof(UsbPortPicker),
        new PropertyMetadata(defaultValue: string.Empty, OnSelectedStateMessageChanged));

    public string SelectedStateMessage
    {
        get => (string)GetValue(SelectedStateMessageProperty);
        private set => SetValue(SelectedStateMessageProperty, value);
    }

    public static readonly DependencyProperty HasSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(HasSelectedStateMessage),
        typeof(bool),
        typeof(UsbPortPicker),
        new PropertyMetadata(defaultValue: false));

    public bool HasSelectedStateMessage
    {
        get => (bool)GetValue(HasSelectedStateMessageProperty);
        private set => SetValue(HasSelectedStateMessageProperty, value);
    }

    private readonly IUsbDeviceService service;
    private readonly Dictionary<string, DeviceState> lastKnownStates = new(StringComparer.OrdinalIgnoreCase);
    private string? lostDeviceId;

    public UsbPortPicker()
        : this(new UsbDeviceService())
    {
    }

    internal UsbPortPicker(IUsbDeviceService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        InitializeComponent();

        Devices = service.Devices;
        ApplyResolvedItemTemplate();

        foreach (var device in Devices)
        {
            HookItem(device);
        }

        Devices.CollectionChanged += OnDevicesCollectionChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void HookItem(UsbDeviceInfo device)
    {
        lastKnownStates[device.DeviceId] = device.State;
        device.PropertyChanged += OnDeviceStatePropertyChanged;
    }

    private void UnhookItem(UsbDeviceInfo device)
    {
        device.PropertyChanged -= OnDeviceStatePropertyChanged;
        lastKnownStates.Remove(device.DeviceId);
    }

    private void OnDeviceStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UsbDeviceInfo.State) ||
            sender is not UsbDeviceInfo device)
        {
            return;
        }

        var oldState = lastKnownStates.TryGetValue(device.DeviceId, out var prev)
            ? prev
            : DeviceState.Unknown;

        if (oldState == device.State)
        {
            return;
        }

        lastKnownStates[device.DeviceId] = device.State;

        RaiseEvent(new DeviceStateChangedRoutedEventArgs(
            DeviceStateChangedEvent,
            device.DeviceId,
            oldState,
            device.State));
    }

    public ObservableCollection<UsbDeviceInfo> Devices { get; }

    public IUsbDeviceService Service => service;

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

    private void ApplyResolvedItemTemplate()
        => ResolvedItemTemplate = ItemTemplate ?? (DataTemplate)Resources["DefaultItemTemplate"];

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "UI initialisation must not crash on hardware probe failure.")]
    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        service.ClassFilter = ClassFilter;

        if (AutoRefreshOnDeviceChange)
        {
            service.StartWatching();
        }

        try
        {
            await service.RefreshAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UsbPortPicker initial refresh failed: {ex.Message}");
        }

        UpdateSelectedStateMessage();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        service.StopWatching();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-initiated refresh must not crash on hardware probe failure.")]
    private async void OnRefreshClick(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await service.RefreshAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UsbPortPicker refresh failed: {ex.Message}");
        }
    }

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
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnValueStatePropertyChanged;
        }

        if (newValue is not null)
        {
            newValue.PropertyChanged += OnValueStatePropertyChanged;
            lostDeviceId = null;
        }

        UpdateSelectedStateMessage();
        RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(oldValue, newValue, ValueChangedEvent));
    }

    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UsbDeviceInfo.State))
        {
            return;
        }

        UpdateSelectedStateMessage();
    }

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbPortPicker picker)
        {
            picker.HasSelectedStateMessage = !string.IsNullOrEmpty((string?)e.NewValue);
        }
    }

    private void UpdateSelectedStateMessage()
    {
        if (Value is null)
        {
            SelectedStateMessage = string.Empty;
            return;
        }

        SelectedStateMessage = Value.State switch
        {
            DeviceState.Disconnected => Miscellaneous.DeviceDisconnected,
            DeviceState.InUse => Miscellaneous.DeviceInUse,
            _ => string.Empty,
        };
    }

    private void OnDevicesCollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is UsbDeviceInfo removed)
                {
                    UnhookItem(removed);
                }
            }
        }

        if (e.Action is System.Collections.Specialized.NotifyCollectionChangedAction.Add &&
            e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is not UsbDeviceInfo info)
                {
                    continue;
                }

                HookItem(info);

                if (AutoRebindOnReconnect &&
                    Value is null &&
                    !string.IsNullOrEmpty(lostDeviceId) &&
                    string.Equals(info.DeviceId, lostDeviceId, StringComparison.OrdinalIgnoreCase))
                {
                    Value = info;
                    RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(null, info, DeviceReconnectedEvent));
                    lostDeviceId = null;
                }
                else if (AutoSelectFirstAvailable &&
                    Value is null &&
                    info.State is DeviceState.Available or DeviceState.JustConnected)
                {
                    Value = info;
                }
            }
        }

        if (Value is not null && Value.State is DeviceState.Disconnected)
        {
            HandleSelectedDeviceLost();
        }
    }

    private void HandleSelectedDeviceLost()
    {
        var lost = Value;
        if (lost is null)
        {
            return;
        }

        lostDeviceId = lost.DeviceId;
        RaiseEvent(new RoutedPropertyChangedEventArgs<UsbDeviceInfo?>(lost, lost, DeviceLostEvent));

        if (ClearValueOnDisconnect)
        {
            Value = null;
        }
        else
        {
            UpdateSelectedStateMessage();
        }
    }
}