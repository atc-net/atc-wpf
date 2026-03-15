namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

internal sealed class IconItem
{
    public IconItem(
        string familyPrefix,
        object enumValue,
        string enumKey,
        string displayName,
        string toolTip)
    {
        FamilyPrefix = familyPrefix;
        EnumValue = enumValue;
        EnumKey = enumKey;
        DisplayName = displayName;
        ToolTip = toolTip;
    }

    public string FamilyPrefix { get; }

    public object EnumValue { get; }

    public string EnumKey { get; }

    public string DisplayName { get; }

    public string ToolTip { get; }
}
