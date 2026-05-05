namespace Atc.Wpf.Hardware.Pickers.Internal;

public class TimeZonePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public TimeZonePickerAutomationPeer(TimeZonePicker owner)
        : base(owner)
    {
    }

    private TimeZonePicker TimeZonePicker => (TimeZonePicker)Owner;

    public bool IsReadOnly => false;

    public string Value
        => TimeZonePicker.Value?.Id ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(TimeZonePicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "time zone picker";

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
            TimeZonePicker.SetCurrentValue(TimeZonePicker.ValueProperty, null);
            return;
        }

        var match = TimeZonePicker.TimeZones.FirstOrDefault(tz =>
            string.Equals(tz.Id, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(tz.DisplayName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            TimeZonePicker.SetCurrentValue(TimeZonePicker.ValueProperty, match);
        }
    }
}