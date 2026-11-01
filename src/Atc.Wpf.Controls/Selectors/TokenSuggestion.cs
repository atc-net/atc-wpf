namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A row in the suggestion pop-up of a <see cref="TokenBox"/>; the data context of <see cref="TokenBox.ItemTemplate"/>.
/// The display value is split around the first match of the typed text, so a template can highlight it.
/// </summary>
public sealed class TokenSuggestion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenSuggestion"/> class.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <param name="displayValue">The display value.</param>
    /// <param name="typedText">The typed text to highlight (case-insensitive); empty for no highlight.</param>
    public TokenSuggestion(
        string key,
        string displayValue,
        string typedText)
    {
        Key = key;
        DisplayValue = displayValue ?? string.Empty;

        var index = string.IsNullOrEmpty(typedText)
            ? -1
            : DisplayValue.IndexOf(typedText, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            TextBeforeMatch = DisplayValue;
            MatchedText = string.Empty;
            TextAfterMatch = string.Empty;
            return;
        }

        TextBeforeMatch = DisplayValue[..index];
        MatchedText = DisplayValue.Substring(index, typedText.Length);
        TextAfterMatch = DisplayValue[(index + typedText.Length)..];
    }

    /// <summary>
    /// Gets the item key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the display value.
    /// </summary>
    public string DisplayValue { get; }

    /// <summary>
    /// Gets the part of the display value before the match.
    /// </summary>
    public string TextBeforeMatch { get; }

    /// <summary>
    /// Gets the matched part of the display value (empty when the typed text does not occur in it).
    /// </summary>
    public string MatchedText { get; }

    /// <summary>
    /// Gets the part of the display value after the match.
    /// </summary>
    public string TextAfterMatch { get; }

    /// <inheritdoc />
    public override string ToString()
        => DisplayValue;
}