namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Integer (milliseconds) To TimeSpan.
/// </summary>
/// <remarks>
/// <para>Supports two-way binding.</para>
/// <para>Convert: int (milliseconds) → TimeSpan</para>
/// <para>ConvertBack: TimeSpan → int (milliseconds)</para>
/// </remarks>
[ValueConversion(typeof(int), typeof(TimeSpan))]
public sealed class IntegerToTimeSpanValueConverter : IValueConverter
{
    public static readonly IntegerToTimeSpanValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not int intValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(int)");
        }

        return TimeSpan.FromMilliseconds(intValue);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not TimeSpan timeSpan)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(TimeSpan)");
        }

        return (int)timeSpan.TotalMilliseconds;
    }
}
