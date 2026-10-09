namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for labeled numeric input controls.
/// </summary>
public interface ILabelNumberControl : ILabelControl
{
    /// <summary>
    /// Gets or sets a value indicating whether the up/down buttons are hidden.
    /// </summary>
    bool HideUpDownButtons { get; set; }
}