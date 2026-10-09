namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Selects the appropriate data template for JSON property nodes based on their value type.
/// </summary>
public sealed class JsonPropertyTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the data template for property nodes with an object value.
    /// Note: template selection currently resolves the <c>ObjectPropertyTemplate</c> resource key instead of this property.
    /// </summary>
    public DataTemplate? ObjectPropertyTemplate { get; set; }

    /// <summary>
    /// Gets or sets the data template for property nodes with an array value.
    /// Note: template selection currently resolves the <c>ArrayPropertyTemplate</c> resource key instead of this property.
    /// </summary>
    public DataTemplate? ArrayPropertyTemplate { get; set; }

    /// <summary>
    /// Gets or sets the data template for property nodes with a primitive value.
    /// Note: template selection currently resolves the <c>PrimitivePropertyTemplate</c> resource key instead of this property.
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
                JsonNodeType.Object => frameworkElement.FindResource("ObjectPropertyTemplate") as DataTemplate,
                JsonNodeType.Array => frameworkElement.FindResource("ArrayPropertyTemplate") as DataTemplate,
                _ => frameworkElement.FindResource("PrimitivePropertyTemplate") as DataTemplate,
            };
        }

        var key = new DataTemplateKey(item.GetType());
        return frameworkElement.FindResource(key) as DataTemplate;
    }
}