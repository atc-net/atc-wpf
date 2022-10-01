namespace Atc.Wpf.Theming.ValueConverters;

[MarkupExtensionReturnType(typeof(ColorToNameValueConverter))]
[ValueConversion(typeof(Color), typeof(string))]
public class ColorToNameValueConverter : MarkupMultiValueConverterBase
{
    public ColorHelper? ColorHelper { get; set; }

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
        return (ColorHelper ?? ColorHelper.DefaultInstance).GetColorName(
            value as Color?,
            parameter as Dictionary<Color, string>,
            useAlphaChannel: true);
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
        var color = values?.FirstOrDefault(x => x?.GetType() == typeof(Color)) as Color?;
        var colorNamesDictionary = values?.FirstOrDefault(x => x.GetType() == typeof(Dictionary<Color, string>)) as Dictionary<Color, string>;
        var useAlphaChannel = values?.FirstOrDefault(x => x?.GetType() == typeof(bool)) as bool?;
        var colorHelper = values?.FirstOrDefault(x => x is ColorHelper) as ColorHelper ?? (ColorHelper ?? ColorHelper.DefaultInstance);

        return colorHelper.GetColorName(
            color,
            colorNamesDictionary,
            useAlphaChannel ?? true);
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
            return (ColorHelper ?? ColorHelper.DefaultInstance).ColorFromString(
                text,
                parameter as Dictionary<Color, string>) ?? Binding.DoNothing;
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
        throw new NotSupportedException();
    }
}