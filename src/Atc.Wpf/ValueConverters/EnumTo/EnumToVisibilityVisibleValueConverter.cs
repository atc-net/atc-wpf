// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum To Visibility-Visible.
/// </summary>
/// <remarks>
/// Converts an enum value to <see cref="Visibility.Visible"/> when it matches the parameter,
/// or <see cref="Visibility.Collapsed"/> when it doesn't.
/// The parameter should be the enum value to compare against (as string or enum).
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Show element when Status is Active --&gt;
/// Visibility="{Binding Status, Converter={x:Static converters:EnumToVisibilityVisibleValueConverter.Instance}, ConverterParameter=Active}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly EnumToVisibilityVisibleValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Enum enumValue ||
            parameter is null)
        {
            return Visibility.Collapsed;
        }

        // Handle parameter as enum of the same type
        if (parameter is Enum parameterEnum)
        {
            return Equals(enumValue, parameterEnum)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Handle parameter as string
        var parameterString = parameter.ToString();
        if (string.IsNullOrEmpty(parameterString))
        {
            return Visibility.Collapsed;
        }

        var enumString = enumValue.ToString();
        return string.Equals(enumString, parameterString, StringComparison.OrdinalIgnoreCase)
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