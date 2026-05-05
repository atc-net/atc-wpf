namespace Atc.Wpf.Hardware.Pickers.Internal;

public class SerialPortPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public SerialPortPickerAutomationPeer(SerialPortPicker owner)
        : base(owner)
    {
    }

    private SerialPortPicker SerialPortPicker => (SerialPortPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => SerialPortPicker.Value?.PortName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(SerialPortPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "serial port picker";

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