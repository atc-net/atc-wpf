namespace Atc.Wpf.Displays;

/// <summary>
/// The thread-safe model of an e-paper panel (an SSD1680-style black/white or black/white/red display): the controller RAM
/// (a black plane and a red plane) and, separately, the image the panel shows, which changes only at a <see cref="Refresh"/>.
/// </summary>
/// <remarks>
/// <para>
/// Both planes use the SSD1680 RAM layout in the native (portrait) orientation: each row is <c>ceil(Width / 8)</c> bytes, bit 7 of a byte
/// is the leftmost of its 8 pixels and the padding bits past <see cref="Width"/> are ignored. In the black plane a bit of 0 is black and 1 is
/// white; in the red plane a bit of 1 is red and 0 is not red. A pixel is shown red when its red bit is 1 and <see cref="HasRed"/> is set,
/// otherwise black or white from its black bit: on a black/white panel the red plane is not an image (a driver may keep the previous
/// picture there) and is never shown.
/// </para>
/// <para>
/// Writing the RAM changes nothing visible and raises no event. <see cref="Refresh"/> copies the RAM to the shown image and raises
/// <see cref="Refreshed"/> once per call, also when the picture is the same, so a control can play the refresh; <see cref="Changed"/>
/// is raised when the shown image, the size or <see cref="HasRed"/> changes.
/// All members are thread-safe; the events are raised on the thread that made the change, never while the buffer is locked,
/// and inside a <see cref="BeginBatch"/> scope they are raised when the outermost scope ends (<see cref="Changed"/> once, then each
/// <see cref="Refreshed"/> in order).
/// </para>
/// </remarks>
public sealed class EPaperFrameBuffer
{
    /// <summary>
    /// The largest width, in pixels.
    /// </summary>
    public const int MaxWidth = 400;

    /// <summary>
    /// The largest height, in pixels.
    /// </summary>
    public const int MaxHeight = 400;

    private const byte WhiteByte = 0xFF;
    private const byte NoRedByte = 0x00;

    private readonly Lock sync = new();
    private readonly DisplayChangeNotifier notifier;
    private readonly List<EPaperRefreshKind> pendingRefreshes = [];
    private int width;
    private int height;
    private bool hasRed;
    private byte[] ramBlack;
    private byte[] ramRed;
    private byte[] shownBlack;
    private byte[] shownRed;
    private bool isChangedPending;
    private long fullRefreshCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="EPaperFrameBuffer"/> class with white RAM and a white picture, no red.
    /// </summary>
    /// <param name="width">The native (portrait) width in pixels, 1 to <see cref="MaxWidth"/>; 128 for the 2.9" and the 2.13" panels (the 2.13" glass shows 122 of them, see <see cref="EPaperDisplay.VisibleColumns"/>).</param>
    /// <param name="height">The native height in pixels, 1 to <see cref="MaxHeight"/>; 296 for the 2.9" panel, 250 for the 2.13" panel.</param>
    /// <param name="hasRed">Whether the panel is black/white/red; <see langword="false"/> for a black/white panel.</param>
    /// <exception cref="ArgumentOutOfRangeException">The size is out of range.</exception>
    public EPaperFrameBuffer(
        int width = 128,
        int height = 296,
        bool hasRed = false)
    {
        ValidateSize(width, height);

        this.width = width;
        this.height = height;
        this.hasRed = hasRed;
        ramBlack = CreatePlane(width, height, WhiteByte);
        ramRed = CreatePlane(width, height, NoRedByte);
        shownBlack = CreatePlane(width, height, WhiteByte);
        shownRed = CreatePlane(width, height, NoRedByte);
        notifier = new DisplayChangeNotifier(sync, RaisePending);
    }

    /// <summary>
    /// Occurs when the shown image, the size or <see cref="HasRed"/> has changed; once per batch, or once per change outside a batch.
    /// Writes to the RAM do not raise it.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Occurs once for every <see cref="Refresh"/>, after <see cref="Changed"/> when the same call or batch also changed the picture.
    /// </summary>
    public event EventHandler<EPaperRefreshedEventArgs>? Refreshed;

