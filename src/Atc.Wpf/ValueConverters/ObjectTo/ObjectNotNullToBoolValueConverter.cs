// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Bool.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class ObjectNotNullToBoolValueConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="ObjectNotNullToBoolValueConverter"/>.
    /// </summary>
    public static readonly ObjectNotNullToBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is not null;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}