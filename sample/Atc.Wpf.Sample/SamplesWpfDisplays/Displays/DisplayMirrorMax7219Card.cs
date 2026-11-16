namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A MAX7219 card, an LED matrix chain or a seven-segment module: each Latch packet is one CS frame for a MAX7219 emulator
/// (created on the first such packet), and a Reset packet is the power-on reset.
/// </summary>
public sealed class DisplayMirrorMax7219Card : DisplayMirrorCard
{
    private readonly DisplayMirrorLazyTarget emulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorMax7219Card"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description.</param>
    /// <param name="isMatrix"><see langword="true"/> for an LED matrix chain, <see langword="false"/> for a seven-segment module.</param>
    /// <param name="deviceCount">The number of chips in the chain: matrix modules, or 1 for a seven-segment module.</param>
    /// <param name="digitCount">The number of digits a seven-segment module shows.</param>
    /// <param name="wiring">How the LEDs of a matrix module are wired.</param>
    /// <param name="ledColor">The LED colour of a matrix.</param>
    public DisplayMirrorMax7219Card(
        byte displayId,
        DisplayMirrorDescribe describe,
        bool isMatrix,
        int deviceCount,
        int digitCount,
        Max7219MatrixWiring wiring,
        LedColor ledColor)
        : base(displayId, describe)
    {
        IsMatrix = isMatrix;
        Buffer = new Max7219ChainBuffer(deviceCount);
        DigitCount = digitCount;
        Wiring = wiring;
        LedColor = ledColor;

        // The registers start in the power-on state either way, so the emulator is created only when the first packet arrives.
        emulator = new DisplayMirrorLazyTarget(() =>
        {
            var controller = new Max7219Controller(Buffer);
            return (DisplayMirrorTargets.For(controller), controller);
        });
        Target = emulator;
    }

    /// <summary>
    /// Gets a value indicating whether the card shows an LED matrix chain rather than a seven-segment module.
    /// </summary>
    public bool IsMatrix { get; }

    /// <summary>
    /// Gets the registers the control shows.
    /// </summary>
    public Max7219ChainBuffer Buffer { get; }

    /// <summary>
    /// Gets the number of matrix modules.
    /// </summary>
    public int ModuleCount => Buffer.DeviceCount;

    /// <summary>
    /// Gets the number of seven-segment digits.
    /// </summary>
    public int DigitCount { get; }

    /// <summary>
    /// Gets the matrix wiring.
    /// </summary>
    public Max7219MatrixWiring Wiring { get; }

    /// <summary>
    /// Gets the LED colour of a matrix.
    /// </summary>
    public LedColor LedColor { get; }

    /// <inheritdoc />
    public override IDisplayMirrorTarget? Target { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            emulator.Dispose();
        }

        base.Dispose(disposing);
    }
}