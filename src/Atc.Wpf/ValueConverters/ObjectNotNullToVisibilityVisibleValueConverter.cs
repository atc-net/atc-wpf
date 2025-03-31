namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(Visibility))]
public sealed class ObjectNotNullToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly ObjectNotNullToVisibilityVisibleValueConverter Instance = new();

    public Visibility NonVisibility { get; set; } = Visibility.Collapsed;

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var nonVisibility = NonVisibility;

        if (parameter is Visibility visibility and (Visibility.Collapsed or Visibility.Hidden))
        {
            nonVisibility = visibility;
        }

        return value is null
            ? nonVisibility
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}