// ReSharper disable once CheckNamespace
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace System.Windows.Media;

public static class SolidColorBrushExtensions
{
    // ReSharper disable once UnusedParameter.Global
    public static SolidColorBrush GetFromHex(
        this SolidColorBrush solidColorBrush,
        string hexValue)
    {
        ArgumentNullException.ThrowIfNull(solidColorBrush);
        ArgumentNullException.ThrowIfNull(hexValue);

        if (!hexValue.StartsWith('#') &&
            hexValue.Length is 3 or 6 or 8)
        {
            hexValue = $"#{hexValue}";
        }
        else if (!hexValue.StartsWith('#'))
        {
            throw new ArgumentException("It is not hex value", nameof(hexValue));
        }

        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexValue));
    }
}