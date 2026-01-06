namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: DateTime or DateTimeOffset (UTC) to local DateTime.
/// </summary>
/// <remarks>
/// <para>Supports two-way binding.</para>
/// <para>Convert: DateTime/DateTimeOffset (UTC) → DateTime (Local)</para>
/// <para>ConvertBack: DateTime (Local) → DateTime (UTC)</para>
/// </remarks>
[ValueConversion(typeof(DateTime), typeof(DateTime))]
[ValueConversion(typeof(DateTimeOffset), typeof(DateTime))]
public sealed class UtcToLocalDateTimeValueConverter : IValueConverter
{
    public static readonly UtcToLocalDateTimeValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            DateTimeOffset dto => dto.ToLocalTime().DateTime,
            DateTime dt => dt.Kind switch
            {
                DateTimeKind.Utc => dt.ToLocalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToLocalTime(),
                _ => dt, // already local
            },
            _ => null,
        };

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not DateTime localDateTime)
        {
            return null;
        }

        return localDateTime.Kind switch
        {
            DateTimeKind.Local => localDateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime(),
            _ => localDateTime, // already UTC
        };
    }
}