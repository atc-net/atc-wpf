namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies how the LEDs of an 8 x 8 MAX7219 matrix module are wired to the chip's digit registers and segment bits,
/// named after the MD_MAX72XX module types.
/// </summary>
/// <remarks>
/// For a dot in row <c>r</c> (0 = top) and module column <c>x</c> (0 = leftmost LED of the module) the register is (§MAX7219.7):
/// <list type="bullet">
/// <item><description><see cref="Fc16"/>: digit <c>r</c>, bit <c>7 - x</c>.</description></item>
/// <item><description><see cref="Generic"/>: digit <c>7 - x</c>, bit <c>7 - r</c>.</description></item>
/// <item><description><see cref="Parola"/>: digit <c>r</c>, bit <c>x</c>.</description></item>
/// <item><description><see cref="ICStation"/>: digit <c>7 - r</c>, bit <c>x</c>.</description></item>
/// </list>
/// For every type the first device of the chain (nearest the microcontroller's DIN) is the rightmost module.
/// </remarks>
public enum Max7219MatrixWiring
{
    /// <summary>
    /// The FC-16 module (MD_MAX72XX <c>FC16_HW</c>): digits are rows, bit 7 is the leftmost LED. Also the orientation of LedControl and U8g2.
    /// </summary>
    Fc16,

    /// <summary>
    /// The generic module (MD_MAX72XX <c>GENERIC_HW</c>): digits are columns (digit 7 leftmost), bit 7 is the top LED.
    /// </summary>
    Generic,

    /// <summary>
    /// The Parola module (MD_MAX72XX <c>PAROLA_HW</c>): digits are rows, bit 0 is the leftmost LED.
    /// </summary>
    Parola,

    /// <summary>
    /// The ICStation module (MD_MAX72XX <c>ICSTATION_HW</c>): digit 7 is the top row, bit 0 is the leftmost LED.
    /// </summary>
    ICStation,
}