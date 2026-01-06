namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// Converts a JsonArrayNode or JsonPropertyNode containing an array to a display string showing its length.
/// </summary>
public sealed class JsonArrayLengthConverter : IValueConverter
{
    public static readonly JsonArrayLengthConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            JsonArrayNode arrayNode => $"[{arrayNode.Length.ToString(GlobalizationConstants.EnglishCultureInfo)}]",
            JsonPropertyNode { Value: JsonArrayNode propArray } => $"[ {propArray.Length.ToString(GlobalizationConstants.EnglishCultureInfo)} ]",
            _ => string.Empty,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}