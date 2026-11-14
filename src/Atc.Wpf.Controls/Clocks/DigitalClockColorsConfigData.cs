namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// The colours of a <see cref="DigitalClock"/> as hex text (#AARRGGBB).
/// An empty value means the theme brush (or no brush for <see cref="Background"/>).
/// </summary>
public class DigitalClockColorsConfigData
{
    /// <summary>
    /// Gets or sets the background colour of the control.
    /// </summary>
    public string? Background { get; set; }

    /// <summary>
    /// Gets or sets the colour of the digits.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the colour of the colons.
    /// </summary>
    public string? Colon { get; set; }
}