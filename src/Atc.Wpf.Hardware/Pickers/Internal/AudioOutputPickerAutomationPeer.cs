namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes an <see cref="AudioOutputPicker"/> to UI Automation, including the value pattern for reading and setting the selected device.
/// </summary>
public class AudioOutputPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioOutputPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public AudioOutputPickerAutomationPeer(AudioOutputPicker owner)
        : base(owner)
    {
    }

    private AudioOutputPicker AudioOutputPicker => (AudioOutputPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value
        => AudioOutputPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(AudioOutputPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "audio output picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the device whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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
            AudioOutputPicker.SetCurrentValue(AudioOutputPicker.ValueProperty, null);
            return;
        }

        var match = AudioOutputPicker.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(d.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            AudioOutputPicker.SetCurrentValue(AudioOutputPicker.ValueProperty, match);
        }
    }
}