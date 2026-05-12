// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: <c>[Flags]</c> Enum To Bool.
/// </summary>
/// <remarks>
/// Returns <see langword="true"/> when the bound flag-enum value has all the bits set
/// that the parameter specifies (i.e. <c>value.HasFlag(parameter)</c>); otherwise <see langword="false"/>.
/// <para>
/// The parameter can be:
/// <list type="bullet">
///   <item><description>A single <see cref="Enum"/> value (e.g. <c>{x:Static local:Permissions.Read}</c>).</description></item>
///   <item><description>A combined enum value via <c>|</c>, e.g. <c>Permissions.Read | Permissions.Write</c> — matches only when the bound value has both flags set.</description></item>
///   <item><description>A string member name (e.g. <c>"Read"</c>) — resolved via <see cref="Enum.Parse(System.Type, string, bool)"/> against the bound value's enum type. Combined flags as <c>"Read,Write"</c> are also supported because <see cref="Enum.Parse(System.Type, string, bool)"/> handles them natively.</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Show admin panel only when user has Admin flag --&gt;
/// IsEnabled="{Binding UserPermissions,
///     Converter={x:Static converters:EnumFlagsToBoolValueConverter.Instance},
///     ConverterParameter={x:Static local:Permissions.Admin}}"
///
/// &lt;!-- Require both Read AND Write --&gt;
/// IsEnabled="{Binding UserPermissions,
///     Converter={x:Static converters:EnumFlagsToBoolValueConverter.Instance},
///     ConverterParameter='Read,Write'}"
/// </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(bool))]
public sealed class EnumFlagsToBoolValueConverter : IValueConverter
{
    public static readonly EnumFlagsToBoolValueConverter Instance = new();

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
            return false;
        }

        var flag = ResolveFlag(enumValue.GetType(), parameter);
        return flag is not null && enumValue.HasFlag(flag);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    internal static Enum? ResolveFlag(
        Type enumType,
        object parameter)
    {
        if (parameter is Enum enumParameter)
        {
            return enumParameter;
        }

        if (parameter is string stringParameter &&
            !string.IsNullOrEmpty(stringParameter))
        {
            try
            {
                return (Enum)Enum.Parse(enumType, stringParameter, ignoreCase: true);
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return null;
            }
        }

        return null;
    }
}