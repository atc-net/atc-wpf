namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A common ST7920 128 x 64 graphic LCD module and its colour.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="Color">The colour preset.</param>
public sealed record GraphicLcdDisplayPreset(
    string Name,
    CharacterDisplayColor Color)
{
    /// <summary>
    /// Gets the presets: the blue and the yellow-green module.
    /// </summary>
    public static IReadOnlyList<GraphicLcdDisplayPreset> All { get; } =
    [
        new("12864 128 x 64 ST7920 (blue)", CharacterDisplayColor.Blue),
        new("12864 128 x 64 ST7920 (yellow-green)", CharacterDisplayColor.YellowGreen),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}