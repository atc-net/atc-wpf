// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// <c>true</c> → <see cref="TextWrapping.Wrap"/>, <c>false</c> → <see cref="TextWrapping.NoWrap"/>.
/// </summary>
[ValueConversion(typeof(bool), typeof(TextWrapping))]
public sealed class BoolToTextWrappingValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly BoolToTextWrappingValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true ? TextWrapping.Wrap : TextWrapping.NoWrap;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is TextWrapping.Wrap;
}