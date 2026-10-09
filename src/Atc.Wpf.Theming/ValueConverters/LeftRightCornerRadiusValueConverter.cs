namespace Atc.Wpf.Theming.ValueConverters;

/// <summary>
/// ValueConverter: converts a uniform <see cref="CornerRadius"/> to one with rounded right
/// corners and square left corners (e.g. for the trailing half of a split button or pill).
/// Reads only <see cref="CornerRadius.TopLeft"/> from the input as the radius source.
/// </summary>
[ValueConversion(typeof(CornerRadius), typeof(CornerRadius))]
public sealed class LeftRightCornerRadiusValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly LeftRightCornerRadiusValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not CornerRadius globalRadius)
        {
            return new CornerRadius(0);
        }

        var r = globalRadius.TopLeft;
        return new CornerRadius(
            0,
            r,
            r,
            0);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}