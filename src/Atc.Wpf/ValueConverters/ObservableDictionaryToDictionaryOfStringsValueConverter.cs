// ReSharper disable MergeIntoLogicalPattern
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ObservableDictionary To Dictionary.
/// </summary>
[ValueConversion(typeof(ObservableDictionary<object, string>), typeof(Dictionary<string, string>))]
public sealed class ObservableDictionaryToDictionaryOfStringsValueConverter : IValueConverter
{
    public static readonly ObservableDictionaryToDictionaryOfStringsValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        if (value is not ObservableDictionary<string, string> &&
            value is not ObservableDictionary<int, string> &&
            value is not ObservableDictionary<Guid, string>)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(ObservableDictionary<object, string>)");
        }

        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
        switch (value)
        {
            case ObservableDictionary<string, string> observableDictionaryString:
            {
                dictionary = observableDictionaryString.ToDictionaryOfStrings();
                break;
            }

            case ObservableDictionary<int, string> observableDictionaryInt:
            {
                dictionary = observableDictionaryInt.ToDictionaryOfStrings();
                break;
            }

            case ObservableDictionary<Guid, string> observableDictionaryGuid:
            {
                observableDictionaryGuid.ToDictionaryOfStrings();
                break;
            }
        }

        return dictionary;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}