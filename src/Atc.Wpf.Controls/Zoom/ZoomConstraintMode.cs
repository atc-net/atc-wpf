namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Specifies how the viewport is constrained relative to content bounds.
/// </summary>
public enum ZoomConstraintMode
{
    /// <summary>
    /// No constraints — the viewport can pan freely past content edges.
    /// </summary>
    Free,

    /// <summary>
    /// The viewport must stay within the content bounds.
    /// Panning stops at the content edges.
    /// </summary>
    Inside,

    /// <summary>
    /// The content must fill the viewport — the user cannot see
    /// empty space outside the content. Constrains both pan and minimum zoom.
    /// </summary>
    Contain,
}