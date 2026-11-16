namespace Atc.Wpf.Displays;

/// <summary>
/// The thread-safe model of the picture of a colour graphic display (a TFT or colour OLED): a pixel buffer of 24-bit colours
/// and the display state (display on/off, inverted, sleeping, brightness).
/// </summary>
/// <remarks>
/// The pixel (x, y) is stored as <c>0x00RRGGBB</c> at index <c>y * Width + x</c>.
/// All members are thread-safe; <see cref="Changed"/> is raised on the thread that made the change, never while the buffer is locked.
/// </remarks>
public sealed class ColorFrameBuffer
{
    /// <summary>
    /// The largest width and height, in pixels.
    /// </summary>
    public const int MaxSize = 480;

    private readonly Lock sync = new();
    private readonly DisplayChangeNotifier notifier;
    private int[] pixels;
    private int width;
    private int height;
    private bool isDisplayOn = true;
    private bool isInverted;
    private bool isSleeping;
    private byte brightness = byte.MaxValue;
    private int dirtyFirst = int.MaxValue;
    private int dirtyLast = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorFrameBuffer"/> class with all pixels black.
    /// </summary>
    /// <param name="width">The width in pixels, 1 to <see cref="MaxSize"/>.</param>
    /// <param name="height">The height in pixels, 1 to <see cref="MaxSize"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The size is out of range.</exception>
    public ColorFrameBuffer(
        int width = 128,
        int height = 160)
    {
        ValidateSize(width, height);

        this.width = width;
        this.height = height;
        pixels = new int[width * height];
        notifier = new DisplayChangeNotifier(sync, RaiseChanged);
        MarkAllDirty();
    }

