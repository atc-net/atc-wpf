namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Describes how the pins of a PCF8574 I2C expander ("backpack") are wired to an HD44780 display.
/// Every property is a bit position, 0 to 7, in the byte written to the expander; the defaults match the common LiquidCrystal_I2C backpack.
/// </summary>
public sealed record Pcf8574PinMap
{
    /// <summary>
    /// Gets the bit position of the register select line (RS): low for instructions, high for data.
    /// </summary>
    public int Rs { get; init; }

    /// <summary>
    /// Gets the bit position of the read/write line (RW): high for read cycles, which are ignored.
    /// </summary>
    public int Rw { get; init; } = 1;

    /// <summary>
    /// Gets the bit position of the enable line (EN): a nibble is latched when it falls from high to low.
    /// </summary>
    public int Enable { get; init; } = 2;

    /// <summary>
    /// Gets the bit position of the backlight transistor.
    /// </summary>
    public int Backlight { get; init; } = 3;

    /// <summary>
    /// Gets the bit position of data line D4; D5, D6 and D7 follow on the next three bit positions.
    /// </summary>
    public int Data4 { get; init; } = 4;
}