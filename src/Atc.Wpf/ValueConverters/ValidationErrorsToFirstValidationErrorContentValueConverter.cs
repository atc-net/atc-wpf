namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ValidationErrors To String.
/// </summary>
[ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(string))]
public sealed class ValidationErrorsToFirstValidationErrorContentValueConverter : IValueConverter
{
    public static readonly ValidationErrorsToFirstValidationErrorContentValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not ReadOnlyObservableCollection<ValidationError> errors || errors.Count == 0)
        {
            return null;
        }

        return errors[0].ErrorContent;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}