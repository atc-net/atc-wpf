namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled color picker control.
/// </summary>
public interface ILabelColorPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    Color? ColorValue { get; set; }

    /// <summary>
    /// Gets or sets the selected color as a brush.
    /// </summary>
    SolidColorBrush? BrushValue { get; set; }
}