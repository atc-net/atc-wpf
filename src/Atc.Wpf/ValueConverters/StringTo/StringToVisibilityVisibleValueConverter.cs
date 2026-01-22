// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String To Visibility-Visible.
/// </summary>
/// <remarks>
/// Converts a string value to <see cref="Visibility.Visible"/> when it matches the parameter,
/// or <see cref="Visibility.Collapsed"/> when it doesn't.
/// Comparison is case-insensitive by default.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Show element when Status is "Active" --&gt;
/// Visibility="{Binding Status, Converter={x:Static converters:StringToVisibilityVisibleValueConverter.Instance}, ConverterParameter=Active}"
/// </code>
/// </example>
[ValueConversion(typeof(string), typeof(Visibility))]
public sealed class StringToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly StringToVisibilityVisibleValueConverter Instance = new();

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
            return Visibility.Visible;
        }

        if (string.IsNullOrEmpty(stringValue) ||
            string.IsNullOrEmpty(parameterString))
        {
            return Visibility.Collapsed;
        }

        return string.Equals(stringValue, parameterString, StringComparison.OrdinalIgnoreCase)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}