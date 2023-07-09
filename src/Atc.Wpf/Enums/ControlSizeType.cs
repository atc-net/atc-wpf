// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// This enum is used for controlling how the control is stretched to fill.
/// </summary>
public enum ControlSizeType
{
    /// <summary>
    /// The image is not scaled. The image location is translated so the top left corner
    /// of the image bounding box is moved to the top left corner of the image control.
    /// </summary>
    None = 0,

    /// <summary>
    /// The image is scaled to fit the control without any stretching.
    /// Either X or Y direction will be scaled to fill the entire width or height.
    /// </summary>
    ContentToSizeNoStretch = 1,

    /// <summary>
    /// The image will be stretched to fill the entire width and height.
    /// </summary>
    ContentToSizeStretch = 2,

    /// <summary>
    /// The control will be resized to fit the un-scaled image. If the image is larger than the
    /// maximum size for the control, the control is set to maximum size and the image is scaled.
    /// </summary>
    SizeToContent = 3,
}