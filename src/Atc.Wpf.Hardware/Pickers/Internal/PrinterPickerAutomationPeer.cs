namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="PrinterPicker"/> to UI Automation, including the value pattern for reading and setting the selected printer.
/// </summary>
public class PrinterPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public PrinterPickerAutomationPeer(PrinterPicker owner)
        : base(owner)
    {
    }

    private PrinterPicker PrinterPicker => (PrinterPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => PrinterPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(PrinterPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "printer picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the printer whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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