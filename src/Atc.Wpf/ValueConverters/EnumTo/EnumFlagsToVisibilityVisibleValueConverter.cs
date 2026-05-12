// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: <c>[Flags]</c> Enum To Visibility-Visible.
/// </summary>
/// <remarks>
/// Returns <see cref="Visibility.Visible"/> when the bound flag-enum value has all the bits
/// set that the parameter specifies (i.e. <c>value.HasFlag(parameter)</c>); otherwise
/// <see cref="Visibility.Collapsed"/>. See <see cref="EnumFlagsToBoolValueConverter"/> for
/// the supported parameter shapes.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Show element only when user has Admin flag --&gt;
/// Visibility="{Binding UserPermissions,
///     Converter={x:Static converters:EnumFlagsToVisibilityVisibleValueConverter.Instance},
///     ConverterParameter={x:Static local:Permissions.Admin}}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(Visibility))]
public sealed class EnumFlagsToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly EnumFlagsToVisibilityVisibleValueConverter Instance = new();

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

        var flag = EnumFlagsToBoolValueConverter.ResolveFlag(enumValue.GetType(), parameter);
        return flag is not null && enumValue.HasFlag(flag)
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