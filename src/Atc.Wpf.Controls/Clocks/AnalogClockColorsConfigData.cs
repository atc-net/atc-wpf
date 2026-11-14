namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// The colours of an <see cref="AnalogClock"/> as hex text (#AARRGGBB).
/// An empty value means the theme brush (or no brush for <see cref="Background"/>).
/// </summary>
public class AnalogClockColorsConfigData
{
    /// <summary>
    /// Gets or sets the background colour of the control (behind the clock face).
    /// </summary>
    public string? Background { get; set; }

    /// <summary>
    /// Gets or sets the colour of the clock face.
    /// </summary>
    public string? ClockBackground { get; set; }

    /// <summary>
    /// Gets or sets the colour of the ring around the clock face.
    /// </summary>
    public string? ClockBorder { get; set; }

    /// <summary>
    /// Gets or sets the colour of the dot in the centre.
    /// </summary>
    public string? ClockCenterDot { get; set; }

    /// <summary>
    /// Gets or sets the colour of the hour indicators.
    /// </summary>
    public string? HourIndicator { get; set; }

    /// <summary>
    /// Gets or sets the colour of the minute indicators.
    /// </summary>
    public string? MinuteIndicator { get; set; }

    /// <summary>
    /// Gets or sets the colour of the numbers.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the colour of the hour hand.
    /// </summary>
    public string? HourHand { get; set; }

    /// <summary>
    /// Gets or sets the colour of the minute hand.
    /// </summary>
    public string? MinuteHand { get; set; }

    /// <summary>
    /// Gets or sets the colour of the second hand.
    /// </summary>
    public string? SecondHand { get; set; }

    /// <summary>
    /// Gets or sets the colour of the AM/PM indicator.
    /// </summary>
    public string? AmPm { get; set; }
}