namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies how a <see cref="TokenBox"/> matches the typed text against the display values of its items.
/// </summary>
public enum TokenFilterMode
{
    /// <summary>The display value starts with the typed text (case-insensitive).</summary>
    StartsWith,

    /// <summary>The display value contains the typed text (case-insensitive).</summary>
    Contains,

    /// <summary>The <see cref="TokenBox.CustomFilter"/> decides.</summary>
    Custom,
}