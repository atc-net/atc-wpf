namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="UsbPortPicker"/> to UI Automation, including the value pattern for reading and setting the selected USB device.
/// </summary>
public class UsbPortPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbPortPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public UsbPortPickerAutomationPeer(UsbPortPicker owner)
        : base(owner)
    {
    }

    private UsbPortPicker UsbPortPicker => (UsbPortPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => UsbPortPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(UsbPortPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "usb port picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the USB device whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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