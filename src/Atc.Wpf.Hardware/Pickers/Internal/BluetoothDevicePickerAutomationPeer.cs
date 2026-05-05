namespace Atc.Wpf.Hardware.Pickers.Internal;

public class BluetoothDevicePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public BluetoothDevicePickerAutomationPeer(BluetoothDevicePicker owner)
        : base(owner)
    {
    }

    private BluetoothDevicePicker BluetoothDevicePicker
        => (BluetoothDevicePicker)Owner;

    public bool IsReadOnly => false;

    public string Value
        => BluetoothDevicePicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(BluetoothDevicePicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "bluetooth device picker";

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
            BluetoothDevicePicker.SetCurrentValue(BluetoothDevicePicker.ValueProperty, null);
            return;
        }

        var match = BluetoothDevicePicker.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(d.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            BluetoothDevicePicker.SetCurrentValue(BluetoothDevicePicker.ValueProperty, match);
        }
    }
}