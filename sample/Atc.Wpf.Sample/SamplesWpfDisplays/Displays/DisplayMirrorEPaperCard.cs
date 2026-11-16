namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// An e-paper card: EPaperFrame packets load the RAM and refresh the buffer; Commands, Data and Reset packets drive an SSD1680 emulator
/// (created on the first such packet).
/// </summary>
public sealed class DisplayMirrorEPaperCard : DisplayMirrorCard
{
    private readonly DisplayMirrorLazyTarget? emulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorEPaperCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description: its native width and height must fit an <see cref="EPaperFrameBuffer"/>.</param>
    /// <param name="hasRed">Whether the panel is black/white/red.</param>
    /// <param name="visibleColumns">The native columns the glass shows (122 on a 2.13" panel sent as 128-pixel-wide frames), or null for all.</param>
    /// <param name="isSsd1680">Whether to emulate an SSD1680 for Commands and Data; otherwise whole frames only.</param>
    public DisplayMirrorEPaperCard(
        byte displayId,
        DisplayMirrorDescribe describe,
        bool hasRed,
        int? visibleColumns,
        bool isSsd1680)
        : base(displayId, describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        VisibleColumns = visibleColumns;
        Buffer = new EPaperFrameBuffer(describe.Width, describe.Height, hasRed);

        if (isSsd1680)
        {
            // GxEPD2 starts the panel at RAM byte 0; an Adafruit_EPD panel with _xram_offset 1 would need RamXOffsetBytes = 1.
            emulator = new DisplayMirrorLazyTarget(() =>
            {
                var controller = new Ssd1680Controller(Buffer);
                return (DisplayMirrorTargets.For(controller), controller);
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
    public EPaperFrameBuffer Buffer { get; }

    /// <summary>
    /// Gets the native pixel columns.
    /// </summary>
    public int PixelColumns => Buffer.Width;

    /// <summary>
    /// Gets the native pixel rows.
    /// </summary>
    public int PixelRows => Buffer.Height;

    /// <summary>
    /// Gets the native columns the glass shows, or null for all of them.
    /// </summary>
    public int? VisibleColumns { get; }

    /// <summary>
    /// Gets a value indicating whether the panel is black/white/red.
    /// </summary>
    public bool HasRed => Buffer.HasRed;

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