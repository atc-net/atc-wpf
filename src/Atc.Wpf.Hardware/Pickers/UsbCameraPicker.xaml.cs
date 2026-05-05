// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Hardware.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class UsbCameraPicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent valueChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent deviceLost;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<UsbCameraInfo?>))]
    private static readonly RoutedEvent deviceReconnected;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(EventHandler<DeviceStateChangedRoutedEventArgs>))]
    private static readonly RoutedEvent deviceStateChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(UsbCameraInfo),
        typeof(UsbCameraPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    public UsbCameraInfo? Value
    {
        get => (UsbCameraInfo?)GetValue(ValueProperty);
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

    [DependencyProperty(DefaultValue = false)]
    private bool showLivePreview;

    [DependencyProperty(DefaultValue = 240.0)]
    private double previewHeight;

    public static readonly DependencyProperty PreferredFormatProperty = DependencyProperty.Register(
        nameof(PreferredFormat),
        typeof(UsbCameraFormat),
        typeof(UsbCameraPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public UsbCameraFormat? PreferredFormat
    {
        get => (UsbCameraFormat?)GetValue(PreferredFormatProperty);
        set => SetValue(PreferredFormatProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(UsbCameraPicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(UsbCameraPicker),
        new PropertyMetadata(defaultValue: null));

    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    public static readonly DependencyProperty SelectedStateMessageProperty = DependencyProperty.Register(
        nameof(SelectedStateMessage),
        typeof(string),
        typeof(UsbCameraPicker),
        new PropertyMetadata(defaultValue: string.Empty, OnSelectedStateMessageChanged));

    public string SelectedStateMessage
    {
        get => (string)GetValue(SelectedStateMessageProperty);
        private set => SetValue(SelectedStateMessageProperty, value);
    }

    public static readonly DependencyProperty HasSelectedStateMessageProperty = DependencyProperty.Register(
        nameof(HasSelectedStateMessage),
        typeof(bool),
        typeof(UsbCameraPicker),
        new PropertyMetadata(defaultValue: false));

    public bool HasSelectedStateMessage
    {
        get => (bool)GetValue(HasSelectedStateMessageProperty);
        private set => SetValue(HasSelectedStateMessageProperty, value);
    }

    private readonly IUsbCameraService service;
    private readonly Dictionary<string, DeviceState> lastKnownStates = new(StringComparer.OrdinalIgnoreCase);
    private string? lostDeviceId;

    public UsbCameraPicker()
        : this(new UsbCameraService())
    {
    }

    internal UsbCameraPicker(IUsbCameraService service)
    {
        this.service = service ?? throw new ArgumentNullException(nameof(service));

        InitializeComponent();

        PartLivePreview.FormatsAvailable += OnPreviewFormatsAvailable;

        Cameras = service.Cameras;
        ApplyResolvedItemTemplate();

        foreach (var camera in Cameras)
        {
            HookItem(camera);
        }

        Cameras.CollectionChanged += OnCamerasCollectionChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void HookItem(UsbCameraInfo camera)
    {
        lastKnownStates[camera.DeviceId] = camera.State;
        camera.PropertyChanged += OnCameraStatePropertyChanged;
    }

    private void UnhookItem(UsbCameraInfo camera)
    {
        camera.PropertyChanged -= OnCameraStatePropertyChanged;
        lastKnownStates.Remove(camera.DeviceId);
    }

    private void OnCameraStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UsbCameraInfo.State) ||
            sender is not UsbCameraInfo camera)
        {
            return;
        }

        var oldState = lastKnownStates.TryGetValue(camera.DeviceId, out var prev)
            ? prev
            : DeviceState.Unknown;

        if (oldState == camera.State)
        {
            return;
        }

        lastKnownStates[camera.DeviceId] = camera.State;

        RaiseEvent(new DeviceStateChangedRoutedEventArgs(
            DeviceStateChangedEvent,
            camera.DeviceId,
            oldState,
            camera.State));
    }

    public ObservableCollection<UsbCameraInfo> Cameras { get; }

    public IUsbCameraService Service => service;

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
            Debug.WriteLine($"UsbCameraPicker initial refresh failed: {ex.Message}");
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
            Debug.WriteLine($"UsbCameraPicker refresh failed: {ex.Message}");
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

        ((UsbCameraPicker)d).OnValueChanged((UsbCameraInfo?)e.OldValue, (UsbCameraInfo?)e.NewValue);
    }

    private void OnValueChanged(
        UsbCameraInfo? oldValue,
        UsbCameraInfo? newValue)
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
        RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(oldValue, newValue, ValueChangedEvent));
    }

    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UsbCameraInfo.State))
        {
            return;
        }

        UpdateSelectedStateMessage();
    }

    private static void OnSelectedStateMessageChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is UsbCameraPicker picker)
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

    private void OnCamerasCollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is UsbCameraInfo removed)
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
                if (item is not UsbCameraInfo info)
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
                    RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(null, info, DeviceReconnectedEvent));
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
        RaiseEvent(new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(lost, lost, DeviceLostEvent));

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