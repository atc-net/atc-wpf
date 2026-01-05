namespace Atc.Wpf.Controls.BaseControls.Internal;

public class ColorPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public ColorPickerAutomationPeer(ColorPicker owner)
        : base(owner)
    {
    }

    private ColorPicker ColorPicker => (ColorPicker)Owner;

    protected override string GetClassNameCore()
        => nameof(ColorPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "color picker";

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    public bool IsReadOnly => false;

    public string Value => ColorPicker.DisplayHexCode ?? string.Empty;

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
        catch (FormatException)
        {
            throw new ArgumentException($"Invalid color value: {value}", nameof(value));
        }
    }
}
