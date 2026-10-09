namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Converts a JsonValueNode to its display string representation.
/// </summary>
public sealed class JsonValueDisplayConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="JsonValueDisplayConverter"/>.
    /// </summary>
    public static readonly JsonValueDisplayConverter Instance = new();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}