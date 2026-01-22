// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum To Visibility-Collapsed.
/// </summary>
/// <remarks>
/// Converts an enum value to <see cref="Visibility.Collapsed"/> when it matches the parameter,
/// or <see cref="Visibility.Visible"/> when it doesn't.
/// This is the inverse of <see cref="EnumToVisibilityVisibleValueConverter"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Hide element when Status is Inactive --&gt;
/// Visibility="{Binding Status, Converter={x:Static converters:EnumToVisibilityCollapsedValueConverter.Instance}, ConverterParameter=Inactive}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly EnumToVisibilityCollapsedValueConverter Instance = new();

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
            return Visibility.Visible;
        }

        // Handle parameter as enum of the same type
        if (parameter is Enum parameterEnum)
        {
            return Equals(enumValue, parameterEnum)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        // Handle parameter as string
        var parameterString = parameter.ToString();
        if (string.IsNullOrEmpty(parameterString))
        {
            return Visibility.Visible;
        }

        var enumString = enumValue.ToString();
        return string.Equals(enumString, parameterString, StringComparison.OrdinalIgnoreCase)
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