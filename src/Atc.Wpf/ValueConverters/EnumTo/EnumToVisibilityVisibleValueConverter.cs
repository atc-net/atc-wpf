// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum To Visibility-Visible.
/// </summary>
/// <remarks>
/// Converts an enum value to <see cref="Visibility.Visible"/> when it matches the parameter
/// (or any item in a multi-value parameter), otherwise returns <see cref="Visibility.Collapsed"/>.
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
/// &lt;!-- Single value --&gt;
/// Visibility="{Binding Status, Converter={x:Static converters:EnumToVisibilityVisibleValueConverter.Instance}, ConverterParameter=Active}"
///
/// &lt;!-- Multi-value (comma string) --&gt;
/// Visibility="{Binding Day, Converter={x:Static converters:EnumToVisibilityVisibleValueConverter.Instance}, ConverterParameter='Monday,Tuesday'}"
///
/// &lt;!-- Multi-value (type-safe via EnumValuesExtension) --&gt;
/// Visibility="{Binding Day,
///     Converter={x:Static converters:EnumToVisibilityVisibleValueConverter.Instance},
///     ConverterParameter={atc:EnumValues {x:Static sys:DayOfWeek.Monday}, {x:Static sys:DayOfWeek.Tuesday}}}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly EnumToVisibilityVisibleValueConverter Instance = new();

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
            return Visibility.Collapsed;
        }

        return EnumParameterMatcher.Matches(enumValue, parameter)
            ? Visibility.Visible
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