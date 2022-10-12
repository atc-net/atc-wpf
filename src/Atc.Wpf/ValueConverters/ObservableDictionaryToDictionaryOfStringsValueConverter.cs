// ReSharper disable MergeIntoLogicalPattern
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ObservableDictionary To Dictionary.
/// </summary>
[ValueConversion(typeof(ObservableDictionary<object, string>), typeof(Dictionary<string, string>))]
public class ObservableDictionaryToDictionaryOfStringsValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
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
                foreach (var item in observableDictionaryString)
                {
                    dictionary.Add(item.Key, item.Value);
                }

                break;
            }

            case ObservableDictionary<int, string> observableDictionaryInt:
            {
                foreach (var item in observableDictionaryInt)
                {
                    dictionary.Add(item.Key.ToString(GlobalizationConstants.EnglishCultureInfo), item.Value);
                }

                break;
            }

            case ObservableDictionary<Guid, string> observableDictionaryGuid:
            {
                foreach (var item in observableDictionaryGuid)
                {
                    dictionary.Add(item.Key.ToString(), item.Value);
                }

                break;
            }
        }

        return dictionary;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}