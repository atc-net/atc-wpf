namespace Atc.Wpf.Hardware.Pickers.Internal;

public class WindowPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public WindowPickerAutomationPeer(WindowPicker owner)
        : base(owner)
    {
    }

    private WindowPicker WindowPicker => (WindowPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => WindowPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(WindowPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "window picker";

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
            WindowPicker.SetCurrentValue(WindowPicker.ValueProperty, null);
            return;
        }

        var match = WindowPicker.Windows.FirstOrDefault(w =>
            string.Equals(w.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(w.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            WindowPicker.SetCurrentValue(WindowPicker.ValueProperty, match);
        }
    }
}