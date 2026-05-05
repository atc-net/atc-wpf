namespace Atc.Wpf.Hardware.Pickers.Internal;

public class PrinterPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public PrinterPickerAutomationPeer(PrinterPicker owner)
        : base(owner)
    {
    }

    private PrinterPicker PrinterPicker => (PrinterPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => PrinterPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(PrinterPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "printer picker";

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
            PrinterPicker.SetCurrentValue(PrinterPicker.ValueProperty, null);
            return;
        }

        var match = PrinterPicker.Printers.FirstOrDefault(p =>
            string.Equals(p.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            PrinterPicker.SetCurrentValue(PrinterPicker.ValueProperty, match);
        }
    }
}