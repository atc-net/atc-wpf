namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// ValueConverter: RenderColorIndicatorType To Visibility.
/// </summary>
[ValueConversion(typeof(RenderColorIndicatorType), typeof(Visibility))]
public class RenderColorIndicatorTypeToVisibilityValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not RenderColorIndicatorType actualRenderMode)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof({nameof(RenderColorIndicatorType)})");
        }

        if (parameter is not RenderColorIndicatorType wantedRenderMode)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof({nameof(RenderColorIndicatorType)})");
        }

        return actualRenderMode == wantedRenderMode
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}