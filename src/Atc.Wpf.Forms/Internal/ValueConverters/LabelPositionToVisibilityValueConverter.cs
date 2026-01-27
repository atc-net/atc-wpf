namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// ValueConverter: LabelPosition to Visibility.
/// </summary>
/// <remarks>
/// ConverterParameter should be the expected LabelPosition value.
/// Returns Visible if the value matches the parameter, otherwise Collapsed.
/// </remarks>
[ValueConversion(typeof(LabelPosition), typeof(Visibility))]
internal sealed class LabelPositionToVisibilityValueConverter : IValueConverter
{
    public static readonly LabelPositionToVisibilityValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not LabelPosition placement)
        {
            // Default to Left if not set
            placement = LabelPosition.Left;
        }

        if (parameter is not LabelPosition expectedPlacement)
        {
            return Visibility.Collapsed;
        }

        return placement == expectedPlacement
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}