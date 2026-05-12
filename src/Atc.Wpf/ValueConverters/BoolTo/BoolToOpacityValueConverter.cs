// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Opacity (true =&gt; 1.0, false =&gt; 0.0).
/// </summary>
/// <remarks>
/// Returns <c>1.0</c> when value is <see langword="true"/>, otherwise <c>0.0</c>.
/// Permissive — <see langword="null"/> and non-bool values both return <c>0.0</c>
/// (rendering an invisible element rather than throwing on a misconfigured binding).
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Fade element out when IsEnabled is false --&gt;
/// Opacity="{Binding IsEnabled, Converter={x:Static converters:BoolToOpacityValueConverter.Instance}}"
/// </code>
/// </example>
[ValueConversion(typeof(bool), typeof(double))]
public sealed class BoolToOpacityValueConverter : IValueConverter
{
    public static readonly BoolToOpacityValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true
            ? 1.0
            : 0.0;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}