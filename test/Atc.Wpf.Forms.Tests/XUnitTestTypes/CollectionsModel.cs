namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class CollectionsModel
{
    public string? Name { get; set; }

    public int Age { get; set; }

    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "The extractor must see a List<T> property.")]
    public List<string> Tags { get; } = [];

    public Dictionary<string, int> Counts { get; } = new(StringComparer.Ordinal);
}