namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Regex Validation.
/// </summary>
/// <remarks>
/// Validates a string value against a regex pattern provided in the converter parameter.
/// Returns <c>true</c> if the string matches the pattern, <c>false</c> otherwise.
/// Returns <c>false</c> for null/empty values or invalid regex patterns.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Validate email format --&gt;
/// IsValid="{Binding Email, Converter={x:Static converters:RegexValidationValueConverter.Instance}, ConverterParameter='^[\\w.-]+@[\\w.-]+\\.\\w+$'}"
///
/// &lt;!-- Validate phone number --&gt;
/// IsValid="{Binding Phone, Converter={x:Static converters:RegexValidationValueConverter.Instance}, ConverterParameter='^\\d{3}-\\d{3}-\\d{4}$'}"
/// </code>
/// </example>
[ValueConversion(typeof(string), typeof(bool))]
public sealed class RegexValidationValueConverter : IValueConverter
{
    public static readonly RegexValidationValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var stringValue = value?.ToString();
        var pattern = parameter?.ToString();

        if (string.IsNullOrEmpty(stringValue) || string.IsNullOrEmpty(pattern))
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(
                stringValue,
                pattern,
                RegexOptions.None,
                TimeSpan.FromSeconds(1));
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
#pragma warning restore CA1031 // Do not catch general exception types
        {
            // Invalid regex pattern
            return false;
        }
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}