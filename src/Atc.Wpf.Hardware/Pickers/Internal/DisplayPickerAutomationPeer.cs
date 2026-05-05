namespace Atc.Wpf.Hardware.Pickers.Internal;

public class DisplayPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public DisplayPickerAutomationPeer(DisplayPicker owner)
        : base(owner)
    {
    }

    private DisplayPicker DisplayPicker => (DisplayPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => DisplayPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(DisplayPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "display picker";

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
            DisplayPicker.SetCurrentValue(DisplayPicker.ValueProperty, null);
            return;
        }

        var match = DisplayPicker.Displays.FirstOrDefault(m =>
            string.Equals(m.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(m.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            DisplayPicker.SetCurrentValue(DisplayPicker.ValueProperty, match);
        }
    }
}