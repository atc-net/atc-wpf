namespace Atc.Wpf.Controls.Media.W3cSvg;

/// <summary>
/// Frozen drawings of static SVG images, shared by every <see cref="SvgImage"/> that shows the same source with
/// the same render settings (an icon repeated in an item template is parsed and rendered once).
/// </summary>
/// <remarks>
/// Only frozen drawings are stored, so animated output (which cannot be frozen) is never shared, and a cached
/// drawing can be used from any thread. The parsed SVG model is not cached: rendering changes it.
/// When full, the least recently used drawing is evicted, so an app showing more distinct icons than the
/// capacity keeps its frequently shown ones cached.
/// </remarks>
internal static class SvgDrawingCache
{
    private const int MaxEntries = 256;

    private static readonly LruCache<Key, Entry> Entries = new(MaxEntries);

    public static bool TryGet(
        Key key,
        [NotNullWhen(true)] out Entry? entry)
        => Entries.TryGet(key, out entry);

    /// <summary>
    /// Freezes the drawing and brushes and stores them; does nothing when any of them cannot be frozen.
    /// </summary>
    public static void TryAdd(
        Key key,
        Drawing drawing,
        IReadOnlyDictionary<string, Brush> paintServerBrushes)
    {
        if (!drawing.CanFreeze)
        {
            return;
        }

        var frozenBrushes = new Dictionary<string, Brush>(StringComparer.Ordinal);
        foreach (var (name, brush) in paintServerBrushes)
        {
            if (!brush.CanFreeze)
            {
                return;
            }

            frozenBrushes[name] = (Brush)brush.GetAsFrozen();
        }

        drawing.Freeze();

        Entries.Set(key, new Entry(drawing, frozenBrushes));
    }

    /// <summary>
    /// Everything the rendered drawing depends on.
    /// </summary>
    internal readonly record struct Key(
        string SourceIdentity,
        Color? OverrideColor,
        Color? OverrideStrokeColor,
        double? OverrideStrokeWidth,
        bool UseAnimations);

    internal sealed record Entry(
        Drawing Drawing,
        IReadOnlyDictionary<string, Brush> PaintServerBrushes);
}