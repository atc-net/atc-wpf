// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String Null Or Empty To Bool.
/// </summary>
[ValueConversion(typeof(string), typeof(bool))]
public sealed class StringNullOrEmptyToBoolValueConverter : IValueConverter
{
    public static readonly StringNullOrEmptyToBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null || string.IsNullOrEmpty(value.ToString());
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}