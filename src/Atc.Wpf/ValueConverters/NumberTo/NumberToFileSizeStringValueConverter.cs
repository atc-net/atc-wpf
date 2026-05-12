// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Number (bytes) To human-readable file-size string.
/// </summary>
/// <remarks>
/// Delegates formatting to <see cref="Atc.Units.DigitalInformation.ByteSizeFormatter"/>.
/// Accepts <see cref="long"/>, <see cref="int"/>, <see cref="ulong"/>, <see cref="uint"/>,
/// <see cref="double"/>, and <see cref="decimal"/> values; everything else returns
/// <see cref="string.Empty"/>.
/// <para>
/// Consumers can override the formatter globally at application startup, e.g.
/// <c>NumberToFileSizeStringValueConverter.Formatter = new ByteSizeFormatter { NumberOfDecimals = 2 };</c>.
/// Reset via <c>Formatter = ByteSizeFormatter.Default</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Display file size --&gt;
/// &lt;TextBlock Text="{Binding FileSizeBytes,
///     Converter={x:Static converters:NumberToFileSizeStringValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(long), typeof(string))]
public sealed class NumberToFileSizeStringValueConverter : IValueConverter
{
    public static readonly NumberToFileSizeStringValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the formatter used to render byte sizes.
    /// Defaults to <see cref="Atc.Units.DigitalInformation.ByteSizeFormatter.Default"/>.
    /// </summary>
    public static Atc.Units.DigitalInformation.ByteSizeFormatter Formatter { get; set; }
        = Atc.Units.DigitalInformation.ByteSizeFormatter.Default;

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => TryGetBytes(value, out var bytes)
            ? Formatter.Format(bytes)
            : string.Empty;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    private static bool TryGetBytes(
        object? value,
        out long bytes)
    {
        switch (value)
        {
            case long l:
                bytes = l;
                return true;
            case int i:
                bytes = i;
                return true;
            case ulong ul when ul <= long.MaxValue:
                bytes = (long)ul;
                return true;
            case uint u:
                bytes = u;
                return true;
            case double d when !double.IsNaN(d) && !double.IsInfinity(d):
                bytes = (long)d;
                return true;
            case decimal dec:
                bytes = (long)dec;
                return true;
            default:
                bytes = 0;
                return false;
        }
    }
}