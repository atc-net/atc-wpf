namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly BoolToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not bool boolValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
        }

        return boolValue
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not Visibility visibility)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Visibility)");
        }

        return visibility == Visibility.Collapsed;
    }
}