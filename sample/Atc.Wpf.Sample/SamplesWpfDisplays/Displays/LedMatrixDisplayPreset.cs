namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A common MAX7219 8 x 8 LED matrix chain: the number of modules, how they are wired and the LED colour.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="ModuleCount">The number of modules (MAX7219 devices), 1 to 8.</param>
/// <param name="Wiring">How the LEDs of a module are wired.</param>
/// <param name="LedColor">The LED colour.</param>
public sealed record LedMatrixDisplayPreset(
    string Name,
    int ModuleCount,
    Max7219MatrixWiring Wiring,
    LedColor LedColor)
{
    /// <summary>
    /// Gets the presets: 1, 4 and 8 modules, each wiring and the LED colours.
    /// </summary>
    public static IReadOnlyList<LedMatrixDisplayPreset> All { get; } =
    [
        new("8 x 8 matrix, 1 module (FC-16, red)", 1, Max7219MatrixWiring.Fc16, LedColor.Red),
        new("32 x 8 matrix, 4 modules (FC-16, red)", 4, Max7219MatrixWiring.Fc16, LedColor.Red),
        new("32 x 8 matrix, 4 modules (Generic, green)", 4, Max7219MatrixWiring.Generic, LedColor.Green),
        new("32 x 8 matrix, 4 modules (Parola, blue)", 4, Max7219MatrixWiring.Parola, LedColor.Blue),
        new("32 x 8 matrix, 4 modules (ICStation, yellow)", 4, Max7219MatrixWiring.ICStation, LedColor.Yellow),
        new("64 x 8 matrix, 8 modules (FC-16, white)", 8, Max7219MatrixWiring.Fc16, LedColor.White),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}