namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for labeled integer input controls.
/// </summary>
public interface ILabelIntegerNumberControl : ILabelNumberControl
{
    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    int Maximum { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    int Minimum { get; set; }
}