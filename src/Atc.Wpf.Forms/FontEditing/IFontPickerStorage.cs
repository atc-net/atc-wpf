namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Provides storage of recently used and favorite font family names for the FontPicker controls.
/// Implementations can persist to disk, registry, or any application-specific store.
/// </summary>
public interface IFontPickerStorage
{
    /// <summary>
    /// Returns the most-recently-used font family source names, in descending order
    /// (most recent first).
    /// </summary>
    IReadOnlyList<string> GetRecentFontFamilies();

    /// <summary>
    /// Records that the given font family was selected by the user.
    /// Implementations should de-duplicate and cap the list at a sensible size.
    /// </summary>
    void RecordRecentFontFamily(string fontFamilySource);
}