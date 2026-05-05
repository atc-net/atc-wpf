namespace Atc.Wpf.Hardware.Pickers.Internal;

public class UsbCameraPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public UsbCameraPickerAutomationPeer(UsbCameraPicker owner)
        : base(owner)
    {
    }

    private UsbCameraPicker UsbCameraPicker => (UsbCameraPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => UsbCameraPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(UsbCameraPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "usb camera picker";

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            UsbCameraPicker.SetCurrentValue(UsbCameraPicker.ValueProperty, null);
            return;
        }

        var match = UsbCameraPicker.Cameras.FirstOrDefault(c =>
            string.Equals(c.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            UsbCameraPicker.SetCurrentValue(UsbCameraPicker.ValueProperty, match);
        }
    }
}