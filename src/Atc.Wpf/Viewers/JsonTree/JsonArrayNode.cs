namespace Atc.Wpf.Viewers.JsonTree;

/// <summary>
/// Represents a JSON array node containing elements.
/// </summary>
public sealed class JsonArrayNode : JsonNode
{
    private readonly IReadOnlyList<JsonNode> items;

    internal JsonArrayNode(IReadOnlyList<JsonNode> items)
        => this.items = items;

    /// <inheritdoc />
    public override JsonNodeType Type
        => JsonNodeType.Array;

    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public int Length => items.Count;

    /// <inheritdoc />
    public override IEnumerable<JsonNode> Children()
        => items;
}