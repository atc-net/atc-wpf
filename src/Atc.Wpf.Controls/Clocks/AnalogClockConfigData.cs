namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// The colours and options of an <see cref="AnalogClock"/>, for example to save them in settings.
/// </summary>
public class AnalogClockConfigData
{
    /// <summary>
    /// Gets or sets the colours.
    /// </summary>
    public AnalogClockColorsConfigData Colors { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the AM/PM indicator is shown.
    /// </summary>
    public bool ShowAmPm { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the second hand is shown.
    /// </summary>
    public bool ShowSeconds { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the numbers 1-12 are shown.
    /// </summary>
    public bool ShowNumbers { get; set; } = true;
}