    /// <summary>
    /// Occurs when the picture or the state of the buffer has changed; once per batch, or once per change outside a batch.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the width in pixels.
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
    /// Gets the height in pixels.
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
    /// Gets or sets a value indicating whether the display is on; the default is <see langword="true"/>.
    /// </summary>
    public bool IsDisplayOn
    {
        get
        {
            lock (sync)
            {
                return isDisplayOn;
            }
        }

        set
        {
            lock (sync)
            {
                if (isDisplayOn == value)
                {
                    return;
                }

                isDisplayOn = value;
                MarkAllDirty();
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the display shows the inverse of the picture.
    /// </summary>
    public bool IsInverted
    {
        get
        {
            lock (sync)
            {
                return isInverted;
            }
        }

        set
        {
            lock (sync)
            {
                if (isInverted == value)
                {
                    return;
                }

                isInverted = value;
                MarkAllDirty();
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the display is in sleep mode.
    /// </summary>
    public bool IsSleeping
    {
        get
        {
            lock (sync)
            {
                return isSleeping;
            }
        }

        set
        {
            lock (sync)
            {
                if (isSleeping == value)
                {
                    return;
                }

                isSleeping = value;
                MarkAllDirty();
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the brightness, 0 to 255; the default is 255.
    /// </summary>
    public byte Brightness
    {
        get
        {
            lock (sync)
            {
                return brightness;
            }
        }

        set
        {
            lock (sync)
            {
                if (brightness == value)
                {
                    return;
                }

                brightness = value;
                MarkAllDirty();
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Changes the size of the buffer; the pixels that fit in both the old and the new size are kept.
    /// </summary>
    /// <param name="width">The width in pixels, 1 to <see cref="MaxSize"/>.</param>
    /// <param name="height">The height in pixels, 1 to <see cref="MaxSize"/>.</param>
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

            var resized = new int[width * height];
            var keptWidth = System.Math.Min(this.width, width);
            var keptHeight = System.Math.Min(this.height, height);
            for (var y = 0; y < keptHeight; y++)
            {
                Array.Copy(pixels, y * this.width, resized, y * width, keptWidth);
            }

            this.width = width;
            this.height = height;
            pixels = resized;
            MarkAllDirty();
        }

        NotifyChanged();
    }

    /// <summary>
    /// Starts a batch: <see cref="Changed"/> is raised once, if anything changed, when the last open batch is disposed.
    /// </summary>
    /// <returns>The scope to dispose to end the batch.</returns>
    public IDisposable BeginBatch()
        => notifier.BeginBatch();

    /// <summary>
    /// Replaces the whole picture with RGB565 pixels given as bytes, row by row from the top left.
    /// </summary>
    /// <param name="data">The pixels, two bytes each: <see cref="Width"/> times <see cref="Height"/> times 2 bytes.</param>
    /// <param name="order">The byte order of a pixel.</param>
    /// <exception cref="ArgumentException">The length is not <see cref="Width"/> times <see cref="Height"/> times 2; the picture is left unchanged.</exception>
    public void LoadFrameRgb565(
        ReadOnlySpan<byte> data,
        Rgb565ByteOrder order = Rgb565ByteOrder.BigEndian)
    {
        bool changed;

        lock (sync)
        {
            if (data.Length != pixels.Length * 2)
            {
                throw new ArgumentException(
                    $"A frame of {width} x {height} pixels is {pixels.Length * 2} bytes, but {data.Length} bytes were given.",
                    nameof(data));
            }

            var bigEndian = order == Rgb565ByteOrder.BigEndian;
            changed = false;
            for (var y = 0; y < height; y++)
            {
                var rowChanged = false;
                var index = y * width;
                var source = index * 2;
                for (var x = 0; x < width; x++, index++, source += 2)
                {
                    var value = bigEndian
                        ? (data[source] << 8) | data[source + 1]
                        : (data[source + 1] << 8) | data[source];
                    var rgb = Expand565(value);
                    if (pixels[index] != rgb)
                    {
                        pixels[index] = rgb;
                        rowChanged = true;
                    }
                }

                if (rowChanged)
                {
                    MarkDirty(y, y);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Replaces the whole picture with RGB565 pixels, row by row from the top left.
    /// </summary>
    /// <param name="rgb565">The pixels, <see cref="Width"/> times <see cref="Height"/> values.</param>
    /// <exception cref="ArgumentException">The length is not <see cref="Width"/> times <see cref="Height"/>; the picture is left unchanged.</exception>
    public void LoadFrame(ReadOnlySpan<ushort> rgb565)
    {
        bool changed;

        lock (sync)
        {
            if (rgb565.Length != pixels.Length)
            {
                throw new ArgumentException(
                    $"A frame of {width} x {height} pixels is {pixels.Length} values, but {rgb565.Length} values were given.",
                    nameof(rgb565));
            }

            changed = false;
            for (var y = 0; y < height; y++)
            {
                var rowChanged = false;
                var index = y * width;
                for (var x = 0; x < width; x++, index++)
                {
                    var rgb = Expand565(rgb565[index]);
                    if (pixels[index] != rgb)
                    {
                        pixels[index] = rgb;
                        rowChanged = true;
                    }
                }

                if (rowChanged)
                {
                    MarkDirty(y, y);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets a pixel.
    /// </summary>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    /// <returns>The colour of the pixel in the picture, fully opaque.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The pixel is outside the buffer.</exception>
    public Color GetPixel(
        int x,
        int y)
    {
        lock (sync)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(x);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, width);
            ArgumentOutOfRangeException.ThrowIfNegative(y);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, height);

            var value = pixels[(y * width) + x];
            return Color.FromRgb(
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value);
        }
    }

    /// <summary>
    /// Sets a pixel; a pixel outside the buffer is ignored.
    /// </summary>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    /// <param name="color">The colour; its alpha is ignored.</param>
    public void SetPixel(
        int x,
        int y,
        Color color)
    {
        bool changed;

        lock (sync)
        {
            changed = SetCore(x, y, ToRgb(color));
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Sets all pixels to one colour.
    /// </summary>
    /// <param name="color">The colour; its alpha is ignored.</param>
    public void Clear(Color color)
    {
        bool changed;

        lock (sync)
        {
            var rgb = ToRgb(color);
            changed = pixels.AsSpan().ContainsAnyExcept(rgb);
            if (changed)
            {
                Array.Fill(pixels, rgb);
                MarkAllDirty();
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Draws a filled rectangle; the part outside the buffer is clipped.
    /// </summary>
    /// <param name="x">The column of the left edge.</param>
    /// <param name="y">The row of the top edge.</param>
    /// <param name="width">The width in pixels; nothing is drawn when it is not positive.</param>
    /// <param name="height">The height in pixels; nothing is drawn when it is not positive.</param>
    /// <param name="color">The colour; its alpha is ignored.</param>
    public void FillRectangle(
        int x,
        int y,
        int width,
        int height,
        Color color)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        bool changed;

        lock (sync)
        {
            changed = FillCore(x, y, width, height, ToRgb(color));
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Draws a line, both ends included (Bresenham); the part outside the buffer is clipped.
    /// </summary>
    /// <param name="x0">The column of the first end.</param>
    /// <param name="y0">The row of the first end.</param>
    /// <param name="x1">The column of the second end.</param>
    /// <param name="y1">The row of the second end.</param>
    /// <param name="color">The colour; its alpha is ignored.</param>
    public void DrawLine(
        int x0,
        int y0,
        int x1,
        int y1,
        Color color)
    {
        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, ToRgb(color));
            changed = DisplayRasterizer.DrawLine(ref sink, x0, y0, x1, y1, width, height);
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Draws the outline of a rectangle; the part outside the buffer is clipped.
    /// </summary>
    /// <param name="x">The column of the left edge.</param>
    /// <param name="y">The row of the top edge.</param>
    /// <param name="width">The width in pixels; nothing is drawn when it is not positive.</param>
    /// <param name="height">The height in pixels; nothing is drawn when it is not positive.</param>
    /// <param name="color">The colour; its alpha is ignored.</param>
    public void DrawRectangle(
        int x,
        int y,
        int width,
        int height,
        Color color)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, ToRgb(color));
            changed = DisplayRasterizer.DrawRectangleOutline(ref sink, x, y, width, height, this.width, this.height);
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Draws text with the 5 x 8 glyphs of an HD44780 character ROM; the part outside the buffer is clipped.
    /// </summary>
    /// <param name="x">The column of the left edge of the first glyph.</param>
    /// <param name="y">The row of the top edge of the first glyph.</param>
    /// <param name="text">The text; a glyph advances 6 pixels times <paramref name="scale"/>, <c>\n</c> moves down 8 rows times <paramref name="scale"/> and back to <paramref name="x"/>, <c>\r</c> is ignored.</param>
    /// <param name="color">The colour of the glyph dots; its alpha is ignored. The background is left as it is.</param>
    /// <param name="scale">The size of a glyph dot in pixels, 1 or more.</param>
    /// <param name="rom">The character ROM that maps the characters to glyphs; a character outside the ROM draws its blank glyph.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> is less than 1.</exception>
    public void DrawText(
        int x,
        int y,
        string text,
        Color color,
        int scale = 1,
        CharacterRom rom = CharacterRom.A00)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 1);

        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, ToRgb(color));
            changed = DisplayRasterizer.DrawText(ref sink, x, y, text, rom, scale);
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    // Writes a run of 0x00RRGGBB pixels into one row, starting at column x; the part outside the buffer is clipped.
    internal void WriteRow(
        int y,
        int x,
        ReadOnlySpan<int> rgb)
    {
        var changed = false;

        lock (sync)
        {
            if (y < 0 || y >= height || x >= width || rgb.IsEmpty)
            {
                return;
            }

            var skip = x < 0 ? -(long)x : 0;
            if (skip >= rgb.Length)
            {
                return;
            }

            var start = System.Math.Max(x, 0);
            var count = System.Math.Min(rgb.Length - (int)skip, width - start);
            var source = rgb.Slice((int)skip, count);
            var target = pixels.AsSpan((y * width) + start, count);
            if (!source.SequenceEqual(target))
            {
                for (var i = 0; i < count; i++)
                {
                    target[i] = source[i] & 0x00FFFFFF;
                }

                MarkDirty(y, y);
                changed = true;
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    // Takes the range of rows changed since the last call; false when nothing changed.
    internal bool TryTakeDirtyRows(
        out int first,
        out int last)
    {
        lock (sync)
        {
            if (dirtyLast < 0)
            {
                first = 0;
                last = -1;
                return false;
            }

            first = dirtyFirst;
            last = System.Math.Min(dirtyLast, height - 1);
            dirtyFirst = int.MaxValue;
            dirtyLast = -1;
            return true;
        }
    }

    // Copies the rows first..last as Bgr32 into an unmanaged image whose row 0 starts at destination, stride bytes per row,
    // destinationWidth x destinationHeight pixels. Returns false, copying nothing, when the buffer has another size now
    // (it was resized after the image was made); the caller recreates the image and copies everything.
    internal bool TryCopyRows(
        int first,
        int last,
        nint destination,
        int stride,
        int destinationWidth,
        int destinationHeight)
    {
        lock (sync)
        {
            if (destinationWidth != width || destinationHeight != height)
            {
                return false;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(stride, width * 4);
            ArgumentOutOfRangeException.ThrowIfNegative(first);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(last, height - 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(last, first);

            for (var row = first; row <= last; row++)
            {
                Marshal.Copy(pixels, row * width, destination + (nint)((long)row * stride), width);
            }

            return true;
        }
    }

    private static int Expand565(int value)
    {
        var r5 = (value >> 11) & 0x1F;
        var g6 = (value >> 5) & 0x3F;
        var b5 = value & 0x1F;
        var r8 = (r5 << 3) | (r5 >> 2);
        var g8 = (g6 << 2) | (g6 >> 4);
        var b8 = (b5 << 3) | (b5 >> 2);
        return (r8 << 16) | (g8 << 8) | b8;
    }

    private static int ToRgb(Color color)
        => (color.R << 16) | (color.G << 8) | color.B;

    private static void ValidateSize(
        int width,
        int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, MaxSize);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, MaxSize);
    }

    // Must be called with the lock held.
    private void MarkAllDirty()
    {
        dirtyFirst = 0;
        dirtyLast = height - 1;
    }

    // Must be called with the lock held.
    private void MarkDirty(
        int firstRow,
        int lastRow)
    {
        if (firstRow < dirtyFirst)
        {
            dirtyFirst = firstRow;
        }

        if (lastRow > dirtyLast)
        {
            dirtyLast = lastRow;
        }
    }

    // Must be called with the lock held and an index inside the buffer; returns true when the pixel changed.
    private bool SetIndex(
        int index,
        int rgb)
    {
        if (pixels[index] == rgb)
        {
            return false;
        }

        pixels[index] = rgb;
        MarkDirty(index / width, index / width);
        return true;
    }

    // Must be called with the lock held; returns true when the pixel changed.
    private bool SetCore(
        long x,
        long y,
        int rgb)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return false;
        }

        return SetIndex((int)((y * width) + x), rgb);
    }

    // Must be called with the lock held; clips to the buffer and returns true when a pixel changed.
    private bool FillCore(
        long x,
        long y,
        long fillWidth,
        long fillHeight,
        int rgb)
    {
        if (!DisplayRasterizer.TryClip(x, y, fillWidth, fillHeight, width, height, out var firstX, out var lastX, out var firstY, out var lastY))
        {
            return false;
        }

        var changed = false;
        for (var row = firstY; row <= lastY; row++)
        {
            var rowChanged = false;
            var index = (row * width) + firstX;
            for (var column = firstX; column <= lastX; column++, index++)
            {
                if (pixels[index] != rgb)
                {
                    pixels[index] = rgb;
                    rowChanged = true;
                }
            }

            if (rowChanged)
            {
                MarkDirty(row, row);
                changed = true;
            }
        }

        return changed;
    }

    private void NotifyChanged()
        => notifier.Notify();

    private void RaiseChanged()
        => Changed?.Invoke(this, EventArgs.Empty);

    private struct Sink : IPixelSink
    {
        private readonly ColorFrameBuffer buffer;
        private readonly int rgb;

        public Sink(
            ColorFrameBuffer buffer,
            int rgb)
        {
            this.buffer = buffer;
            this.rgb = rgb;
        }

        public readonly bool Plot(
            long x,
            long y)
            => buffer.SetCore(x, y, rgb);

        public readonly bool Fill(
            long x,
            long y,
            long width,
            long height)
            => buffer.FillCore(x, y, width, height, rgb);
    }
}