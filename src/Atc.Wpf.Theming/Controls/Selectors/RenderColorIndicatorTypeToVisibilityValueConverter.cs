namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// ValueConverter: RenderColorIndicatorType To Visibility.
/// </summary>
[ValueConversion(typeof(RenderColorIndicatorType), typeof(Visibility))]
public sealed class RenderColorIndicatorTypeToVisibilityValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly RenderColorIndicatorTypeToVisibilityValueConverter Instance = new();

    /// <summary>
    /// Converts a <see cref="RenderColorIndicatorType"/> to <see cref="Visibility.Visible"/> when it equals
    /// the <see cref="RenderColorIndicatorType"/> given as parameter; otherwise <see cref="Visibility.Collapsed"/>.
    /// </summary>
    /// <param name="value">The actual indicator type.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The indicator type to compare against.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The resulting <see cref="Visibility"/>.</returns>
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

    /// <summary>
    /// Not supported; this is a one-way converter.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>This method always throws.</returns>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}