using JsonTreeNode = Atc.Wpf.Viewers.JsonTree.JsonNode;

namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Converts a JsonNode to its children for hierarchical data binding.
/// </summary>
public sealed class JsonNodeChildrenConverter : IValueConverter
{
    public static readonly JsonNodeChildrenConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JsonTreeNode node)
        {
            return null;
        }

        return node.Children();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}