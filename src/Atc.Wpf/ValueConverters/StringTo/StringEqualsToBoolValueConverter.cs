// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String Equals To Bool.
/// </summary>
/// <remarks>
/// Returns <see langword="true"/> when the bound string matches the parameter,
/// otherwise <see langword="false"/>. Comparison is case-insensitive
/// (<see cref="StringComparison.OrdinalIgnoreCase"/>). The bool-returning twin of
/// <see cref="StringToVisibilityVisibleValueConverter"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Select theme via radio buttons bound to a string --&gt;
/// IsChecked="{Binding SelectedTheme,
///     Converter={x:Static converters:StringEqualsToBoolValueConverter.Instance},
///     ConverterParameter=Dark}"
/// </code>
/// </example>
[ValueConversion(typeof(string), typeof(bool))]
public sealed class StringEqualsToBoolValueConverter : IValueConverter
{
    public static readonly StringEqualsToBoolValueConverter Instance = new();

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
            return true;
        }

        if (string.IsNullOrEmpty(stringValue) ||
            string.IsNullOrEmpty(parameterString))
        {
            return false;
        }

        return string.Equals(stringValue, parameterString, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}