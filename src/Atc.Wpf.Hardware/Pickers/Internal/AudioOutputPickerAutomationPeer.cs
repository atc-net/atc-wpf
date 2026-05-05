namespace Atc.Wpf.Hardware.Pickers.Internal;

public class AudioOutputPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public AudioOutputPickerAutomationPeer(AudioOutputPicker owner)
        : base(owner)
    {
    }

    private AudioOutputPicker AudioOutputPicker => (AudioOutputPicker)Owner;

    public bool IsReadOnly => false;

    public string Value
        => AudioOutputPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(AudioOutputPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "audio output picker";

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