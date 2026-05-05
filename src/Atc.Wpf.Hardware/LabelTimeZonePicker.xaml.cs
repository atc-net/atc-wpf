// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelTimeZonePicker : ILabelTimeZonePicker
{
    [DependencyProperty]
    private TimeZoneInfo? value;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    public event EventHandler<ValueChangedEventArgs<TimeZoneInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<TimeZoneInfo?>>? LostFocusInvalid;

    public LabelTimeZonePicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<TimeZoneInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<TimeZoneInfo?> e)
    {
        Validate(this, e, raiseEvents: true);
    }

    private static void Validate(
        LabelTimeZonePicker control,
        RoutedPropertyChangedEventArgs<TimeZoneInfo?> e,
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

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelTimeZonePicker control,
        RoutedPropertyChangedEventArgs<TimeZoneInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<TimeZoneInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelTimeZonePicker control,
        RoutedPropertyChangedEventArgs<TimeZoneInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<TimeZoneInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}