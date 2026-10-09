namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="NetworkAdapterPicker"/> to UI Automation, including the value pattern for reading and setting the selected adapter.
/// </summary>
public class NetworkAdapterPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkAdapterPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public NetworkAdapterPickerAutomationPeer(NetworkAdapterPicker owner)
        : base(owner)
    {
    }

    private NetworkAdapterPicker NetworkAdapterPicker
        => (NetworkAdapterPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value
        => NetworkAdapterPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(NetworkAdapterPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "network adapter picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the adapter whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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