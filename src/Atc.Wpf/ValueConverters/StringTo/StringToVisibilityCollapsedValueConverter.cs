// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String To Visibility-Collapsed.
/// </summary>
/// <remarks>
/// Converts a string value to <see cref="Visibility.Collapsed"/> when it matches the parameter,
/// or <see cref="Visibility.Visible"/> when it doesn't.
/// This is the inverse of <see cref="StringToVisibilityVisibleValueConverter"/>.
/// Comparison is case-insensitive by default.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Hide element when Status is "Hidden" --&gt;
/// Visibility="{Binding Status, Converter={x:Static converters:StringToVisibilityCollapsedValueConverter.Instance}, ConverterParameter=Hidden}"
/// </code>
/// </example>
[ValueConversion(typeof(string), typeof(Visibility))]
public sealed class StringToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly StringToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var stringValue = value?.ToString();
        var parameterString = parameter?.ToString();

        if (string.IsNullOrEmpty(stringValue) &&
            string.IsNullOrEmpty(parameterString))
        {
            return Visibility.Collapsed;
        }

        if (string.IsNullOrEmpty(stringValue) ||
            string.IsNullOrEmpty(parameterString))
        {
            return Visibility.Visible;
        }

        return string.Equals(stringValue, parameterString, StringComparison.OrdinalIgnoreCase)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}