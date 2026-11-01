namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Provides data for <see cref="TokenBox.TokenCreating"/>: a typed or pasted value that matches no item is about to
/// become a new token.
/// </summary>
public sealed class TokenCreatingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenCreatingEventArgs"/> class.
    /// </summary>
    /// <param name="text">The trimmed text of the new value; also the default key.</param>
    public TokenCreatingEventArgs(string text)
    {
        Text = text;
        Key = text;
    }

    /// <summary>
    /// Gets the trimmed text of the new value (its display value).
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets or sets the key of the new token; the text itself unless changed.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is refused (no token is created; the text stays in the field).
    /// </summary>
    public bool Cancel { get; set; }
}