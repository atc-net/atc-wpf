namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum-Description To String.
/// </summary>
public sealed class EnumDescriptionToStringValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not Enum enumValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not an enum type");
        }

        return EnumHelper.GetDescription(enumValue);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}