    /// <summary>
    /// Gets the native width in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            lock (sync)
            {
                return width;
            }
        }
    }

    /// <summary>
    /// Gets the native height in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            lock (sync)
            {
                return height;
            }
        }
    }

    /// <summary>
    /// Gets the number of bytes in a row of a plane: the width divided by 8, rounded up.
    /// </summary>
    public int BytesPerRow
    {
        get
        {
            lock (sync)
            {
                return BytesPerRowOf(width);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the panel is black/white/red. When it is <see langword="false"/> the red plane is kept
    /// but not shown.
    /// </summary>
    public bool HasRed
    {
        get
        {
            lock (sync)
            {
                return hasRed;
            }
        }

        set
        {
            lock (sync)
            {
                if (hasRed == value)
                {
                    return;
                }

                hasRed = value;
                isChangedPending = true;
            }

            notifier.Notify();
        }
    }

    /// <summary>
    /// Changes the size of the buffer; the RAM and shown pixels that fit in both the old and the new size are kept, the rest is white
    /// with no red.
    /// </summary>
    /// <param name="width">The native width in pixels, 1 to <see cref="MaxWidth"/>.</param>
    /// <param name="height">The native height in pixels, 1 to <see cref="MaxHeight"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The size is out of range.</exception>
    public void Resize(
        int width,
        int height)
    {
        ValidateSize(width, height);

        lock (sync)
        {
            if (this.width == width &&
                this.height == height)
            {
                return;
            }

            ramBlack = ResizePlane(ramBlack, this.width, this.height, width, height, WhiteByte);
            ramRed = ResizePlane(ramRed, this.width, this.height, width, height, NoRedByte);
            shownBlack = ResizePlane(shownBlack, this.width, this.height, width, height, WhiteByte);
            shownRed = ResizePlane(shownRed, this.width, this.height, width, height, NoRedByte);
            this.width = width;
            this.height = height;
            isChangedPending = true;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Starts a batch: <see cref="Changed"/> (once, if anything visible changed) and <see cref="Refreshed"/> (once per refresh, in order)
    /// are raised when the last open batch is disposed.
    /// </summary>
    /// <returns>The scope to dispose to end the batch.</returns>
    public IDisposable BeginBatch()
        => notifier.BeginBatch();

    /// <summary>
    /// Writes one byte (8 pixels) of a RAM plane; nothing visible changes until the next <see cref="Refresh"/>.
    /// A byte outside the buffer is ignored.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="byteX">The byte column: pixels <c>8 * byteX</c> to <c>8 * byteX + 7</c>, bit 7 the leftmost.</param>
    /// <param name="y">The row.</param>
    /// <param name="value">The byte, in the polarity of <paramref name="plane"/>.</param>
    public void WriteRam(
        EPaperPlane plane,
        int byteX,
        int y,
        byte value)
    {
        lock (sync)
        {
            var bytesPerRow = BytesPerRowOf(width);
            if (byteX < 0 || byteX >= bytesPerRow || y < 0 || y >= height)
            {
                return;
            }

            PlaneOf(plane)[(y * bytesPerRow) + byteX] = value;
        }
    }

    /// <summary>
    /// Replaces the RAM planes; nothing visible changes until the next <see cref="Refresh"/>.
    /// </summary>
    /// <param name="black">The black plane, <see cref="BytesPerRow"/> times <see cref="Height"/> bytes; bit 0 = black, 1 = white.</param>
    /// <param name="red">
    /// The red plane, the same length; bit 1 = red. Empty to leave the red plane as it is.
    /// </param>
    /// <exception cref="ArgumentException">A plane has the wrong length; the RAM is left unchanged.</exception>
    public void LoadRam(
        ReadOnlySpan<byte> black,
        ReadOnlySpan<byte> red)
    {
        lock (sync)
        {
            var length = ramBlack.Length;
            if (black.Length != length)
            {
                throw new ArgumentException(
                    $"A plane of {width} x {height} pixels is {length} bytes, but {black.Length} bytes were given.",
                    nameof(black));
            }

            if (!red.IsEmpty &&
                red.Length != length)
            {
                throw new ArgumentException(
                    $"A plane of {width} x {height} pixels is {length} bytes, but {red.Length} bytes were given.",
                    nameof(red));
            }

            black.CopyTo(ramBlack);
            if (!red.IsEmpty)
            {
                red.CopyTo(ramRed);
            }
        }
    }

    /// <summary>
    /// Shows the RAM: copies both planes to the shown image and raises <see cref="Refreshed"/> (and <see cref="Changed"/> when the
    /// shown image changed).
    /// </summary>
    /// <param name="kind">The kind of the refresh, passed on in <see cref="Refreshed"/>.</param>
    public void Refresh(EPaperRefreshKind kind)
    {
        lock (sync)
        {
            if (!AreVisibleBitsEqual(ramBlack, shownBlack) ||
                (hasRed && !AreVisibleBitsEqual(ramRed, shownRed)))
            {
                isChangedPending = true;
            }

            ramBlack.CopyTo(shownBlack, 0);
            ramRed.CopyTo(shownRed, 0);
            pendingRefreshes.Add(kind);
            if (kind == EPaperRefreshKind.Full)
            {
                fullRefreshCount++;
            }
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets a pixel of the shown image.
    /// </summary>
    /// <param name="x">The native column.</param>
    /// <param name="y">The native row.</param>
    /// <returns>The colour the panel shows.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The pixel is outside the buffer.</exception>
    public EPaperInk GetShownPixel(
        int x,
        int y)
    {
        lock (sync)
        {
            ThrowIfOutside(x, y);

            var index = (y * BytesPerRowOf(width)) + (x / 8);
            var mask = 0x80 >> (x % 8);
            if (hasRed && (shownRed[index] & mask) != 0)
            {
                return EPaperInk.Red;
            }

            return (shownBlack[index] & mask) == 0
                ? EPaperInk.Black
                : EPaperInk.White;
        }
    }

    /// <summary>
    /// Gets the raw bit of a pixel in a RAM plane.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="x">The native column.</param>
    /// <param name="y">The native row.</param>
    /// <returns>The bit: in the black plane <see langword="true"/> is white, in the red plane <see langword="true"/> is red.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The pixel is outside the buffer.</exception>
    public bool GetRamPixel(
        EPaperPlane plane,
        int x,
        int y)
    {
        lock (sync)
        {
            ThrowIfOutside(x, y);

            return (PlaneOf(plane)[(y * BytesPerRowOf(width)) + (x / 8)] & (0x80 >> (x % 8))) != 0;
        }
    }

    /// <summary>
    /// Copies the shown image under one lock: both shown planes, the size, <see cref="HasRed"/> and the number of full refreshes so far.
    /// The arrays are reused when they have the plane length and replaced otherwise.
    /// </summary>
    /// <param name="black">The black plane copy, <see cref="BytesPerRow"/> times <see cref="Height"/> bytes after the call.</param>
    /// <param name="red">The red plane copy, the same length.</param>
    /// <returns>The state read together with the planes.</returns>
    internal EPaperShownSnapshot CopyShown(
        ref byte[] black,
        ref byte[] red)
    {
        lock (sync)
        {
            if (black.Length != shownBlack.Length)
            {
                black = new byte[shownBlack.Length];
            }

            if (red.Length != shownRed.Length)
            {
                red = new byte[shownRed.Length];
            }

            shownBlack.CopyTo(black, 0);
            shownRed.CopyTo(red, 0);
            return new EPaperShownSnapshot(width, height, hasRed, fullRefreshCount);
        }
    }

    private static int BytesPerRowOf(int width)
        => (width + 7) / 8;

    private static byte[] CreatePlane(
        int width,
        int height,
        byte fill)
    {
        var plane = new byte[BytesPerRowOf(width) * height];
        Array.Fill(plane, fill);
        return plane;
    }

    // Keeps the pixels inside both sizes; the rest of the new plane is the fill (padding bits are ignored either way).
    private static byte[] ResizePlane(
        byte[] plane,
        int oldWidth,
        int oldHeight,
        int newWidth,
        int newHeight,
        byte fill)
    {
        var oldBytesPerRow = BytesPerRowOf(oldWidth);
        var newBytesPerRow = BytesPerRowOf(newWidth);
        var resized = CreatePlane(newWidth, newHeight, fill);
        var keptRows = System.Math.Min(oldHeight, newHeight);
        var keptWidth = System.Math.Min(oldWidth, newWidth);
        for (var y = 0; y < keptRows; y++)
        {
            for (var x = 0; x < keptWidth; x++)
            {
                var mask = 0x80 >> (x % 8);
                var isSet = (plane[(y * oldBytesPerRow) + (x / 8)] & mask) != 0;
                ref var target = ref resized[(y * newBytesPerRow) + (x / 8)];
                target = isSet
                    ? (byte)(target | mask)
                    : (byte)(target & ~mask);
            }
        }

        return resized;
    }

    private static void ValidateSize(
        int width,
        int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, MaxWidth);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, MaxHeight);
    }

    // Must be called with the lock held.
    private byte[] PlaneOf(EPaperPlane plane)
        => plane == EPaperPlane.Red
            ? ramRed
            : ramBlack;

    // Must be called with the lock held.
    private void ThrowIfOutside(
        int x,
        int y)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, width);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, height);
    }

    // Compares two planes of this size ignoring the padding bits past Width in the last byte of each row (§SSD1680.4 item 6). Under the lock.
    private bool AreVisibleBitsEqual(
        byte[] left,
        byte[] right)
    {
        var paddingBits = (BytesPerRowOf(width) * 8) - width;
        if (paddingBits == 0)
        {
            return left.AsSpan().SequenceEqual(right);
        }

        var bytesPerRow = BytesPerRowOf(width);
        var lastByteMask = (byte)(0xFF << paddingBits);
        for (var row = 0; row < height; row++)
        {
            var start = row * bytesPerRow;
            var lastByte = start + bytesPerRow - 1;
            if (!left.AsSpan(start, bytesPerRow - 1).SequenceEqual(right.AsSpan(start, bytesPerRow - 1)) ||
                ((left[lastByte] ^ right[lastByte]) & lastByteMask) != 0)
            {
                return false;
            }
        }

        return true;
    }

    // Raised by the notifier outside the lock: takes what is pending and raises Changed, then each Refreshed in order.
    private void RaisePending()
    {
        bool isChanged;
        EPaperRefreshKind[] refreshes;

        lock (sync)
        {
            isChanged = isChangedPending;
            isChangedPending = false;
            refreshes = pendingRefreshes.Count == 0
                ? []
                : [.. pendingRefreshes];
            pendingRefreshes.Clear();
        }

        if (isChanged)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        foreach (var kind in refreshes)
        {
            Refreshed?.Invoke(this, new EPaperRefreshedEventArgs(kind));
        }
    }
}