// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Enum To Visibility-Collapsed.
/// </summary>
/// <remarks>
/// Converts an enum value to <see cref="Visibility.Collapsed"/> when it matches the parameter
/// (or any item in a multi-value parameter), otherwise returns <see cref="Visibility.Visible"/>.
/// This is the inverse of <see cref="EnumToVisibilityVisibleValueConverter"/>.
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
/// Visibility="{Binding Status, Converter={x:Static converters:EnumToVisibilityCollapsedValueConverter.Instance}, ConverterParameter=Inactive}"
///
/// &lt;!-- Multi-value (comma string) --&gt;
/// Visibility="{Binding Day, Converter={x:Static converters:EnumToVisibilityCollapsedValueConverter.Instance}, ConverterParameter='Saturday,Sunday'}"
///
/// &lt;!-- Multi-value (type-safe via EnumValuesExtension) --&gt;
/// Visibility="{Binding Day,
///     Converter={x:Static converters:EnumToVisibilityCollapsedValueConverter.Instance},
///     ConverterParameter={atc:EnumValues {x:Static sys:DayOfWeek.Saturday}, {x:Static sys:DayOfWeek.Sunday}}}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly EnumToVisibilityCollapsedValueConverter Instance = new();

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
            return Visibility.Visible;
        }

        return EnumParameterMatcher.Matches(enumValue, parameter)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}