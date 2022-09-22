// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Controls.ValueConverters;

[SuppressMessage("Design", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Design", "S112:General exceptions should never be thrown", Justification = "OK.")]
public sealed class JArrayLengthValueConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is not JToken jToken)
        {
            throw new Exception("Wrong type for this converter");
        }

        switch (jToken.Type)
        {
            case JTokenType.Array:
                var arrayLen = jToken.Children().Count();
                return $"[{arrayLen.ToString(GlobalizationConstants.EnglishCultureInfo)}]";
            case JTokenType.Property:
                var propertyArrayLen = jToken.Children().FirstOrDefault()?.Children().Count() ?? 0;
                return $"[ {propertyArrayLen.ToString(GlobalizationConstants.EnglishCultureInfo)} ]";
            default:
                throw new Exception("Type should be JProperty or JArray");
        }
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}