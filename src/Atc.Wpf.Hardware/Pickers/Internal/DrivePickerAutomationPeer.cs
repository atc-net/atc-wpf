namespace Atc.Wpf.Hardware.Pickers.Internal;

public class DrivePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public DrivePickerAutomationPeer(DrivePicker owner)
        : base(owner)
    {
    }

    private DrivePicker DrivePicker => (DrivePicker)Owner;

    public bool IsReadOnly => false;

    public string Value => DrivePicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(DrivePicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "drive picker";

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
            DrivePicker.SetCurrentValue(DrivePicker.ValueProperty, null);
            return;
        }

        var match = DrivePicker.Drives.FirstOrDefault(d =>
            string.Equals(d.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(d.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            DrivePicker.SetCurrentValue(DrivePicker.ValueProperty, match);
        }
    }
}