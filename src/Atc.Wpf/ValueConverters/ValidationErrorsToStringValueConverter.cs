namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ValidationErrors To String.
/// </summary>
[ValueConversion(typeof(ICollection<ValidationError>), typeof(string))]
public sealed class ValidationErrorsToStringValueConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new ValidationErrorsToStringValueConverter();
    }

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ICollection<ValidationError> errors
            ? string.Join('\n', (from e in errors select e.ErrorContent as string).ToArray())
            : string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}