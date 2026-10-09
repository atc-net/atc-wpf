namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled input control for a pair of decimal X and Y values.
/// </summary>
public interface ILabelDecimalXyBox : ILabelDecimalNumberControl
{
    /// <summary>
    /// Gets or sets the text shown before the X value.
    /// </summary>
    string PrefixTextX { get; set; }

    /// <summary>
    /// Gets or sets the text shown before the Y value.
    /// </summary>
    string PrefixTextY { get; set; }

    /// <summary>
    /// Gets or sets the text shown after the values.
    /// </summary>
    string SuffixText { get; set; }

    /// <summary>
    /// Gets or sets the X value.
    /// </summary>
    decimal ValueX { get; set; }

    /// <summary>
    /// Gets or sets the Y value.
    /// </summary>
    decimal ValueY { get; set; }
}