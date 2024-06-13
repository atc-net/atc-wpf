namespace Atc.Wpf.Controls.TemplateSelectors;

public sealed class JPropertyDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ObjectPropertyTemplate { get; set; }

    public DataTemplate? ArrayPropertyTemplate { get; set; }

    public DataTemplate? PrimitivePropertyTemplate { get; set; }

    public override DataTemplate? SelectTemplate(
        object? item,
        DependencyObject container)
    {
        if (item == null)
        {
            return null;
        }

        if (container is not FrameworkElement frameworkElement)
        {
            return null;
        }

        var type = item.GetType();
        if (type == typeof(JProperty))
        {
            var jProperty = item as JProperty;
            return jProperty?.Value.Type switch
            {
                JTokenType.Object => frameworkElement.FindResource("ObjectPropertyTemplate") as DataTemplate,
                JTokenType.Array => frameworkElement.FindResource("ArrayPropertyTemplate") as DataTemplate,
                _ => frameworkElement.FindResource("PrimitivePropertyTemplate") as DataTemplate,
            };
        }

        var key = new DataTemplateKey(type);
        return frameworkElement.FindResource(key) as DataTemplate;
    }
}