namespace Atc.Wpf.Hardware.Pickers.Internal;

public class NetworkAdapterPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public NetworkAdapterPickerAutomationPeer(NetworkAdapterPicker owner)
        : base(owner)
    {
    }

    private NetworkAdapterPicker NetworkAdapterPicker
        => (NetworkAdapterPicker)Owner;

    public bool IsReadOnly => false;

    public string Value
        => NetworkAdapterPicker.Value?.FriendlyName ?? string.Empty;

    protected override string GetClassNameCore()
        => nameof(NetworkAdapterPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "network adapter picker";

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
            NetworkAdapterPicker.SetCurrentValue(NetworkAdapterPicker.ValueProperty, null);
            return;
        }

        var match = NetworkAdapterPicker.Adapters.FirstOrDefault(a =>
            string.Equals(a.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            NetworkAdapterPicker.SetCurrentValue(NetworkAdapterPicker.ValueProperty, match);
        }
    }
}