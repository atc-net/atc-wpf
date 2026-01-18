namespace Atc.Wpf.Viewers.JsonTree;

/// <summary>
/// Represents a JSON object node containing properties.
/// </summary>
public sealed class JsonObjectNode : JsonNode
{
    private readonly IReadOnlyList<JsonPropertyNode> properties;

    internal JsonObjectNode(IReadOnlyList<JsonPropertyNode> properties)
        => this.properties = properties;

    /// <inheritdoc />
    public override JsonNodeType Type
        => JsonNodeType.Object;

    /// <inheritdoc />
    public override IEnumerable<JsonNode> Children()
        => properties;
}