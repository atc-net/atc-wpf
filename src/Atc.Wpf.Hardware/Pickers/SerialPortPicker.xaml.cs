// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class SerialPortPicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent valueChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent deviceLost;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<SerialPortInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(SerialPortInfo),
        typeof(SerialPortPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    public SerialPortInfo? Value
    {
        get => (SerialPortInfo?)GetValue(ValueProperty);
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

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(SerialPortPicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(SerialPortPicker),
        new PropertyMetadata(defaultValue: null));

    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    public static readonly DependencyProperty SelectedStateMessageProperty = DependencyProperty.Register(
        nameof(SelectedStateMessage),
        typeof(string),
        typeof(SerialPortPicker),
        new PropertyMetadata(defaultValue: string.Empty, OnSelectedStateMessageChanged));

    public string SelectedStateMessage
    {
        get => (string)GetValue(SelectedStateMessageProperty);
        private set => SetValue(SelectedStateMessageProperty, value);
    }

    public static readonly DependencyProperty HasSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(HasSelectedStateMessage),
        typeof(bool),
        typeof(SerialPortPicker),
        new PropertyMetadata(defaultValue: false));

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
        typeof(SerialPortPicker),
        new PropertyMetadata(defaultValue: true));

    public bool ShowSelectedStateMessage
    {
        get => (bool)GetValue(ShowSelectedStateMessageProperty);
        set => SetValue(ShowSelectedStateMessageProperty, value);
    }

    private readonly ISerialPortService service;
    private readonly Dictionary<string, DeviceState> lastKnownStates = new(StringComparer.OrdinalIgnoreCase);
    private string? lostDeviceId;

    public SerialPortPicker()
        : this(new SerialPortService())
    {
    }

    internal SerialPortPicker(ISerialPortService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        InitializeComponent();

        Ports = service.Ports;
        ApplyResolvedItemTemplate();

        foreach (var port in Ports)
        {
            HookItem(port);
        }

        Ports.CollectionChanged += OnPortsCollectionChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void HookItem(SerialPortInfo port)
    {
        lastKnownStates[port.DeviceId] = port.State;
        port.PropertyChanged += OnPortStatePropertyChanged;
    }

    private void UnhookItem(SerialPortInfo port)
    {
        port.PropertyChanged -= OnPortStatePropertyChanged;
        lastKnownStates.Remove(port.DeviceId);
    }

    private void OnPortStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SerialPortInfo.State) ||
            sender is not SerialPortInfo port)
        {
            return;
        }

        var oldState = lastKnownStates.TryGetValue(port.DeviceId, out var prev)
            ? prev
            : DeviceState.Unknown;

        if (oldState == port.State)
        {
            return;
        }

        lastKnownStates[port.DeviceId] = port.State;

        RaiseEvent(new DeviceStateChangedRoutedEventArgs(
            DeviceStateChangedEvent,
            port.DeviceId,
            oldState,
            port.State));
    }

    public ObservableCollection<SerialPortInfo> Ports { get; }

    public ISerialPortService Service => service;

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

    private void ApplyResolvedItemTemplate()
        => ResolvedItemTemplate = ItemTemplate ?? (DataTemplate)Resources["DefaultItemTemplate"];

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "UI initialisation must not crash on hardware probe failure.")]
    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
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
            Debug.WriteLine($"SerialPortPicker initial refresh failed: {ex.Message}");
        }

        if (DetectInUseState && Value is not null)
        {
            await ProbeInUseAsync(Value);
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
            Debug.WriteLine($"SerialPortPicker refresh failed: {ex.Message}");
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

        ((SerialPortPicker)d).OnValueChanged((SerialPortInfo?)e.OldValue, (SerialPortInfo?)e.NewValue);
    }

    private void OnValueChanged(
        SerialPortInfo? oldValue,
        SerialPortInfo? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnValueStatePropertyChanged;
        }

        if (newValue is not null)
        {
            newValue.PropertyChanged += OnValueStatePropertyChanged;
            lostDeviceId = null;

            if (DetectInUseState)
            {
                _ = ProbeInUseAsync(newValue);
            }
        }

        UpdateSelectedStateMessage();
        RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(oldValue, newValue, ValueChangedEvent));
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

    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SerialPortInfo.State))
        {
            return;
        }

        UpdateSelectedStateMessage();
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

    private void OnPortsCollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is SerialPortInfo removed)
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
                if (item is not SerialPortInfo info)
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
                    RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(null, info, DeviceReconnectedEvent));
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
        RaiseEvent(new RoutedPropertyChangedEventArgs<SerialPortInfo?>(lost, lost, DeviceLostEvent));

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