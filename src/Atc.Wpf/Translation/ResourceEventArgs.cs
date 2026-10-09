namespace Atc.Wpf.Translation;

/// <summary>
/// Provides data for events that concern a resource key in a resource (resx) file.
/// </summary>
public sealed class ResourceEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceEventArgs"/> class.
    /// </summary>
    /// <param name="resxName">The name of the resource file.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="uiCulture">The UI culture.</param>
    public ResourceEventArgs(
        string resxName,
        string key,
        CultureInfo uiCulture)
    {
        ResxName = resxName;
        Key = key;
        UiCulture = uiCulture;
    }

    /// <summary>
    /// Gets the name of the resource file.
    /// </summary>
    public string ResxName { get; }

    /// <summary>
    /// Gets the resource key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the UI culture.
    /// </summary>
    public CultureInfo UiCulture { get; }
}