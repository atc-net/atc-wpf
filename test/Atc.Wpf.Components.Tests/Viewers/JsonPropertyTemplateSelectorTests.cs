namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class JsonPropertyTemplateSelectorTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData("object")]
    [InlineData("array")]
    [InlineData("primitive")]
    public void SelectTemplate_TemplatePropertiesSet_ReturnsTheMatchingProperty(
        string propertyName)
    {
        var objectTemplate = new DataTemplate();
        var arrayTemplate = new DataTemplate();
        var primitiveTemplate = new DataTemplate();
        var sut = new JsonPropertyTemplateSelector
        {
            ObjectPropertyTemplate = objectTemplate,
            ArrayPropertyTemplate = arrayTemplate,
            PrimitivePropertyTemplate = primitiveTemplate,
        };

        var template = sut.SelectTemplate(GetProperty(propertyName), new FrameworkElement());

        var expected = propertyName switch
        {
            "object" => objectTemplate,
            "array" => arrayTemplate,
            _ => primitiveTemplate,
        };
        Assert.Same(expected, template);
    }

    [StaFact]
    public void SelectTemplate_TemplatePropertyNotSet_FallsBackToTheResourceKey()
    {
        var resourceTemplate = new DataTemplate();
        var container = new FrameworkElement();
        container.Resources["ObjectPropertyTemplate"] = resourceTemplate;
        var sut = new JsonPropertyTemplateSelector();

        var template = sut.SelectTemplate(GetProperty("object"), container);

        Assert.Same(resourceTemplate, template);
    }

    [StaFact]
    public void SelectTemplate_NoTemplateFound_ReturnsNull()
    {
        var sut = new JsonPropertyTemplateSelector();

        var template = sut.SelectTemplate(GetProperty("primitive"), new FrameworkElement());

        Assert.Null(template);
    }

    private static JsonPropertyNode GetProperty(string name)
        => JsonNode
            .Parse("""{ "object": { "a": 1 }, "array": [ 1 ], "primitive": 1 }""")
            .Children()
            .OfType<JsonPropertyNode>()
            .Single(x => x.Name == name);
}