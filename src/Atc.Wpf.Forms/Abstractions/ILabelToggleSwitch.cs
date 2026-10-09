namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled toggle switch control.
/// </summary>
public interface ILabelToggleSwitch : ILabelControlBase
{
    /// <summary>
    /// Gets or sets the flow direction of the switch content.
    /// </summary>
    FlowDirection ContentDirection { get; set; }

    /// <summary>
    /// Gets or sets the text shown when the switch is off.
    /// </summary>
    string? OffText { get; set; }

    /// <summary>
    /// Gets or sets the text shown when the switch is on.
    /// </summary>
    string? OnText { get; set; }

    /// <summary>
    /// Gets or sets the content shown next to the switch.
    /// </summary>
    string? OffOnContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the switch is on.
    /// </summary>
    bool IsOn { get; set; }
}