namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for labeled input controls with mandatory and validation support.
/// </summary>
public interface ILabelControl : ILabelControlBase
{
    /// <summary>
    /// Gets or sets a value indicating whether an asterisk is shown when the control is mandatory.
    /// </summary>
    bool ShowAsteriskOnMandatory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a value is required.
    /// </summary>
    bool IsMandatory { get; set; }

    /// <summary>
    /// Gets or sets the brush used for the mandatory indicator.
    /// </summary>
    SolidColorBrush MandatoryColor { get; set; }

    /// <summary>
    /// Gets or sets the brush used for the validation message.
    /// </summary>
    SolidColorBrush ValidationColor { get; set; }

    /// <summary>
    /// Gets or sets the validation message shown below the input.
    /// </summary>
    string ValidationText { get; set; }

    /// <summary>
    /// Determines whether the current value of the control is valid.
    /// </summary>
    /// <returns><see langword="true"/> if the value is valid; otherwise, <see langword="false"/>.</returns>
    bool IsValid();
}