// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: string-color-name To SolidColorBrush.
/// </summary>
[ValueConversion(typeof(string), typeof(Brush))]
public sealed class ColorNameToBrushValueConverter : IValueConverter
{
    public static readonly ColorNameToBrushValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return Brushes.DeepPink;
        }

        if (value is not string stringValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(string)");
        }

        var brush = SolidColorBrushHelper.GetBrushFromString(stringValue, GlobalizationConstants.EnglishCultureInfo);
        return brush ?? throw new InvalidCastException($"{stringValue} is not a valid brush");
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return "DeepPink";
        }

        if (value is not SolidColorBrush brush)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Brush)");
        }

        var brushName = SolidColorBrushHelper.GetBrushNameFromBrush(brush);

        return string.IsNullOrEmpty(brushName)
            ? value.ToString()!
            : brushName;
    }
}