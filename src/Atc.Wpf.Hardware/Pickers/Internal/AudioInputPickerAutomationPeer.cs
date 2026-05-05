namespace Atc.Wpf.Hardware.Pickers.Internal;

public class AudioInputPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public AudioInputPickerAutomationPeer(AudioInputPicker owner)
        : base(owner)
    {
    }

    private AudioInputPicker AudioInputPicker => (AudioInputPicker)Owner;

    public bool IsReadOnly => false;

    public string Value => AudioInputPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(AudioInputPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "audio input picker";

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
            AudioInputPicker.SetCurrentValue(AudioInputPicker.ValueProperty, null);
            return;
        }

        var match = AudioInputPicker.Devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(d.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            AudioInputPicker.SetCurrentValue(AudioInputPicker.ValueProperty, match);
        }
    }
}