// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum To Inverse Bool.
/// </summary>
/// <remarks>
/// Returns <see langword="false"/> when the bound enum value matches the parameter
/// (or any item in a multi-value parameter), otherwise returns <see langword="true"/>.
/// This is the inverse of <see cref="EnumToBoolValueConverter"/>.
/// <para>
/// The parameter can be:
/// <list type="bullet">
///   <item><description>A single <see cref="Enum"/> value (e.g. <c>{x:Static sys:DayOfWeek.Monday}</c>).</description></item>
///   <item><description>A single string member name (e.g. <c>"Monday"</c>) — case-insensitive.</description></item>
///   <item><description>A comma-separated string of member names (e.g. <c>"Monday,Tuesday"</c>) — items are trimmed and case-insensitive.</description></item>
///   <item><description>An <see cref="IEnumerable"/> of <see cref="Enum"/> values or member-name strings (e.g. an <c>x:Array</c> or the <c>EnumValuesExtension</c> markup extension).</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Disable button while in transitional states --&gt;
/// IsEnabled="{Binding ConnectionState,
///     Converter={x:Static converters:EnumToInverseBoolValueConverter.Instance},
///     ConverterParameter='Connecting,Disconnecting'}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(bool))]
public sealed class EnumToInverseBoolValueConverter : IValueConverter
{
    public static readonly EnumToInverseBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Enum enumValue ||
            parameter is null)
        {
            return true;
        }

        return !EnumParameterMatcher.Matches(enumValue, parameter);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}