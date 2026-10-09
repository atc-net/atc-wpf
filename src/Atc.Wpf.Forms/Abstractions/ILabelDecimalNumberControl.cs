namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for labeled decimal input controls.
/// </summary>
public interface ILabelDecimalNumberControl : ILabelNumberControl
{
    /// <summary>
    /// Gets or sets the number of decimal places shown.
    /// </summary>
    int DecimalPlaces { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    decimal Maximum { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    decimal Minimum { get; set; }
}