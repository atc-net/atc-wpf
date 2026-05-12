namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To Bool (true if not null).
/// </summary>
/// <remarks>
/// <para>
/// Functionally equivalent to <see cref="ObjectNotNullToBoolValueConverter"/>.
/// For new bindings, prefer <see cref="ObjectNotNullToBoolValueConverter"/> — it follows the
/// library's <c>&lt;Source&gt;To&lt;Target&gt;ValueConverter</c> naming convention used by
/// all other converters in this namespace. This type is retained for backwards compatibility
/// and XAML brevity.
/// </para>
/// <para>One-way binding only. ConvertBack is not supported because the original object cannot be reconstructed from a boolean.</para>
/// </remarks>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class IsNotNullValueConverter : IValueConverter
{
    public static readonly IsNotNullValueConverter Instance = new();

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