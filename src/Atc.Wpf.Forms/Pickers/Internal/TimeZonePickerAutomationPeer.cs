namespace Atc.Wpf.Forms.Pickers.Internal;

/// <summary>
/// Exposes a <c>TimeZonePicker</c> to UI Automation, including the Value pattern for reading and setting the selected time zone by id or display name.
/// </summary>
public class TimeZonePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZonePickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The time zone picker this peer represents.</param>
    public TimeZonePickerAutomationPeer(TimeZonePicker owner)
        : base(owner)
    {
    }

    private TimeZonePicker TimeZonePicker => (TimeZonePicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value
        => TimeZonePicker.Value?.Id ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(TimeZonePicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "time zone picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
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