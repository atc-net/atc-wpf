// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelAudioOutputPicker : ILabelAudioOutputPicker
{
    /// <summary>
    /// Identifies the <see cref="DeviceError"/> routed event.
    /// </summary>
    public static readonly RoutedEvent DeviceErrorEvent = DevicePickerEvents.DeviceErrorEvent.AddOwner(typeof(LabelAudioOutputPicker));

    /// <summary>
    /// Occurs when the picker inside this control could not start watching for device changes or could not read its device list.
    /// </summary>
    public event EventHandler<DeviceErrorRoutedEventArgs> DeviceError
    {
        add => AddHandler(DeviceErrorEvent, value);
        remove => RemoveHandler(DeviceErrorEvent, value);
    }

    [DependencyProperty]
    private AudioDeviceInfo? value;

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

    [DependencyProperty(DefaultValue = 120.0)]
    private double previewHeight;

    /// <summary>
    /// Occurs when the selection changes and passes validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<AudioDeviceInfo?>>? LostFocusValid;

    /// <summary>
    /// Occurs when the selection changes and fails validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<AudioDeviceInfo?>>? LostFocusInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelAudioOutputPicker"/> class.
    /// </summary>
    public LabelAudioOutputPicker()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        // A device in use only shows a note in the validation slot; it does not make the field invalid.
        return string.IsNullOrEmpty(ValidationText) || Value is { State: DeviceState.InUse };
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<AudioDeviceInfo?> e)
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
        if (e.PropertyName != nameof(AudioDeviceInfo.State))
        {
            return;
        }

        Validate(
            this,
            new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(Value, Value),
            raiseEvents: false);
    }

    private static void Validate(
        LabelAudioOutputPicker control,
        RoutedPropertyChangedEventArgs<AudioDeviceInfo?> e,
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
        LabelAudioOutputPicker control,
        RoutedPropertyChangedEventArgs<AudioDeviceInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<AudioDeviceInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelAudioOutputPicker control,
        RoutedPropertyChangedEventArgs<AudioDeviceInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<AudioDeviceInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}