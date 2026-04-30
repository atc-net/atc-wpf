// ReSharper disable CheckNamespace
// ReSharper disable MergeIntoLogicalPattern
// ReSharper disable RedundantCast
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MultiValueConverter: Combines a <see cref="bool"/> visibility flag with a <see cref="double"/> width
/// (or any numeric size). Returns the double when the bool is <c>true</c>, otherwise <c>0d</c>.
/// </summary>
/// <remarks>
/// Useful for collapsing a <see cref="System.Windows.Controls.GridViewColumn"/> (which has no
/// <c>Visibility</c> property) by setting its <c>Width</c> to <c>0</c> when hidden.
/// Bind two values: index 0 = <see cref="bool"/>, index 1 = <see cref="double"/>.
/// </remarks>
[ValueConversion(typeof(object[]), typeof(double))]
public sealed class BoolAndDoubleToDoubleMultiValueConverter : IMultiValueConverter
{
    public static readonly BoolAndDoubleToDoubleMultiValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object?[] values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (values is null || values.Length < 2)
        {
            return 0d;
        }

        if (values[0] is not bool boolValue || !boolValue)
        {
            return 0d;
        }

        return values[1] switch
        {
            double d => d,
            float f => (double)f,
            int i => (double)i,
            _ => 0d,
        };
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}