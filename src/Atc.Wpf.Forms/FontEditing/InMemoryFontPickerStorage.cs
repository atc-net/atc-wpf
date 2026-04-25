namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Process-lifetime in-memory implementation of <see cref="IFontPickerStorage"/>.
/// Keeps the most recent <see cref="MaxRecentItems"/> font family names; no persistence
/// across application restarts. Apps that want persistence should provide their own
/// implementation and assign it to <see cref="FontPickerStorage.Current"/>.
/// </summary>
public sealed class InMemoryFontPickerStorage : IFontPickerStorage
{
    public const int MaxRecentItems = 8;

    private readonly System.Threading.Lock syncRoot = new();
    private readonly LinkedList<string> recent = new();

    public IReadOnlyList<string> GetRecentFontFamilies()
    {
        lock (syncRoot)
        {
            return recent.ToList();
        }
    }

    public void RecordRecentFontFamily(string fontFamilySource)
    {
        if (string.IsNullOrWhiteSpace(fontFamilySource))
        {
            return;
        }

        lock (syncRoot)
        {
            var existing = recent.Find(fontFamilySource);
            if (existing is not null)
            {
                recent.Remove(existing);
            }

            recent.AddFirst(fontFamilySource);

            while (recent.Count > MaxRecentItems)
            {
                recent.RemoveLast();
            }
        }
    }
}