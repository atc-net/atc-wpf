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
        Validate(this, e, raiseEvents: true);
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