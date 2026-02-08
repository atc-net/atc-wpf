namespace Atc.Wpf.Printing;

/// <summary>
/// Specifies how content is scaled when printing.
/// </summary>
public enum PrintScaleMode
{
    /// <summary>
    /// No scaling is applied. Content is printed at its original size.
    /// </summary>
    None,

    /// <summary>
    /// Content is scaled up or down to fill the printable area.
    /// </summary>
    FitToPage,

    /// <summary>
    /// Content is scaled down only if it exceeds the printable area.
    /// Content smaller than the printable area is not scaled up.
    /// </summary>
    ShrinkToFit,
}