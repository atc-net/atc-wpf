namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="ProcessPicker"/> to UI Automation, including the value pattern for reading and setting the selected process.
/// </summary>
public class ProcessPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public ProcessPickerAutomationPeer(ProcessPicker owner)
        : base(owner)
    {
    }

    private ProcessPicker ProcessPicker => (ProcessPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => ProcessPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(ProcessPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "process picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the process whose ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
    /// </summary>
    /// <param name="value">The process ID or friendly name to select.</param>
    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            ProcessPicker.SetCurrentValue(ProcessPicker.ValueProperty, null);
            return;
        }

        var match = ProcessPicker.Processes.FirstOrDefault(p =>
            string.Equals(p.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            ProcessPicker.SetCurrentValue(ProcessPicker.ValueProperty, match);
        }
    }
}