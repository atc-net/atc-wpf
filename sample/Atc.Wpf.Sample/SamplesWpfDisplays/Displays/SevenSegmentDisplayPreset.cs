namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A common MAX7219 seven-segment module: the number of digits and the LED colour.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="DigitCount">The number of digits, 1 to 8.</param>
/// <param name="LedColor">The LED colour.</param>
public sealed record SevenSegmentDisplayPreset(
    string Name,
    int DigitCount,
    LedColor LedColor)
{
    /// <summary>
    /// Gets the presets: 4 and 8 digits.
    /// </summary>
    public static IReadOnlyList<SevenSegmentDisplayPreset> All { get; } =
    [
        new("4 digits (MAX7219, red)", 4, LedColor.Red),
        new("8 digits (MAX7219, red)", 8, LedColor.Red),
        new("8 digits (MAX7219, green)", 8, LedColor.Green),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}