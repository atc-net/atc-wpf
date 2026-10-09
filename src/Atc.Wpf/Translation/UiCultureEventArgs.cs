namespace Atc.Wpf.Translation;

/// <summary>
/// Provides data for the event raised when the UI culture changes.
/// </summary>
public sealed class UiCultureEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UiCultureEventArgs"/> class.
    /// </summary>
    /// <param name="oldCulture">The previous UI culture.</param>
    /// <param name="newCulture">The new UI culture.</param>
    public UiCultureEventArgs(
        CultureInfo oldCulture,
        CultureInfo newCulture)
    {
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }

    /// <summary>
    /// Gets the previous UI culture.
    /// </summary>
    public CultureInfo OldCulture { get; }

    /// <summary>
    /// Gets the new UI culture.
    /// </summary>
    public CultureInfo NewCulture { get; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(OldCulture)}: {OldCulture}, {nameof(NewCulture)}: {NewCulture}";
}