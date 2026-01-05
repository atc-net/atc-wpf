namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// Converts a JsonValueNode to its display string representation.
/// </summary>
public sealed class JsonValueDisplayConverter : IValueConverter
{
    public static readonly JsonValueDisplayConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JsonValueNode jsonValue)
        {
            return value;
        }

        return jsonValue.DisplayValue;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}
