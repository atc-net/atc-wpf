namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="SerialPortPicker"/> to UI Automation, including the value pattern for reading and setting the selected port.
/// </summary>
public class SerialPortPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SerialPortPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public SerialPortPickerAutomationPeer(SerialPortPicker owner)
        : base(owner)
    {
    }

    private SerialPortPicker SerialPortPicker => (SerialPortPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => SerialPortPicker.Value?.PortName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(SerialPortPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "serial port picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the port whose port name or device ID matches <paramref name="value"/>; an empty value clears the selection.
    /// </summary>
    /// <param name="value">The port name (for example "COM3") or device ID to select.</param>
    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            SerialPortPicker.SetCurrentValue(SerialPortPicker.ValueProperty, null);
            return;
        }

        var match = SerialPortPicker.Ports.FirstOrDefault(p =>
            string.Equals(p.PortName, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p.DeviceId, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            SerialPortPicker.SetCurrentValue(SerialPortPicker.ValueProperty, match);
        }
    }
}