// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelAudioInputPicker : ILabelAudioInputPicker
{
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

    public event EventHandler<ValueChangedEventArgs<AudioDeviceInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<AudioDeviceInfo?>>? LostFocusInvalid;

    public LabelAudioInputPicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<AudioDeviceInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<AudioDeviceInfo?> e)
    {
        Validate(this, e, raiseEvents: true);
    }

    private static void Validate(
        LabelAudioInputPicker control,
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

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelAudioInputPicker control,
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
        LabelAudioInputPicker control,
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