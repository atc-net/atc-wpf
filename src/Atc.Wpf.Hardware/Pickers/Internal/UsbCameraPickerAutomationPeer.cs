namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Exposes a <see cref="UsbCameraPicker"/> to UI Automation, including the value pattern for reading and setting the selected camera.
/// </summary>
public class UsbCameraPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbCameraPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The picker this peer exposes.</param>
    public UsbCameraPickerAutomationPeer(UsbCameraPicker owner)
        : base(owner)
    {
    }

    private UsbCameraPicker UsbCameraPicker => (UsbCameraPicker)Owner;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => UsbCameraPicker.Value?.FriendlyName ?? string.Empty;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(UsbCameraPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "usb camera picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <summary>
    /// Selects the camera whose device ID or friendly name matches <paramref name="value"/>; an empty value clears the selection.
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
            UsbCameraPicker.SetCurrentValue(UsbCameraPicker.ValueProperty, null);
            return;
        }

        var match = UsbCameraPicker.Cameras.FirstOrDefault(c =>
            string.Equals(c.DeviceId, value, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.FriendlyName, value, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            UsbCameraPicker.SetCurrentValue(UsbCameraPicker.ValueProperty, match);
        }
    }
}