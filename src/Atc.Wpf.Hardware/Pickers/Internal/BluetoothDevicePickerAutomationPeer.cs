namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="BluetoothDevicePicker"/> to UI Automation, including the value pattern for reading and setting the selected device.
/// </summary>
public class BluetoothDevicePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothDevicePickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public BluetoothDevicePickerAutomationPeer(BluetoothDevicePicker owner)
        : base(owner)
    {
    }

    private BluetoothDevicePicker BluetoothDevicePicker
        => (BluetoothDevicePicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value
        => BluetoothDevicePicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(BluetoothDevicePicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "bluetooth device picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the device whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
    /// </summary>
    /// <param name="value">The device ID or friendly name to select.</param>
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