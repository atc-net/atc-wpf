namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// The thread-safe model of the picture of a monochrome graphic display (an SSD1306-style OLED): a page-organised pixel buffer
/// and the display state (display on/off, inverted, entire display on, contrast).
/// </summary>
/// <remarks>
/// The buffer is stored like the display RAM of the chip: pages of 8 rows, each page <see cref="Width"/> bytes, and in a byte the least significant bit
/// is the top row of the page. The pixel (x, y) is bit <c>y % 8</c> of the byte <c>(y / 8) * Width + x</c>.
/// All members are thread-safe; <see cref="Changed"/> is raised on the thread that made the change, never while the buffer is locked.
/// </remarks>
public sealed class MonochromeFrameBuffer
{
    /// <summary>
    /// The largest width, in pixels: the 132 columns of the display RAM of an SH1106.
    /// </summary>
    public const int MaxWidth = 132;

    /// <summary>
    /// The largest height, in pixels.
    /// </summary>
    public const int MaxHeight = 64;

    private const int PageHeight = 8;
    private const byte DefaultContrast = 0x7F;

    private readonly Lock sync = new();
    private readonly DisplayChangeNotifier notifier;
    private byte[] pages;
    private int width;
    private int height;
    private bool isDisplayOn = true;
    private bool isInverted;
    private bool isEntireDisplayOn;
    private byte contrast = DefaultContrast;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonochromeFrameBuffer"/> class with all pixels off.
    /// </summary>
    /// <param name="width">The width in pixels, 1 to <see cref="MaxWidth"/>.</param>
    /// <param name="height">The height in pixels, 1 to <see cref="MaxHeight"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The size is out of range.</exception>
    public MonochromeFrameBuffer(
        int width = 128,
        int height = 64)
    {
        ValidateSize(width, height);

        this.width = width;
        this.height = height;
        pages = new byte[width * PageCountOf(height)];
        notifier = new DisplayChangeNotifier(sync, RaiseChanged);
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
    /// Gets the number of pages: the height in pixels divided by 8, rounded up.
    /// </summary>
    public int PageCount
    {
        get
        {
            lock (sync)
            {
                return PageCountOf(height);
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
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the display lights every pixel regardless of the picture (the chip's entire display on command).
    /// </summary>
    public bool IsEntireDisplayOn
    {
        get
        {
            lock (sync)
            {
                return isEntireDisplayOn;
            }
        }

        set
        {
            lock (sync)
            {
                if (isEntireDisplayOn == value)
                {
                    return;
                }

                isEntireDisplayOn = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the contrast, 0 to 255; the default is 0x7F.
    /// </summary>
    public byte Contrast
    {
        get
        {
            lock (sync)
            {
                return contrast;
            }
        }

        set
        {
            lock (sync)
            {
                if (contrast == value)
                {
                    return;
                }

                contrast = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Changes the size of the buffer; the pixels that fit in both the old and the new size are kept.
    /// </summary>
    /// <param name="width">The width in pixels, 1 to <see cref="MaxWidth"/>.</param>
    /// <param name="height">The height in pixels, 1 to <see cref="MaxHeight"/>.</param>
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

            var resized = new byte[width * PageCountOf(height)];
            var keptWidth = System.Math.Min(this.width, width);
            var keptHeight = System.Math.Min(this.height, height);
            for (var y = 0; y < keptHeight; y++)
            {
                for (var x = 0; x < keptWidth; x++)
                {
                    if (GetCore(x, y))
                    {
                        resized[((y / PageHeight) * width) + x] |= (byte)(1 << (y % PageHeight));
                    }
                }
            }

            this.width = width;
            this.height = height;
            pages = resized;
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
    /// Replaces the whole picture with display RAM content.
    /// </summary>
    /// <param name="pages">The pages in order, each <see cref="Width"/> bytes, the least significant bit of a byte on top.</param>
    /// <exception cref="ArgumentException">The length is not <see cref="Width"/> times <see cref="PageCount"/>; the picture is left unchanged.</exception>
    public void LoadFrame(ReadOnlySpan<byte> pages)
    {
        bool changed;

        lock (sync)
        {
            if (pages.Length != this.pages.Length)
            {
                throw new ArgumentException(
                    $"A frame of {width} x {height} pixels is {this.pages.Length} bytes, but {pages.Length} bytes were given.",
                    nameof(pages));
            }

            changed = !pages.SequenceEqual(this.pages);
            if (changed)
            {
                pages.CopyTo(this.pages);
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets a copy of the display RAM content.
    /// </summary>
    /// <returns>The pages in order, each <see cref="Width"/> bytes, the least significant bit of a byte on top.</returns>
    public byte[] GetFrame()
    {
        lock (sync)
        {
            return (byte[])pages.Clone();
        }
    }

    /// <summary>
    /// Gets a pixel.
    /// </summary>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    /// <returns><see langword="true"/> when the pixel is on in the picture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The pixel is outside the buffer.</exception>
    public bool GetPixel(
        int x,
        int y)
    {
        lock (sync)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(x);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, width);
            ArgumentOutOfRangeException.ThrowIfNegative(y);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, height);

            return GetCore(x, y);
        }
    }

    /// <summary>
    /// Sets a pixel; a pixel outside the buffer is ignored.
    /// </summary>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    /// <param name="isOn">Whether the pixel is on.</param>
    public void SetPixel(
        int x,
        int y,
        bool isOn = true)
    {
        bool changed;

        lock (sync)
        {
            changed = SetCore(x, y, isOn);
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    /// <summary>
    /// Turns all pixels off.
    /// </summary>
    public void Clear()
        => FillAll(0x00);

    /// <summary>
    /// Turns all pixels on.
    /// </summary>
    public void Fill()
        => FillAll(0xFF);

    /// <summary>
    /// Draws a line, both ends included (Bresenham); the part outside the buffer is clipped.
    /// </summary>
    /// <param name="x0">The column of the first end.</param>
    /// <param name="y0">The row of the first end.</param>
    /// <param name="x1">The column of the second end.</param>
    /// <param name="y1">The row of the second end.</param>
    /// <param name="isOn">Whether the pixels are turned on or off.</param>
    public void DrawLine(
        int x0,
        int y0,
        int x1,
        int y1,
        bool isOn = true)
    {
        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, isOn);
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
    /// <param name="isOn">Whether the pixels are turned on or off.</param>
    public void DrawRectangle(
        int x,
        int y,
        int width,
        int height,
        bool isOn = true)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, isOn);
            changed = DisplayRasterizer.DrawRectangleOutline(ref sink, x, y, width, height, this.width, this.height);
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
    /// <param name="isOn">Whether the pixels are turned on or off.</param>
    public void FillRectangle(
        int x,
        int y,
        int width,
        int height,
        bool isOn = true)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, isOn);
            changed = sink.Fill(x, y, width, height);
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
    /// <param name="text">The text; a glyph advances 6 pixels, <c>\n</c> moves down 8 rows and back to <paramref name="x"/>, <c>\r</c> is ignored.</param>
    /// <param name="rom">The character ROM that maps the characters to glyphs; a character outside the ROM draws its blank glyph.</param>
    /// <param name="isOn">Whether the glyph dots are turned on or off; the background is left as it is.</param>
    public void DrawText(
        int x,
        int y,
        string text,
        CharacterRom rom = CharacterRom.A00,
        bool isOn = true)
    {
        ArgumentNullException.ThrowIfNull(text);

        bool changed;

        lock (sync)
        {
            var sink = new Sink(this, isOn);
            changed = DisplayRasterizer.DrawText(ref sink, x, y, text, rom, 1);
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    private static int PageCountOf(int height)
        => (height + PageHeight - 1) / PageHeight;

    private static void ValidateSize(
        int width,
        int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, MaxWidth);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, MaxHeight);
    }

    private void FillAll(byte value)
    {
        bool changed;

        lock (sync)
        {
            changed = pages.AsSpan().ContainsAnyExcept(value);
            if (changed)
            {
                Array.Fill(pages, value);
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    // Must be called with the lock held and a pixel inside the buffer.
    private bool GetCore(
        int x,
        int y)
        => (pages[((y / PageHeight) * width) + x] & (1 << (y % PageHeight))) != 0;

    // Must be called with the lock held; returns true when the pixel changed.
    private bool SetCore(
        long x,
        long y,
        bool isOn)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return false;
        }

        var index = (int)(((y / PageHeight) * width) + x);
        var mask = (byte)(1 << (int)(y % PageHeight));
        var old = pages[index];
        var updated = isOn
            ? (byte)(old | mask)
            : (byte)(old & ~mask);

        if (updated == old)
        {
            return false;
        }

        pages[index] = updated;
        return true;
    }

    private void NotifyChanged()
        => notifier.Notify();

    private void RaiseChanged()
        => Changed?.Invoke(this, EventArgs.Empty);

    private struct Sink : IPixelSink
    {
        private readonly MonochromeFrameBuffer buffer;
        private readonly bool isOn;

        public Sink(
            MonochromeFrameBuffer buffer,
            bool isOn)
        {
            this.buffer = buffer;
            this.isOn = isOn;
        }

        public readonly bool Plot(
            long x,
            long y)
            => buffer.SetCore(x, y, isOn);

        public readonly bool Fill(
            long x,
            long y,
            long width,
            long height)
        {
            if (!DisplayRasterizer.TryClip(x, y, width, height, buffer.width, buffer.height, out var firstX, out var lastX, out var firstY, out var lastY))
            {
                return false;
            }

            var changed = false;
            for (var row = firstY; row <= lastY; row++)
            {
                for (var column = firstX; column <= lastX; column++)
                {
                    changed |= buffer.SetCore(column, row, isOn);
                }
            }

            return changed;
        }
    }
}