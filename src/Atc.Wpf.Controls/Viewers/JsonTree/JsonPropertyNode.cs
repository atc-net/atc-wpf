namespace Atc.Wpf.Controls.Viewers.JsonTree;

/// <summary>
/// Represents a JSON property (name-value pair) in an object.
/// </summary>
public sealed class JsonPropertyNode : JsonNode
{
    private readonly JsonNode valueNode;

    internal JsonPropertyNode(string name, JsonNode valueNode)
    {
        this.Name = name;
        this.valueNode = valueNode;
    }

    /// <inheritdoc />
    public override JsonNodeType Type => JsonNodeType.Property;

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value node of this property.
    /// </summary>
    public JsonNode Value => valueNode;

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public JsonNodeType ValueType => valueNode.Type;

    /// <inheritdoc />
    public override IEnumerable<JsonNode> Children()
    {
        if (valueNode is JsonObjectNode or JsonArrayNode)
        {
            return valueNode.Children();
        }

        return [];
    }
}