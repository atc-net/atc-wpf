namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To Bool (true if null).
/// </summary>
/// <remarks>
/// <para>One-way binding only. ConvertBack is not supported because the original object cannot be reconstructed from a boolean.</para>
/// </remarks>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class IsNullValueConverter : IValueConverter
{
    public static readonly IsNullValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is null;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}