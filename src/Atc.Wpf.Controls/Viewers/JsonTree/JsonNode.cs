namespace Atc.Wpf.Controls.Viewers.JsonTree;

/// <summary>
/// Abstract base class for JSON tree nodes used in JsonViewer.
/// </summary>
public abstract class JsonNode
{
    /// <summary>
    /// Gets the type of this JSON node.
    /// </summary>
    public abstract JsonNodeType Type { get; }

    /// <summary>
    /// Gets the child nodes of this node.
    /// </summary>
    /// <returns>An enumerable of child nodes.</returns>
    public abstract IEnumerable<JsonNode> Children();

    /// <summary>
    /// Parses a JSON string into a JsonNode tree.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The root JsonNode.</returns>
    public static JsonNode Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return FromElement(document.RootElement);
    }

    internal static JsonNode FromElement(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.Object => CreateObjectNode(element),
            JsonValueKind.Array => CreateArrayNode(element),
            _ => CreateValueNode(element),
        };

    private static JsonObjectNode CreateObjectNode(JsonElement element)
    {
        var children = new List<JsonPropertyNode>();
        foreach (var property in element.EnumerateObject())
        {
            children.Add(CreatePropertyNode(property));
        }

        return new JsonObjectNode(children);
    }

    private static JsonArrayNode CreateArrayNode(JsonElement element)
    {
        var children = new List<JsonNode>();
        foreach (var item in element.EnumerateArray())
        {
            children.Add(FromElement(item));
        }

        return new JsonArrayNode(children);
    }

    private static JsonPropertyNode CreatePropertyNode(JsonProperty property)
    {
        var name = property.Name;
        var valueNode = FromElement(property.Value);
        return new JsonPropertyNode(name, valueNode);
    }

    private static JsonValueNode CreateValueNode(JsonElement element)
    {
        var nodeType = GetValueNodeType(element);
        var rawValue = GetRawValue(element);
        var displayValue = GetDisplayValue(element);
        return new JsonValueNode(nodeType, rawValue, displayValue);
    }

    internal static JsonNodeType GetValueNodeType(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.String => ClassifyStringValue(element.GetString()),
            JsonValueKind.Number => ClassifyNumberValue(element),
            JsonValueKind.True or JsonValueKind.False => JsonNodeType.Boolean,
            JsonValueKind.Null => JsonNodeType.Null,
            JsonValueKind.Object => JsonNodeType.Object,
            JsonValueKind.Array => JsonNodeType.Array,
            _ => JsonNodeType.String,
        };

    private static string? GetRawValue(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            _ => element.GetRawText(),
        };

    private static string GetDisplayValue(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.String => $"\"{element.GetString()}\"",
            JsonValueKind.Null => "null",
            _ => element.GetRawText(),
        };

    private static JsonNodeType ClassifyStringValue(string? value)
    {
        if (value is null)
        {
            return JsonNodeType.Null;
        }

        if (Guid.TryParse(value, out _))
        {
            return JsonNodeType.Guid;
        }

        if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            return JsonNodeType.Uri;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
        {
            return JsonNodeType.Date;
        }

        return JsonNodeType.String;
    }

    private static JsonNodeType ClassifyNumberValue(JsonElement element)
        => element.TryGetInt64(out _)
            ? JsonNodeType.Integer
            : JsonNodeType.Float;
}