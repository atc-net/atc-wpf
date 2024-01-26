// ReSharper disable InvertIf
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum-Description To String.
/// </summary>
public sealed class EnumDescriptionToStringValueConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// Supported case-formats:
    /// <list type="table">
    ///    <item>
    ///        <term>U</term>
    ///        <description>Converts the entire string to uppercase.</description>
    ///    </item>
    ///    <item>
    ///        <term>u</term>
    ///        <description>Capitalizes the first character of the string.</description>
    ///    </item>
    ///    <item>
    ///        <term>L</term>
    ///        <description>Converts the entire string to lowercase.</description>
    ///    </item>
    ///    <item>
    ///        <term>l</term>
    ///        <description>Converts the first character of the string to lowercase.</description>
    ///    </item>
    ///    <item>
    ///        <term>Ul</term>
    ///        <description>Converts the string to uppercase with the first character in lowercase.</description>
    ///    </item>
    ///    <item>
    ///        <term>Lu</term>
    ///        <description>Converts the string to lowercase with the first character capitalized.</description>
    ///    </item>
    ///    <item>
    ///        <term>[Format]:</term>
    ///        <description>Applies the specified format and appends a colon ´<b>:</b>´ to the end.</description>
    ///    </item>
    ///    <item>
    ///        <term>[Format].</term>
    ///        <description>Applies the specified format and appends a period ´<b>.</b>´ to the end.</description>
    ///    </item>
    /// </list>
    /// </summary>
    /// <param name="value">The value produced by the binding source - Must be an Enum value.</param>
    /// <param name="targetType">The type of the binding target property - Not used.</param>
    /// <param name="parameter">The converter parameter to use - Optional case-formatter.</param>
    /// <param name="culture">The culture to use in the converter - Not used.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not Enum enumValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not an enum type");
        }

        if (parameter is not null)
        {
            var parameterValue = parameter.ToString();
            if (!string.IsNullOrEmpty(parameterValue))
            {
                var caseFormatter = "{0:" + parameterValue + "}";
                return string.Format(
                    StringCaseFormatter.Default,
                    caseFormatter,
                    EnumHelper.GetDescription(enumValue));
            }
        }

        return EnumHelper.GetDescription(enumValue);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}