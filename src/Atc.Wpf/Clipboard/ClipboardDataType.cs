namespace Atc.Wpf.Clipboard;

/// <summary>
/// Specifies the type of data stored in a clipboard entry.
/// </summary>
public enum ClipboardDataType
{
    /// <summary>
    /// Plain text data.
    /// </summary>
    Text,

    /// <summary>
    /// Image (bitmap) data.
    /// </summary>
    Image,

    /// <summary>
    /// A list of file paths.
    /// </summary>
    FileDropList,

    /// <summary>
    /// Data in another (custom) format.
    /// </summary>
    Other,
}