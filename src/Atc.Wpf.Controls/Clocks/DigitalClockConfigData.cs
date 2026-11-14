namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// The colours and options of a <see cref="DigitalClock"/>, for example to save them in settings.
/// </summary>
[System.Xml.Serialization.XmlRoot("DigitalClock")]
public class DigitalClockConfigData
{
    /// <summary>
    /// Gets or sets the colours.
    /// </summary>
    public DigitalClockColorsConfigData Colors { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the colons fade in and out each second.
    /// </summary>
    public bool AnimateColon { get; set; } = true;

    /// <summary>
    /// Gets or sets the blur radius of the glow drawn behind the digits.
    /// </summary>
    public int NumberBlurRadius { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether the seconds are shown.
    /// </summary>
    public bool ShowSeconds { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a change of digit is animated.
    /// </summary>
    public bool PlayAnimation { get; set; } = true;
}