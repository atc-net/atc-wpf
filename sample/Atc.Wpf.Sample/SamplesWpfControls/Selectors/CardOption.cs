namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// One option of the CardSelector sample: a key, a title, a description, a Segoe MDL2 glyph and an optional price.
/// </summary>
public sealed class CardOption
{
    public CardOption(
        string id,
        string title,
        string description,
        string glyph,
        string? price = null,
        bool isAvailable = true)
    {
        Id = id;
        Title = title;
        Description = description;
        Glyph = glyph;
        Price = price;
        IsAvailable = isAvailable;
    }

    public string Id { get; }

    public string Title { get; }

    public string Description { get; }

    public string Glyph { get; }

    public string? Price { get; }

    public bool IsAvailable { get; }
}