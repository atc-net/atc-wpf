// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class AudioOutputPicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent valueChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent deviceLost;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<AudioDeviceInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(AudioDeviceInfo),
        typeof(AudioOutputPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    public AudioDeviceInfo? Value
    {
        get => (AudioDeviceInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showRefreshButton;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRefreshOnDeviceChange;

    [DependencyProperty(DefaultValue = false)]
    private bool clearValueOnDisconnect;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRebindOnReconnect;

    [DependencyProperty(DefaultValue = false)]
    private bool autoSelectFirstAvailable;

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(AudioOutputPicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(AudioOutputPicker),
        new PropertyMetadata(defaultValue: null));

    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    public static readonly DependencyProperty SelectedStateMessageProperty = DependencyProperty.Register(
        nameof(SelectedStateMessage),
        typeof(string),
        typeof(AudioOutputPicker),
        new PropertyMetadata(defaultValue: string.Empty, OnSelectedStateMessageChanged));

    public string SelectedStateMessage
    {
        get => (string)GetValue(SelectedStateMessageProperty);
        private set => SetValue(SelectedStateMessageProperty, value);
    }

    public static readonly DependencyProperty HasSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(HasSelectedStateMessage),
        typeof(bool),
        typeof(AudioOutputPicker),
        new PropertyMetadata(defaultValue: false));

    public bool HasSelectedStateMessage
    {
        get => (bool)GetValue(HasSelectedStateMessageProperty);
        private set => SetValue(HasSelectedStateMessageProperty, value);
    }

    private readonly IAudioDeviceService service;
    private readonly Dictionary<string, DeviceState> lastKnownStates = new(StringComparer.OrdinalIgnoreCase);
    private string? lostDeviceId;

    public AudioOutputPicker()
        : this(new AudioDeviceService(AudioDeviceKind.Output))
    {
    }

    internal AudioOutputPicker(IAudioDeviceService service)
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

    private void HookItem(AudioDeviceInfo device)
    {
        lastKnownStates[device.DeviceId] = device.State;
        device.PropertyChanged += OnDeviceStatePropertyChanged;
    }

    private void UnhookItem(AudioDeviceInfo device)
    {
        device.PropertyChanged -= OnDeviceStatePropertyChanged;
        lastKnownStates.Remove(device.DeviceId);
    }

    private void OnDeviceStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(AudioDeviceInfo.State) ||
            sender is not AudioDeviceInfo device)
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

    public ObservableCollection<AudioDeviceInfo> Devices { get; }

    public IAudioDeviceService Service => service;

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
            Debug.WriteLine($"AudioOutputPicker initial refresh failed: {ex.Message}");
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
            Debug.WriteLine($"AudioOutputPicker refresh failed: {ex.Message}");
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

        ((AudioOutputPicker)d).OnValueChanged((AudioDeviceInfo?)e.OldValue, (AudioDeviceInfo?)e.NewValue);
    }

    private void OnValueChanged(
        AudioDeviceInfo? oldValue,
        AudioDeviceInfo? newValue)
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
        RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(oldValue, newValue, ValueChangedEvent));
    }

    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(AudioDeviceInfo.State))
        {
            return;
        }

        UpdateSelectedStateMessage();
    }

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is AudioOutputPicker picker)
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
                if (item is AudioDeviceInfo removed)
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
                if (item is not AudioDeviceInfo info)
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
                    RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(null, info, DeviceReconnectedEvent));
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
        RaiseEvent(new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(lost, lost, DeviceLostEvent));

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