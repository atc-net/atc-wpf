namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled slider control.
/// </summary>
public interface ILabelSlider : ILabelControl
{
    /// <summary>
    /// Gets or sets the maximum value of the slider.
    /// </summary>
    int Maximum { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the slider.
    /// </summary>
    int Minimum { get; set; }

    /// <summary>
    /// Gets or sets the current value of the slider.
    /// </summary>
    int Value { get; set; }

    /// <summary>
    /// Gets or sets where the value tooltip is shown while dragging.
    /// </summary>
    AutoToolTipPlacement AutoToolTipPlacement { get; set; }

    /// <summary>
    /// Gets or sets the interval between tick marks.
    /// </summary>
    int TickFrequency { get; set; }

    /// <summary>
    /// Gets or sets where tick marks are drawn relative to the track.
    /// </summary>
    TickPlacement TickPlacement { get; set; }
}