// ReSharper disable InvertIf
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Returns the value if not null, otherwise returns the parameter as fallback.
/// </summary>
/// <remarks>
/// <para>One-way binding only. ConvertBack is not supported because the original null state cannot be reconstructed.</para>
/// </remarks>
[ValueConversion(typeof(object), typeof(object))]
public sealed class NullCheckValueConverter : IValueConverter
{
    public static readonly NullCheckValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value ?? parameter;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}