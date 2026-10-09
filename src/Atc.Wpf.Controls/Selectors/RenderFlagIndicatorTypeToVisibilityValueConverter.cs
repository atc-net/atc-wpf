namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// ValueConverter: RenderFlagIndicatorType To Visibility.
/// </summary>
[ValueConversion(typeof(RenderFlagIndicatorType), typeof(Visibility))]
public sealed class RenderFlagIndicatorTypeToVisibilityValueConverter : IValueConverter
{
    /// <summary>The shared instance of the converter.</summary>
    public static readonly RenderFlagIndicatorTypeToVisibilityValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not RenderFlagIndicatorType actualRenderMode)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof({nameof(RenderFlagIndicatorType)})");
        }

        if (parameter is not RenderFlagIndicatorType wantedRenderMode)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof({nameof(RenderFlagIndicatorType)})");
        }

        return actualRenderMode == wantedRenderMode
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(
            $"{nameof(RenderFlagIndicatorTypeToVisibilityValueConverter)} is one-way only.");
}