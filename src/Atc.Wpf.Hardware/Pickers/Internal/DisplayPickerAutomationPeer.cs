namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="DisplayPicker"/> to UI Automation, including the value pattern for reading and setting the selected display.
/// </summary>
public class DisplayPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public DisplayPickerAutomationPeer(DisplayPicker owner)
        : base(owner)
    {
    }

    private DisplayPicker DisplayPicker => (DisplayPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => DisplayPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(DisplayPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "display picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the display whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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