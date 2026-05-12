// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Inverse Opacity (true =&gt; 0.0, false =&gt; 1.0).
/// </summary>
/// <remarks>
/// Returns <c>0.0</c> when value is <see langword="true"/>, otherwise <c>1.0</c>.
/// Permissive — <see langword="null"/> and non-bool values both return <c>1.0</c>
/// (rendering a fully-opaque element rather than throwing on a misconfigured binding).
/// This is the inverse of <see cref="BoolToOpacityValueConverter"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Fade element in when IsBusy is false, out when true --&gt;
/// Opacity="{Binding IsBusy, Converter={x:Static converters:BoolToInverseOpacityValueConverter.Instance}}"
/// </code>
/// </example>
[ValueConversion(typeof(bool), typeof(double))]
public sealed class BoolToInverseOpacityValueConverter : IValueConverter
{
    public static readonly BoolToInverseOpacityValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true
            ? 0.0
            : 1.0;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}