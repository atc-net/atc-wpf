namespace Atc.Wpf.Theming.ValueConverters;

[MarkupExtensionReturnType(typeof(ColorToNameValueConverter))]
[ValueConversion(typeof(Color), typeof(string))]
public sealed class ColorToNameValueConverter : MarkupMultiValueConverterBase
{
    /// <summary>
    /// Converts a given <see cref="Color"/> to its Name
    /// </summary>
    /// <param name="value">Needed: The <see cref="Color"/>. </param>
    /// <param name="targetType">The targetType.</param>
    /// <param name="parameter">Optional: A <see cref="Dictionary{TKey, TValue}"/></param>
    /// <param name="culture">The culture.</param>
    /// <returns>The name of the color or the Hex-Code if no name is available</returns>
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return ColorHelper.GetColorNameFromColor(color);
        }

        return Colors.Pink;
    }

    /// <summary>
    /// Converts a given <see cref="Color"/> to its Name
    /// </summary>
    /// <param name="values">
    /// Needed: The <see cref="Color"/>.
    /// Optional: A <see cref="Dictionary{TKey, TValue}"/>.
    /// Optional: A <see cref="bool"/> if the alpha channel is visible.
    /// Optional: A custom <see cref="ColorHelper"/> used to get the color name.
    /// </param>
    /// <param name="targetType">The targetType.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>The name of the color or the Hex-Code if no name is available</returns>
    public override object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.FirstOrDefault(x => x?.GetType() == typeof(Color)) is Color color)
        {
            return ColorHelper.GetColorNameFromColor(color, CultureInfo.InvariantCulture);
        }

        return Colors.Pink;
    }

    /// <summary>
    /// Converts a given <see cref="string"/> back to a <see cref="Color"/>
    /// </summary>
    /// <param name="value">The name of the <see cref="Color"/></param>
    /// <param name="targetType">The targetType.</param>
    /// <param name="parameter">Optional: A <see cref="Dictionary{TKey, TValue}"/></param>
    /// <param name="culture">The culture.</param>
    /// <returns><see cref="Color"/></returns>
    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            return ColorHelper.GetColorFromString(text, CultureInfo.InvariantCulture);
        }

        Trace.TraceError($"Unable to convert the provided value '{value}' to System.Windows.Media.Color");
        return Binding.DoNothing;
    }

    /// <summary>
    /// The ConvertBack-Method is not available inside a <see cref="MultiBinding"/>. Use a <see cref="Binding"/> with the optional <see cref="Binding.ConverterParameter"/> instead.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The targetTypes.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <throws><see cref="NotSupportedException"/></throws>
    public override object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}