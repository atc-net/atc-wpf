// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelDisplayPicker : ILabelDisplayPicker
{
    [DependencyProperty]
    private DisplayInfo? value;

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

    public event EventHandler<ValueChangedEventArgs<DisplayInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<DisplayInfo?>>? LostFocusInvalid;

    public LabelDisplayPicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<DisplayInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<DisplayInfo?> e)
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
        if (e.PropertyName != nameof(DisplayInfo.State))
        {
            return;
        }

        Validate(
            this,
            new RoutedPropertyChangedEventArgs<DisplayInfo?>(Value, Value),
            raiseEvents: false);
    }

    private static void Validate(
        LabelDisplayPicker control,
        RoutedPropertyChangedEventArgs<DisplayInfo?> e,
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
        LabelDisplayPicker control,
        RoutedPropertyChangedEventArgs<DisplayInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<DisplayInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelDisplayPicker control,
        RoutedPropertyChangedEventArgs<DisplayInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<DisplayInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}