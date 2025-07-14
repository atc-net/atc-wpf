namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: DateTime or DateTimeOffset to local DateTime.
/// </summary>
[ValueConversion(typeof(DateTime), typeof(DateTime))]
[ValueConversion(typeof(DateTimeOffset), typeof(DateTime))]
public sealed class UtcToLocalDateTimeValueConverter : IValueConverter
{
    public static readonly UtcToLocalDateTimeValueConverter Instance = new();

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

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}