// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Inverse Bool.
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public sealed class BoolToInverseBoolValueConverter : IValueConverter
{
    public static readonly BoolToInverseBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Convert(value);

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Convert(value);

    private static bool Convert(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not bool boolValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
        }

        return !boolValue;
    }
}