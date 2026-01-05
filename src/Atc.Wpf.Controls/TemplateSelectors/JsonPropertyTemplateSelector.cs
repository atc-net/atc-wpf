namespace Atc.Wpf.Controls.TemplateSelectors;

/// <summary>
/// Selects the appropriate data template for JSON property nodes based on their value type.
/// </summary>
public sealed class JsonPropertyTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ObjectPropertyTemplate { get; set; }

    public DataTemplate? ArrayPropertyTemplate { get; set; }

    public DataTemplate? PrimitivePropertyTemplate { get; set; }

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
