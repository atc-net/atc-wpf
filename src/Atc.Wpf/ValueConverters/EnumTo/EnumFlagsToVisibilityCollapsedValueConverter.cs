// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: <c>[Flags]</c> Enum To Visibility-Collapsed.
/// </summary>
/// <remarks>
/// Returns <see cref="Visibility.Collapsed"/> when the bound flag-enum value has all the bits
/// set that the parameter specifies (i.e. <c>value.HasFlag(parameter)</c>); otherwise
/// <see cref="Visibility.Visible"/>. This is the inverse of
/// <see cref="EnumFlagsToVisibilityVisibleValueConverter"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Hide element when user has Banned flag --&gt;
/// Visibility="{Binding UserPermissions,
///     Converter={x:Static converters:EnumFlagsToVisibilityCollapsedValueConverter.Instance},
///     ConverterParameter={x:Static local:Permissions.Banned}}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumFlagsToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly EnumFlagsToVisibilityCollapsedValueConverter Instance = new();

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

        var flag = EnumFlagsToBoolValueConverter.ResolveFlag(enumValue.GetType(), parameter);
        return flag is not null && enumValue.HasFlag(flag)
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