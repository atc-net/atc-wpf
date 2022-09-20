namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityVisibleValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not bool boolValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
        }

        return boolValue
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not Visibility visibility)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Visibility)");
        }

        return visibility == Visibility.Visible;
    }
}