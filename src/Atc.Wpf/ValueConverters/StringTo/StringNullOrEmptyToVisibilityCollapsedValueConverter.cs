// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String Null Or Empty To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(string), typeof(Visibility))]
public sealed class StringNullOrEmptyToVisibilityCollapsedValueConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="StringNullOrEmptyToVisibilityCollapsedValueConverter"/>.
    /// </summary>
    public static readonly StringNullOrEmptyToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is null || string.IsNullOrEmpty(value.ToString())
            ? Visibility.Collapsed
            : Visibility.Visible;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}