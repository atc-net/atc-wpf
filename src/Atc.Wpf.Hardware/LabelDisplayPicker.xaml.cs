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
        Validate(this, e, raiseEvents: true);
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