namespace Atc.Wpf.Controls.Viewers.JsonTree;

/// <summary>
/// Represents a JSON primitive value (string, number, boolean, null).
/// </summary>
public sealed class JsonValueNode : JsonNode
{
    internal JsonValueNode(
        JsonNodeType nodeType,
        string? rawValue,
        string displayValue)
    {
        Type = nodeType;
        RawValue = rawValue;
        DisplayValue = displayValue;
    }

    /// <inheritdoc />
    public override JsonNodeType Type { get; }

    /// <summary>
    /// Gets the raw value as a string.
    /// </summary>
    public string? RawValue { get; }

    /// <summary>
    /// Gets the display value for the JSON viewer.
    /// </summary>
    public string DisplayValue { get; }

    /// <inheritdoc />
    public override IEnumerable<JsonNode> Children() => [];

    /// <inheritdoc />
    public override string ToString() => DisplayValue;
}