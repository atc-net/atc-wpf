namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A colour panel card: ColorFrame packets load the buffer; Commands, Data and Reset packets drive a chip emulator (created on the first such packet).
/// </summary>
public sealed class DisplayMirrorColorCard : DisplayMirrorCard
{
    private readonly DisplayMirrorLazyTarget? emulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorColorCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description: its width and height must fit a <see cref="ColorFrameBuffer"/>.</param>
    /// <param name="panelKind">The panel kind.</param>
    /// <param name="createController">Creates the chip emulator on the buffer, or <see langword="null"/> to take whole frames only.</param>
    public DisplayMirrorColorCard(
        byte displayId,
        DisplayMirrorDescribe describe,
        ColorPanelKind panelKind,
        Func<ColorFrameBuffer, IGraphicDisplayController>? createController)
        : base(displayId, describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        Buffer = new ColorFrameBuffer(describe.Width, describe.Height);
        PanelKind = panelKind;

        if (createController is not null)
        {
            emulator = new DisplayMirrorLazyTarget(() =>
            {
                var controller = createController(Buffer);
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
    public ColorFrameBuffer Buffer { get; }

    /// <summary>
    /// Gets the pixel columns.
    /// </summary>
    public int PixelColumns => Buffer.Width;

    /// <summary>
    /// Gets the pixel rows.
    /// </summary>
    public int PixelRows => Buffer.Height;

    /// <summary>
    /// Gets the panel kind.
    /// </summary>
    public ColorPanelKind PanelKind { get; }

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