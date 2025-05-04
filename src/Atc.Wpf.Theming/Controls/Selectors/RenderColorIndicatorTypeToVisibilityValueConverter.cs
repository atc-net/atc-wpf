namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// ValueConverter: RenderColorIndicatorType To Visibility.
/// </summary>
[ValueConversion(typeof(RenderColorIndicatorType), typeof(Visibility))]
public sealed class RenderColorIndicatorTypeToVisibilityValueConverter : IValueConverter
{
    public static readonly RenderColorIndicatorTypeToVisibilityValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not RenderColorIndicatorType actualRenderMode ||
            parameter is not RenderColorIndicatorType wantedRenderMode)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof({nameof(RenderColorIndicatorType)})");
        }

        return actualRenderMode == wantedRenderMode
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