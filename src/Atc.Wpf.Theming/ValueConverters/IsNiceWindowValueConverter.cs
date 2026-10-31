namespace Atc.Wpf.Theming.ValueConverters;

/// <summary>
/// ValueConverter: <see cref="object"/> → <see cref="bool"/>; <see langword="true"/> when the value is a
/// <see cref="NiceWindow"/>. Lets a style bind to NiceWindow-only properties (such as the title brushes)
/// only when the window really is a NiceWindow, so the style stays free of binding errors in a plain Window.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class IsNiceWindowValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly IsNiceWindowValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is NiceWindow;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}