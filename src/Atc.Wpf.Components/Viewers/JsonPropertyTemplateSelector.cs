namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Selects the appropriate data template for JSON property nodes based on their value type.
/// </summary>
public sealed class JsonPropertyTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the data template for property nodes with an object value.
    /// When not set, the <c>ObjectPropertyTemplate</c> resource is used.
    /// </summary>
    public DataTemplate? ObjectPropertyTemplate { get; set; }

    /// <summary>
    /// Gets or sets the data template for property nodes with an array value.
    /// When not set, the <c>ArrayPropertyTemplate</c> resource is used.
    /// </summary>
    public DataTemplate? ArrayPropertyTemplate { get; set; }

    /// <summary>
    /// Gets or sets the data template for property nodes with a primitive value.
    /// When not set, the <c>PrimitivePropertyTemplate</c> resource is used.
    /// </summary>
    public DataTemplate? PrimitivePropertyTemplate { get; set; }

    /// <inheritdoc />
    public override DataTemplate? SelectTemplate(
        object? item,
        DependencyObject container)
    {
        if (item is null)
        {
            return null;
        }

        if (container is not FrameworkElement frameworkElement)
        {
            return null;
        }

        if (item is JsonPropertyNode propertyNode)
        {
            return propertyNode.ValueType switch
            {
                JsonNodeType.Object => ObjectPropertyTemplate ?? FindTemplate(frameworkElement, "ObjectPropertyTemplate"),
                JsonNodeType.Array => ArrayPropertyTemplate ?? FindTemplate(frameworkElement, "ArrayPropertyTemplate"),
                _ => PrimitivePropertyTemplate ?? FindTemplate(frameworkElement, "PrimitivePropertyTemplate"),
            };
        }

        return FindTemplate(frameworkElement, new DataTemplateKey(item.GetType()));
    }

    private static DataTemplate? FindTemplate(
        FrameworkElement frameworkElement,
        object resourceKey)
        => frameworkElement.TryFindResource(resourceKey) as DataTemplate;
}