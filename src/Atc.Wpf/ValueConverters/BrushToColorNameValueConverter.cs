namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: SolidColorBrush To Color-Name (key).
/// </summary>
/// <remarks>
/// <para>Supports two-way binding.</para>
/// <para>Convert: SolidColorBrush → string (English-invariant color key, e.g. "Red")</para>
/// <para>ConvertBack: string (color key) → SolidColorBrush</para>
/// <para>
/// Returns an empty string when the brush has no matching well-known color
/// (which leaves a bound <c>WellKnownColorSelector.SelectedKey</c> unchanged).
/// </para>
/// </remarks>
[ValueConversion(typeof(SolidColorBrush), typeof(string))]
public sealed class BrushToColorNameValueConverter : IValueConverter
{
    public static readonly BrushToColorNameValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return string.Empty;
        }

        return SolidColorBrushHelper.GetBrushKeyFromBrush(brush) ?? string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not string key || string.IsNullOrEmpty(key))
        {
            return Binding.DoNothing;
        }

        var brush = SolidColorBrushHelper.GetBrushFromName(
            key,
            GlobalizationConstants.EnglishCultureInfo);

        return brush is null
            ? Binding.DoNothing
            : brush;
    }
}