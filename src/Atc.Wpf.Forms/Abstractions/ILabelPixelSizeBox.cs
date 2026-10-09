namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled input control for a width and height in pixels.
/// </summary>
public interface ILabelPixelSizeBox : ILabelIntegerNumberControl
{
    /// <summary>
    /// Gets or sets the width value.
    /// </summary>
    int ValueWidth { get; set; }

    /// <summary>
    /// Gets or sets the height value.
    /// </summary>
    int ValueHeight { get; set; }
}