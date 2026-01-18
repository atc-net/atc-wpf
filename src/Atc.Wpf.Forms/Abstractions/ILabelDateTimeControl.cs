namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for date/time picker label controls.
/// </summary>
public interface ILabelDateTimeControl : ILabelControl
{
    /// <summary>
    /// Gets or sets the custom culture for date/time formatting.
    /// </summary>
    CultureInfo? CustomCulture { get; set; }

    /// <summary>
    /// Occurs when the control loses focus and the value is valid.
    /// </summary>
    event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusValid;

    /// <summary>
    /// Occurs when the control loses focus and the value is invalid.
    /// </summary>
    event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusInvalid;
}