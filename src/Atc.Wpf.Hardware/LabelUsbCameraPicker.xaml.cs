// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelUsbCameraPicker : ILabelUsbCameraPicker
{
    [DependencyProperty]
    private UsbCameraInfo? value;

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
        typeof(LabelUsbCameraPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public UsbCameraFormat? PreferredFormat
    {
        get => (UsbCameraFormat?)GetValue(PreferredFormatProperty);
        set => SetValue(PreferredFormatProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<UsbCameraInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<UsbCameraInfo?>>? LostFocusInvalid;

    public LabelUsbCameraPicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
    {
        if (e.OldValue is not null)
        {
            e.OldValue.PropertyChanged -= OnValueStatePropertyChanged;
        }

        if (e.NewValue is not null)
        {
            e.NewValue.PropertyChanged += OnValueStatePropertyChanged;
        }

        Validate(this, e, raiseEvents: true);
    }

    /// <summary>
    /// Re-validates when the selected device's <see cref="DeviceState"/> changes
    /// (e.g. it transitions to <see cref="DeviceState.InUse"/> after an in-use probe,
    /// or to <see cref="DeviceState.Disconnected"/> when the device is unplugged).
    /// Without this hook, the validation text only refreshes when a different
    /// device is selected.
    /// </summary>
    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UsbCameraInfo.State))
        {
            return;
        }

        Validate(
            this,
            new RoutedPropertyChangedEventArgs<UsbCameraInfo?>(Value, Value),
            raiseEvents: false);
    }

    private static void Validate(
        LabelUsbCameraPicker control,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e,
        bool raiseEvents)
    {
        control.ValidationText = string.Empty;

        if (control is { IsMandatory: true, Value: null })
        {
            control.ValidationText = Validations.DeviceIsRequired;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Value is { State: DeviceState.Disconnected })
        {
            control.ValidationText = Validations.DeviceNoLongerAvailable;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Value is { State: DeviceState.InUse })
        {
            control.ValidationText = Miscellaneous.DeviceInUse;
            return;
        }

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelUsbCameraPicker control,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<UsbCameraInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelUsbCameraPicker control,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<UsbCameraInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}