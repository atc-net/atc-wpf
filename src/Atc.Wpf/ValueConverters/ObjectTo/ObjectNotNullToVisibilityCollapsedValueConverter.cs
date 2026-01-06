// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(Visibility))]
public sealed class ObjectNotNullToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly ObjectNotNullToVisibilityCollapsedValueConverter Instance = new();

    public Visibility NonVisibility { get; set; } = Visibility.Visible;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var nonVisibility = NonVisibility;

        if (parameter is Visibility visibility and Visibility.Visible)
        {
            nonVisibility = visibility;
        }

        return value is null
            ? nonVisibility
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}