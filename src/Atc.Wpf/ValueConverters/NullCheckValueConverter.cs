// ReSharper disable InvertIf
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// NullCheckValueConverter.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]

public sealed class NullCheckValueConverter : IValueConverter
{
    public static readonly NullCheckValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value ?? parameter;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}