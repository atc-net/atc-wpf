// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String To List of Strings.
/// </summary>
[ValueConversion(typeof(string), typeof(List<string>))]
public sealed class StringToSplitStringListValueConverter : IValueConverter
{
    public static readonly StringToSplitStringListValueConverter Instance = new();

    public char Separator { get; set; } = ';';

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is string stringValue
            ? stringValue.Split([Separator], StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList()
            : value;

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is IList<string> list)
        {
            return string.Join(Separator.ToString(), list);
        }

        return value;
    }
}