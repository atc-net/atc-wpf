namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A target that creates a chip emulator (and its bus decoder) on the first bus packet (Commands, Data, Reset, I2cWrite, Latch) it is given.
/// </summary>
/// <remarks>
/// A chip emulator applies the chip's power-on state to its buffer when it is created (display off, RAM cleared), so a card that is
/// fed whole frames only must not create one up front: the frames would load into a panel that is switched off.
/// Describe and frame packets are never taken here, so they fall through to the frame target before this one in a combined target.
/// </remarks>
public sealed class DisplayMirrorLazyTarget : IDisplayMirrorTarget, IDisposable
{
    private readonly Lock sync = new();
    private readonly Func<(IDisplayMirrorTarget Target, IDisposable? Owner)> create;
    private IDisplayMirrorTarget? target;
    private IDisposable? owner;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorLazyTarget"/> class.
    /// </summary>
    /// <param name="create">Creates the emulator target and the object to dispose with it (the emulator), on the receiver's thread.</param>
    public DisplayMirrorLazyTarget(
        Func<(IDisplayMirrorTarget Target, IDisposable? Owner)> create)
    {
        ArgumentNullException.ThrowIfNull(create);

        this.create = create;
    }

    /// <inheritdoc />
    public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
    {
        if (packet.Type == DisplayMirrorMessageType.Describe ||
            DisplayMirrorProtocol.IsFrameType(packet.Type))
        {
            return DisplayMirrorRejectReason.Unsupported;
        }

        IDisplayMirrorTarget current;
        lock (sync)
        {
            if (isDisposed)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            if (target is null)
            {
                (target, owner) = create();
            }

            current = target;
        }

        // A disposed emulator is inert, so a packet applied while the card is replaced does no harm.
        return current.Apply(packet);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (sync)
        {
            isDisposed = true;
            owner?.Dispose();
            owner = null;
            target = null;
        }
    }
}