namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// The library whose MAX7219 traffic the LED matrix sample replays.
/// </summary>
public enum LedMatrixReplayLibrary
{
    /// <summary>
    /// MD_MAX72XX: <c>begin()</c>, then the text through <c>setBuffer</c> (one frame per digit row for every device), in the module's wiring.
    /// </summary>
    MdMax72xx,

    /// <summary>
    /// LedControl: the constructor (which leaves the chips shut down), <c>shutdown(false)</c>, <c>setIntensity</c> and <c>setRow</c> per device;
    /// always in the FC-16 orientation.
    /// </summary>
    LedControl,

    /// <summary>
    /// U8g2: its init, power save off and the tile rows; always in the FC-16 orientation.
    /// </summary>
    U8g2,
}