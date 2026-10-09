namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="WindowPicker"/> to UI Automation, including the value pattern for reading and setting the selected window.
/// </summary>
public class WindowPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public WindowPickerAutomationPeer(WindowPicker owner)
        : base(owner)
    {
    }

    private WindowPicker WindowPicker => (WindowPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => WindowPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(WindowPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "window picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the window whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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
            WindowPicker.SetCurrentValue(WindowPicker.ValueProperty, null);
            return;
        }

        var match = WindowPicker.Windows.FirstOrDefault(w =>
            string.Equals(w.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(w.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            WindowPicker.SetCurrentValue(WindowPicker.ValueProperty, match);
        }
    }
}