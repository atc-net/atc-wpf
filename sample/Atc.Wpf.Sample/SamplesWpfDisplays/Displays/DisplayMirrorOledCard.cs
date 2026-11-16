namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A monochrome OLED card: MonoFrame packets load the buffer; Commands, Data, Reset and I2cWrite packets drive an SSD1306 / SSD1309 / SH1106
/// emulator (created on the first such packet) and its I2C decoder.
/// </summary>
public sealed class DisplayMirrorOledCard : DisplayMirrorCard
{
    private readonly DisplayMirrorLazyTarget? emulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorOledCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description: its width and height must fit a <see cref="MonochromeFrameBuffer"/>.</param>
    /// <param name="chip">The chip to emulate, or <see langword="null"/> to take whole frames only.</param>
    /// <param name="color">The pixel colour.</param>
    public DisplayMirrorOledCard(
        byte displayId,
        DisplayMirrorDescribe describe,
        OledChip? chip,
        OledDisplayColor color)
        : base(displayId, describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        Buffer = new MonochromeFrameBuffer(describe.Width, describe.Height);
        Color = color;

        if (chip is { } emulatedChip)
        {
            emulator = new DisplayMirrorLazyTarget(() =>
            {
                var controller = new Ssd1306Controller(Buffer, emulatedChip);
                var decoder = new Ssd1306I2cDecoder(controller);
                return (DisplayMirrorTargets.Combine(DisplayMirrorTargets.For(controller), DisplayMirrorTargets.For(decoder)), controller);
            });
            Target = DisplayMirrorTargets.Combine(DisplayMirrorTargets.For(Buffer), emulator);
        }
        else
        {
            Target = DisplayMirrorTargets.For(Buffer);
        }
    }

    /// <summary>
    /// Gets the buffer the control shows.
    /// </summary>
    public MonochromeFrameBuffer Buffer { get; }

    /// <summary>
    /// Gets the pixel columns.
    /// </summary>
    public int PixelColumns => Buffer.Width;

    /// <summary>
    /// Gets the pixel rows.
    /// </summary>
    public int PixelRows => Buffer.Height;

    /// <summary>
    /// Gets the pixel colour.
    /// </summary>
    public OledDisplayColor Color { get; }

    /// <inheritdoc />
    public override IDisplayMirrorTarget? Target { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            emulator?.Dispose();
        }

        base.Dispose(disposing);
    }
}