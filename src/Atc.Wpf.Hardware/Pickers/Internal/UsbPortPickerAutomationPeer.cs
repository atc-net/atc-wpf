namespace Atc.Wpf.Hardware.Pickers.Internal;

public class UsbPortPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public UsbPortPickerAutomationPeer(UsbPortPicker owner)
        : base(owner)
    {
    }

    private UsbPortPicker UsbPortPicker => (UsbPortPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => UsbPortPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(UsbPortPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "usb port picker";

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
            UsbPortPicker.SetCurrentValue(UsbPortPicker.ValueProperty, null);
            return;
        }

        var match = UsbPortPicker.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(d.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            UsbPortPicker.SetCurrentValue(UsbPortPicker.ValueProperty, match);
        }
    }
}