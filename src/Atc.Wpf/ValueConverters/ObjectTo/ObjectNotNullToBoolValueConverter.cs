// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Bool.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class ObjectNotNullToBoolValueConverter : IValueConverter
{
    public static readonly ObjectNotNullToBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}