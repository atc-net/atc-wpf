namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// An ST7920 128 x 64 graphic LCD card: MonoFrame packets load the buffer; Data packets are the serial (SPI) byte stream (start bytes
/// <c>0xF8</c> / <c>0xFA</c> and nibble pairs, what the U8g2 tee sends) for an <see cref="St7920SerialDecoder"/>, Commands packets are
/// logical ST7920 instructions for the emulator, and Reset resets the chip and the serial framing. The emulator is created on the first
/// such packet.
/// </summary>
/// <remarks>
/// A device that sends logical instruction and data bytes (not serial-framed) cannot use this card's Data path: register
/// <c>DisplayMirrorTargets.For(controller)</c> instead, which takes Data as logical bytes.
/// </remarks>
public sealed class DisplayMirrorGraphicLcdCard : DisplayMirrorCard
{
    private readonly DisplayMirrorLazyTarget? emulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorGraphicLcdCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description: 128 x 64.</param>
    /// <param name="color">The colour preset.</param>
    /// <param name="isSt7920">Whether to emulate an ST7920; otherwise whole frames only.</param>
    public DisplayMirrorGraphicLcdCard(
        byte displayId,
        DisplayMirrorDescribe describe,
        CharacterDisplayColor color,
        bool isSt7920)
        : base(displayId, describe)
    {
        Buffer = new MonochromeFrameBuffer(GraphicLcdDisplay.PixelColumns, GraphicLcdDisplay.PixelRows);
        Color = color;

        if (isSt7920)
        {
            // The serial decoder comes first, so Data is always taken as serial-framed; Commands fall through to the controller.
            // Latch means nothing to an ST7920 (and the controller target would take its bytes as data), so it stays Unsupported.
            emulator = new DisplayMirrorLazyTarget(() =>
            {
                var controller = new St7920Controller(Buffer);
                var decoder = new St7920SerialDecoder(controller);
                var target = DisplayMirrorTargets.Combine(
                    DisplayMirrorTargets.For(decoder),
                    new NoLatchTarget(DisplayMirrorTargets.For(controller)));
                return (target, controller);
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
    /// Gets the colour preset.
    /// </summary>
    public CharacterDisplayColor Color { get; }

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

    // Passes every packet but Latch to the inner target.
    private sealed class NoLatchTarget(IDisplayMirrorTarget inner) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
            => packet.Type == DisplayMirrorMessageType.Latch
                ? DisplayMirrorRejectReason.Unsupported
                : inner.Apply(packet);
    }
}