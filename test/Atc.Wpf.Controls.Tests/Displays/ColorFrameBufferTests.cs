namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class ColorFrameBufferTests
{
    [Fact]
    public void Constructor_OutOfRangeSize_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ColorFrameBuffer(0, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ColorFrameBuffer(10, ColorFrameBuffer.MaxSize + 1));
    }

    [Fact]
    public void LoadFrameRgb565_BigEndian_ExpandsTheBits()
    {
        var buffer = new ColorFrameBuffer(4, 1);

        buffer.LoadFrameRgb565([0xF8, 0x00, 0x07, 0xE0, 0x00, 0x1F, 0x84, 0x10]);

        Assert.Equal(Color.FromRgb(255, 0, 0), buffer.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(0, 255, 0), buffer.GetPixel(1, 0));
        Assert.Equal(Color.FromRgb(0, 0, 255), buffer.GetPixel(2, 0));
        Assert.Equal(Color.FromRgb(132, 130, 132), buffer.GetPixel(3, 0));
        Assert.Equal(255, buffer.GetPixel(0, 0).A);
    }

    [Fact]
    public void LoadFrameRgb565_LittleEndian_SwapsTheBytes()
    {
        var buffer = new ColorFrameBuffer(2, 1);

        buffer.LoadFrameRgb565([0x00, 0xF8, 0xE0, 0x07], Rgb565ByteOrder.LittleEndian);

        Assert.Equal(Color.FromRgb(255, 0, 0), buffer.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(0, 255, 0), buffer.GetPixel(1, 0));
    }

    [Fact]
    public void LoadFrame_Ushorts_ExpandsTheBits()
    {
        var buffer = new ColorFrameBuffer(2, 1);

        buffer.LoadFrame([0xF800, 0x8410]);

        Assert.Equal(Color.FromRgb(255, 0, 0), buffer.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(132, 130, 132), buffer.GetPixel(1, 0));
        Assert.Throws<ArgumentException>(() => buffer.LoadFrame(new ushort[3]));
    }

    [Fact]
    public void LoadFrameRgb565_WrongLength_ThrowsAndKeepsThePicture()
    {
        var buffer = new ColorFrameBuffer(4, 2);
        buffer.SetPixel(0, 0, Colors.Red);

        var exception = Assert.Throws<ArgumentException>(() => buffer.LoadFrameRgb565(new byte[15]));

        Assert.Contains("16", exception.Message, StringComparison.Ordinal);
        Assert.Equal(Colors.Red, buffer.GetPixel(0, 0));
    }

    [Fact]
    public void GetPixel_OutsideTheBuffer_Throws()
    {
        var buffer = new ColorFrameBuffer(4, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPixel(4, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPixel(0, -1));
    }

    [Fact]
    public void DrawText_Scale2_DoublesTheGlyph()
    {
        var one = new ColorFrameBuffer(32, 32);
        var two = new ColorFrameBuffer(32, 32);

        one.DrawText(0, 0, "A", Colors.White);
        two.DrawText(0, 0, "A", Colors.White, scale: 2);

        var any = false;
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                var expected = one.GetPixel(x, y);
                any |= expected == Colors.White;
                Assert.Equal(expected, two.GetPixel(x * 2, y * 2));
                Assert.Equal(expected, two.GetPixel((x * 2) + 1, y * 2));
                Assert.Equal(expected, two.GetPixel(x * 2, (y * 2) + 1));
                Assert.Equal(expected, two.GetPixel((x * 2) + 1, (y * 2) + 1));
            }
        }

        Assert.True(any);
        Assert.Throws<ArgumentOutOfRangeException>(() => one.DrawText(0, 0, "A", Colors.White, scale: 0));
    }

    [Fact]
    public void Drawing_IsClipped()
    {
        var buffer = new ColorFrameBuffer(8, 8);

        buffer.SetPixel(-1, 0, Colors.Red);
        buffer.FillRectangle(-5, -5, 7, 7, Colors.Lime);
        buffer.DrawRectangle(5, 5, 10, 10, Colors.Blue);
        buffer.DrawLine(-1_000_000, -1_000_000, 1_000_000, 1_000_000, Colors.White);
        buffer.DrawLine(0, 5_000_000, 7, 5_000_000, Colors.White);
        buffer.DrawText(6, 6, "ABC\nDEF", Colors.White, scale: 3);

        Assert.Equal(Colors.Lime, buffer.GetPixel(0, 1));
        Assert.Equal(Colors.Blue, buffer.GetPixel(5, 7));
        Assert.Equal(Colors.White, buffer.GetPixel(3, 3));
        Assert.Equal(Colors.Black, buffer.GetPixel(0, 7));
    }

    [Fact]
    public void UnchangedWrites_RaiseNoChanged()
    {
        var buffer = new ColorFrameBuffer(4, 4);
        buffer.SetPixel(1, 1, Colors.Red);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        buffer.SetPixel(1, 1, Colors.Red);
        buffer.FillRectangle(1, 1, 1, 1, Colors.Red);
        buffer.Clear(Colors.Black);
        buffer.IsDisplayOn = true;
        buffer.Brightness = 255;
        buffer.SetPixel(99, 99, Colors.Red);

        Assert.Equal(1, count);
    }

    [Fact]
    public void DirtyRows_CoverOnlyTheWrittenRows()
    {
        var buffer = new ColorFrameBuffer(8, 8);
        Assert.True(buffer.TryTakeDirtyRows(out var first, out var last));
        Assert.Equal((0, 7), (first, last));
        Assert.False(buffer.TryTakeDirtyRows(out _, out _));

        buffer.SetPixel(1, 3, Colors.Red);
        buffer.SetPixel(2, 5, Colors.Red);

        Assert.True(buffer.TryTakeDirtyRows(out first, out last));
        Assert.Equal((3, 5), (first, last));
        Assert.False(buffer.TryTakeDirtyRows(out _, out _));

        buffer.SetPixel(1, 3, Colors.Red);
        Assert.False(buffer.TryTakeDirtyRows(out _, out _));

        buffer.WriteRow(6, 0, [0xFF0000, 0x00FF00]);
        Assert.True(buffer.TryTakeDirtyRows(out first, out last));
        Assert.Equal((6, 6), (first, last));
        Assert.Equal(Colors.Lime, buffer.GetPixel(1, 6));
    }

    [Fact]
    public void DirtyRows_StateChangeAndResizeMarkEverything()
    {
        var buffer = new ColorFrameBuffer(8, 8);
        buffer.TryTakeDirtyRows(out _, out _);

        buffer.IsInverted = true;
        Assert.True(buffer.TryTakeDirtyRows(out var first, out var last));
        Assert.Equal((0, 7), (first, last));

        buffer.Brightness = 10;
        Assert.True(buffer.TryTakeDirtyRows(out _, out _));

        buffer.Resize(8, 4);
        Assert.True(buffer.TryTakeDirtyRows(out first, out last));
        Assert.Equal((0, 3), (first, last));
    }

    [Fact]
    public void WriteRow_ClipsToTheBuffer()
    {
        var buffer = new ColorFrameBuffer(4, 2);

        buffer.WriteRow(0, -1, [0x111111, 0x222222, 0x333333]);
        buffer.WriteRow(1, 3, [0x444444, 0x555555]);
        buffer.WriteRow(5, 0, [0x666666]);

        Assert.Equal(Color.FromRgb(0x22, 0x22, 0x22), buffer.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(0x33, 0x33, 0x33), buffer.GetPixel(1, 0));
        Assert.Equal(Color.FromRgb(0x44, 0x44, 0x44), buffer.GetPixel(3, 1));
    }

    [Fact]
    public void TryCopyRows_WritesBgr32WithTheStrideAndTouchesNothingElse()
    {
        var buffer = new ColorFrameBuffer(2, 3);
        buffer.SetPixel(0, 1, Color.FromRgb(1, 2, 3));
        buffer.SetPixel(1, 2, Color.FromRgb(4, 5, 6));
        const int stride = 12;
        var memory = Marshal.AllocHGlobal((stride * 3) + 16);
        try
        {
            for (var i = 0; i < (stride * 3) + 16; i++)
            {
                Marshal.WriteByte(memory, i, 0xEE);
            }

            Assert.True(buffer.TryCopyRows(1, 2, memory, stride, 2, 3));

            Assert.Equal(0xEE, Marshal.ReadByte(memory, 0));
            Assert.Equal(0xEE, Marshal.ReadByte(memory, stride - 1));
            Assert.Equal(3, Marshal.ReadByte(memory, stride));
            Assert.Equal(2, Marshal.ReadByte(memory, stride + 1));
            Assert.Equal(1, Marshal.ReadByte(memory, stride + 2));
            Assert.Equal(6, Marshal.ReadByte(memory, (2 * stride) + 4));
            Assert.Equal(5, Marshal.ReadByte(memory, (2 * stride) + 5));
            Assert.Equal(4, Marshal.ReadByte(memory, (2 * stride) + 6));

            // The stride padding and everything past the last row stay untouched.
            Assert.Equal(0xEE, Marshal.ReadByte(memory, stride + 8));
            for (var i = 3 * stride; i < (3 * stride) + 16; i++)
            {
                Assert.Equal(0xEE, Marshal.ReadByte(memory, i));
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(0, 3, memory, stride, 2, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(-1, 1, memory, stride, 2, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(2, 1, memory, stride, 2, 3));
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    [Fact]
    public void TryCopyRows_BadStride_ThrowsAndWritesNothing()
    {
        var buffer = new ColorFrameBuffer(4, 2);
        buffer.Clear(Colors.White);
        var memory = Marshal.AllocHGlobal(64);
        try
        {
            for (var i = 0; i < 64; i++)
            {
                Marshal.WriteByte(memory, i, 0xEE);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(0, 1, memory, 15, 4, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(0, 1, memory, -16, 4, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.TryCopyRows(0, 1, memory, int.MinValue, 4, 2));

            for (var i = 0; i < 64; i++)
            {
                Assert.Equal(0xEE, Marshal.ReadByte(memory, i));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    [Fact]
    public void TryCopyRows_AfterResize_ReturnsFalseAndWritesNothing()
    {
        var buffer = new ColorFrameBuffer(4, 4);
        buffer.Clear(Colors.White);
        var memory = Marshal.AllocHGlobal(64);
        try
        {
            for (var i = 0; i < 64; i++)
            {
                Marshal.WriteByte(memory, i, 0xEE);
            }

            buffer.Resize(8, 8);

            Assert.False(buffer.TryCopyRows(0, 3, memory, 16, 4, 4));

            for (var i = 0; i < 64; i++)
            {
                Assert.Equal(0xEE, Marshal.ReadByte(memory, i));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    [Fact]
    public void DrawText_AdvancesSixTimesScaleAndNewlineMovesEightTimesScale()
    {
        var single = new ColorFrameBuffer(64, 64);
        single.DrawText(0, 0, "A", Colors.White, scale: 2);
        var pair = new ColorFrameBuffer(64, 64);
        pair.DrawText(0, 0, "AA", Colors.White, scale: 2);
        var lines = new ColorFrameBuffer(64, 64);
        lines.DrawText(0, 0, "A\nA", Colors.White, scale: 2);

        for (var y = 0; y < 16; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                var expected = single.GetPixel(x, y);
                Assert.Equal(expected, pair.GetPixel(x + 12, y));
                Assert.Equal(expected, lines.GetPixel(x, y + 16));
            }
        }
    }

    [Fact]
    public void DrawText_HugeScaleOrFarOrigin_IsClippedWithoutWrapping()
    {
        var buffer = new ColorFrameBuffer(8, 8);

        buffer.DrawText(0, 0, "A", Colors.White, scale: int.MaxValue);
        buffer.DrawText(int.MaxValue - 3, 0, "AA", Colors.Red);
        buffer.DrawText(0, int.MaxValue - 3, "AA", Colors.Red, scale: 2);
        buffer.DrawText(int.MinValue, int.MinValue, "AA", Colors.Red, scale: int.MaxValue);

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                Assert.NotEqual(Colors.Red, buffer.GetPixel(x, y));
            }
        }
    }

    [Fact]
    public void DrawLine_ProducesTheExactBresenhamPixels()
    {
        var buffer = new ColorFrameBuffer(8, 8);

        buffer.DrawLine(0, 0, 6, 2, Colors.White);

        var expected = new HashSet<(int, int)> { (0, 0), (1, 0), (2, 1), (3, 1), (4, 1), (5, 2), (6, 2) };
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                Assert.Equal(expected.Contains((x, y)), buffer.GetPixel(x, y) == Colors.White);
            }
        }
    }

    [Fact]
    public void DrawRectangle_DrawsOnlyTheOutline()
    {
        var buffer = new ColorFrameBuffer(8, 8);

        buffer.DrawRectangle(1, 2, 4, 3, Colors.White);

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var onOutline = x is >= 1 and <= 4 && y is >= 2 and <= 4 && (x is 1 or 4 || y is 2 or 4);
                Assert.Equal(onOutline, buffer.GetPixel(x, y) == Colors.White);
            }
        }
    }

    [Fact]
    public void BeginBatch_RaisesChangedOnce()
    {
        var buffer = new ColorFrameBuffer(16, 16);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        using (buffer.BeginBatch())
        {
            buffer.SetPixel(1, 1, Colors.Red);
            buffer.DrawLine(0, 0, 10, 10, Colors.Lime);
            buffer.IsInverted = true;
            Assert.Equal(0, count);
        }

        Assert.Equal(1, count);

        using (buffer.BeginBatch())
        {
            buffer.SetPixel(1, 1, Colors.Lime);
        }

        Assert.Equal(1, count);
    }

    [Fact]
    public void WritesFromManyThreads_KeepEveryPixelRowAndChange()
    {
        const int size = 64;
        var buffer = new ColorFrameBuffer(size, size);
        buffer.TryTakeDirtyRows(out _, out _);
        var changes = 0;
        buffer.Changed += (_, _) => Interlocked.Increment(ref changes);

        Parallel.For(
            0,
            size * size,
            i =>
            {
                var x = i % size;
                var y = i / size;
                buffer.SetPixel(x, y, Color.FromRgb((byte)x, (byte)y, 7));
            });

        Assert.Equal(size * size, changes);
        Assert.True(buffer.TryTakeDirtyRows(out var first, out var last));
        Assert.Equal((0, size - 1), (first, last));
        Assert.False(buffer.TryTakeDirtyRows(out _, out _));
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                Assert.Equal(Color.FromRgb((byte)x, (byte)y, 7), buffer.GetPixel(x, y));
            }
        }
    }

    [Fact]
    public async Task ConcurrentResizeAndCopy_NeverOverrunsTheDestination()
    {
        const int maxSize = 64;
        const int guard = 256;
        var capacity = maxSize * maxSize * 4;
        var memory = Marshal.AllocHGlobal(capacity + guard);
        try
        {
            for (var i = 0; i < guard; i++)
            {
                Marshal.WriteByte(memory, capacity + i, 0xEE);
            }

            var buffer = new ColorFrameBuffer(maxSize, maxSize);
            var stop = false;
            var resizer = Task.Run(
                () =>
                {
                    for (var i = 0; i < 2000; i++)
                    {
                        buffer.Resize(i % 2 == 0 ? 32 : maxSize, i % 2 == 0 ? 48 : maxSize);
                    }

                    Volatile.Write(ref stop, true);
                },
                TestContext.Current.CancellationToken);
            var painter = Task.Run(
                () =>
                {
                    while (!Volatile.Read(ref stop))
                    {
                        buffer.FillRectangle(0, 0, maxSize, maxSize, Colors.White);
                        buffer.Clear(Colors.Black);
                    }
                },
                TestContext.Current.CancellationToken);
            var copier = Task.Run(
                () =>
                {
                    var copied = 0;
                    while (!Volatile.Read(ref stop))
                    {
                        var width = buffer.Width;
                        var height = buffer.Height;
                        if (buffer.TryCopyRows(0, height - 1, memory, width * 4, width, height))
                        {
                            copied++;
                        }
                    }

                    return copied;
                },
                TestContext.Current.CancellationToken);

            await Task.WhenAll(resizer, painter, copier);

            for (var i = 0; i < guard; i++)
            {
                Assert.Equal(0xEE, Marshal.ReadByte(memory, capacity + i));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }
}