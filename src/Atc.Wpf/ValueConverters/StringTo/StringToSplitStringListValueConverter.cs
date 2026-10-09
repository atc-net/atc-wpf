// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String To List of Strings.
/// </summary>
[ValueConversion(typeof(string), typeof(List<string>))]
public sealed class StringToSplitStringListValueConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="StringToSplitStringListValueConverter"/>.
    /// </summary>
    public static readonly StringToSplitStringListValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the character used to split the string and to join the list back.
    /// </summary>
    public char Separator { get; set; } = ';';

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is string stringValue
            ? stringValue
                .Split([Separator], StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList()
            : value;

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is IList<string> list
            ? string.Join(Separator.ToString(), list)
            : value;
}