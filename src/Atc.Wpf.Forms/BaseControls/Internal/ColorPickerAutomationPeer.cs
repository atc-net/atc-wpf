namespace Atc.Wpf.Forms.BaseControls.Internal;

/// <summary>
/// Exposes a <see cref="ColorPicker"/> to UI Automation, including its color as a hex value through the Value pattern.
/// </summary>
public class ColorPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPickerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The color picker this peer represents.</param>
    public ColorPickerAutomationPeer(ColorPicker owner)
        : base(owner)
    {
    }

    private ColorPicker ColorPicker => (ColorPicker)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(ColorPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "color picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => ColorPicker.DisplayHexCode ?? string.Empty;

    /// <inheritdoc />
    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        try
        {
            var color = (Color)ColorConverter.ConvertFromString(value);
            ColorPicker.SetCurrentValue(ColorPicker.ColorValueProperty, color);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid color value: {value}", nameof(value), ex);
        }
    }
}