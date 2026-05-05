namespace Atc.Wpf.Hardware.Pickers.Internal;

public class ProcessPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public ProcessPickerAutomationPeer(ProcessPicker owner)
        : base(owner)
    {
    }

    private ProcessPicker ProcessPicker => (ProcessPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => ProcessPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(ProcessPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "process picker";

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