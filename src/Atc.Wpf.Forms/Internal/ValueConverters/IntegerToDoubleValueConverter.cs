namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Integer to Double.
/// </summary>
/// <remarks>
/// <para>
/// Functionally identical to <see cref="Atc.Wpf.Controls.ValueConverters.IntegerToDoubleValueConverter"/>.
/// Prefer the public converter in <c>Atc.Wpf.Controls</c> when accessible. This internal copy
/// exists so labelled form controls in <c>Atc.Wpf.Forms</c> can reference it without leaking the
/// type to the public Forms API surface.
/// </para>
/// <para>
/// If both converters' bodies ever diverge, update <see cref="Atc.Wpf.Controls.ValueConverters.IntegerToDoubleValueConverter"/>
/// first (it is the canonical source) and mirror the change here.
/// </para>
/// </remarks>
[ValueConversion(typeof(int), typeof(double))]
[ValueConversion(typeof(decimal), typeof(double))]
internal sealed class IntegerToDoubleValueConverter : IValueConverter
{
    public static readonly IntegerToDoubleValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            int i => i,
            decimal d => d,
            _ => 0D,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    => value switch
        {
            double d => d,
            _ => 0,
        };
}