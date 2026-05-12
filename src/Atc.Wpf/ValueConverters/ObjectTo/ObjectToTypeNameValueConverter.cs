// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: any object → its runtime <see cref="Type"/> name.
/// </summary>
/// <remarks>
/// Returns <see cref="System.Reflection.MemberInfo.Name"/> by default, or <see cref="Type.FullName"/> when
/// <c>ConverterParameter=Full</c> (case-insensitive). Null input returns
/// <see cref="string.Empty"/>. Useful for diagnostic/debug overlays.
/// </remarks>
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding SelectedItem,
///     Converter={x:Static converters:ObjectToTypeNameValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(object), typeof(string))]
public sealed class ObjectToTypeNameValueConverter : IValueConverter
{
    public static readonly ObjectToTypeNameValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var type = value.GetType();

        if (parameter is string p &&
            string.Equals(p, "Full", StringComparison.OrdinalIgnoreCase))
        {
            return type.FullName ?? type.Name;
        }

        return type.Name;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}