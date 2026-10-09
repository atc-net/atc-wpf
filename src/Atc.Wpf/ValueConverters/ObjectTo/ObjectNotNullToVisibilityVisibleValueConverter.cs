// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(Visibility))]
public sealed class ObjectNotNullToVisibilityVisibleValueConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="ObjectNotNullToVisibilityVisibleValueConverter"/>.
    /// </summary>
    public static readonly ObjectNotNullToVisibilityVisibleValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the visibility returned when the value is <see langword="null"/>; can be overridden by passing <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Hidden"/> as the converter parameter.
    /// </summary>
    public Visibility NonVisibility { get; set; } = Visibility.Collapsed;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
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
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}