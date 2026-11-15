namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class MonochromeFrameBufferTests
{
    [Fact]
    public void SetPixel_SetsTheBitOfItsPage()
    {
        var buffer = new MonochromeFrameBuffer();

        buffer.SetPixel(3, 10);

        Assert.Equal(0b0000_0100, buffer.GetFrame()[(1 * 128) + 3]);
    }

    [Fact]
    public void LoadFrame_RoundTripsThroughGetFrame()
    {
        var buffer = new MonochromeFrameBuffer();
        var frame = new byte[128 * 8];
        for (var i = 0; i < frame.Length; i++)
        {
            frame[i] = (byte)(i * 7);
        }

        buffer.LoadFrame(frame);

        Assert.Equal(frame, buffer.GetFrame());
    }

    [Fact]
    public void LoadFrame_WrongLength_ThrowsAndKeepsThePicture()
    {
        var buffer = new MonochromeFrameBuffer();
        buffer.SetPixel(0, 0);

        var exception = Assert.Throws<ArgumentException>(() => buffer.LoadFrame(new byte[512]));

        Assert.Contains("1024", exception.Message, StringComparison.Ordinal);
        Assert.True(buffer.GetPixel(0, 0));
    }

    [Fact]
    public void PageCount_RoundsUp()
    {
        var buffer = new MonochromeFrameBuffer(128, 20);

        Assert.Equal(3, buffer.PageCount);
        Assert.Equal(384, buffer.GetFrame().Length);
    }

    [Fact]
    public void DrawText_A_DrawsTheRomGlyph()
    {
        var buffer = new MonochromeFrameBuffer();
        byte[] glyph = [0x0E, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11];

        buffer.DrawText(0, 0, "A");

        for (var row = 0; row < glyph.Length; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var expected = (glyph[row] & (0x10 >> column)) != 0;
                Assert.Equal(expected, buffer.GetPixel(column, row));
            }
        }
    }

    [Fact]
    public void DrawText_AdvancesSixPixels()
    {
        var one = new MonochromeFrameBuffer();
        var two = new MonochromeFrameBuffer();
        one.DrawText(0, 0, "A");
        two.DrawText(0, 0, "AA");

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                Assert.Equal(one.GetPixel(column, row), two.GetPixel(column + 6, row));
            }

            Assert.False(two.GetPixel(5, row));
        }
    }

    [Fact]
    public void DrawText_NewLine_MovesDownEightRowsAndBackToX()
    {
        var one = new MonochromeFrameBuffer();
        var two = new MonochromeFrameBuffer();
        one.DrawText(4, 0, "A");
        two.DrawText(4, 0, "B\r\nA");

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                Assert.Equal(one.GetPixel(4 + column, row), two.GetPixel(4 + column, row + 8));
            }
        }
    }

    [Fact]
    public void Drawing_OutsideTheBuffer_IsClippedNotThrown()
    {
        var buffer = new MonochromeFrameBuffer();

        buffer.SetPixel(-1, -1);
        buffer.SetPixel(128, 64);
        buffer.DrawLine(-50, -50, 300, 200);
        buffer.DrawRectangle(-10, -10, 500, 500);
        buffer.FillRectangle(120, 60, 50, 50);
        buffer.DrawText(-3, -3, "Hello\nWorld");
        buffer.DrawText(126, 62, "X");

        Assert.True(buffer.GetPixel(127, 63));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPixel(128, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPixel(0, -1));
    }

    [Fact]
    public void BeginBatch_RaisesChangedOnce()
    {
        var buffer = new MonochromeFrameBuffer();
        var count = 0;
        buffer.Changed += (_, _) => count++;

        using (buffer.BeginBatch())
        {
            buffer.SetPixel(1, 1);
            buffer.DrawLine(0, 0, 10, 10);
            buffer.IsInverted = true;
            Assert.Equal(0, count);
        }

        Assert.Equal(1, count);
    }

    [Fact]
    public void BeginBatch_WithoutChanges_RaisesNothing()
    {
        var buffer = new MonochromeFrameBuffer();
        var count = 0;
        buffer.Changed += (_, _) => count++;

        using (buffer.BeginBatch())
        {
            buffer.SetPixel(1, 1, isOn: false);
        }

        Assert.Equal(0, count);
    }

    [Fact]
    public void UnchangedWrites_RaiseNoChanged()
    {
        var buffer = new MonochromeFrameBuffer();
        buffer.SetPixel(2, 2);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        buffer.SetPixel(2, 2);
        buffer.Contrast = 0x7F;
        buffer.IsDisplayOn = true;
        buffer.IsInverted = false;
        buffer.IsEntireDisplayOn = false;

        Assert.Equal(0, count);

        buffer.Contrast = 0x10;
        Assert.Equal(1, count);
    }

    [Fact]
    public void Resize_KeepsTheOverlappingPixels()
    {
        var buffer = new MonochromeFrameBuffer();
        buffer.SetPixel(5, 5);
        buffer.SetPixel(100, 60);

        buffer.Resize(64, 32);

        Assert.Equal(64, buffer.Width);
        Assert.Equal(32, buffer.Height);
        Assert.True(buffer.GetPixel(5, 5));
        Assert.Equal(64 * 4, buffer.GetFrame().Length);

        buffer.Resize(128, 64);

        Assert.True(buffer.GetPixel(5, 5));
        Assert.False(buffer.GetPixel(100, 60));
    }

    [Theory]
    [InlineData(0, 64)]
    [InlineData(133, 64)]
    [InlineData(128, 0)]
    [InlineData(128, 65)]
    public void Resize_OutOfRange_Throws(
        int width,
        int height)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new MonochromeFrameBuffer().Resize(width, height));

    [Fact]
    public void DrawLine_DrawsBothEndsAndFillRectangleFills()
    {
        var buffer = new MonochromeFrameBuffer();

        buffer.DrawLine(2, 3, 20, 9);
        buffer.FillRectangle(30, 30, 4, 3);
        buffer.DrawRectangle(50, 10, 5, 4);

        Assert.True(buffer.GetPixel(2, 3));
        Assert.True(buffer.GetPixel(20, 9));
        Assert.True(buffer.GetPixel(33, 32));
        Assert.False(buffer.GetPixel(34, 32));
        Assert.True(buffer.GetPixel(50, 10));
        Assert.True(buffer.GetPixel(54, 13));
        Assert.False(buffer.GetPixel(52, 11));
    }

    [Fact]
    public void WritesFromManyThreads_KeepEveryPixel()
    {
        var buffer = new MonochromeFrameBuffer();

        // The threads write different bits of the same bytes, so a missing lock loses writes.
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, thread =>
        {
            for (var iteration = 0; iteration < 200; iteration++)
            {
                for (var x = 0; x < 128; x++)
                {
                    buffer.SetPixel(x, thread);
                }
            }
        });

        Assert.All(buffer.GetFrame().Take(128), b => Assert.Equal(0xFF, b));
    }

    [Fact]
    public void DrawText_UnmappedCharacter_DrawsTheBlankGlyph()
    {
        var buffer = new MonochromeFrameBuffer();

        buffer.DrawText(0, 0, "€中 ");

        Assert.All(buffer.GetFrame(), b => Assert.Equal(0, b));
    }

    [Fact]
    public void NestedBeginBatch_RaisesChangedOnceAtTheOutermostDispose()
    {
        var buffer = new MonochromeFrameBuffer();
        var count = 0;
        buffer.Changed += (_, _) => count++;

        var outer = buffer.BeginBatch();
        var inner = buffer.BeginBatch();
        buffer.SetPixel(1, 1);
        inner.Dispose();
        Assert.Equal(0, count);

        outer.Dispose();
        Assert.Equal(1, count);
    }

    [Fact]
    public void LoadFrameClearAndFill_WithoutChange_RaiseNoChanged()
    {
        var buffer = new MonochromeFrameBuffer();
        var count = 0;
        buffer.Changed += (_, _) => count++;

        buffer.Clear();
        buffer.LoadFrame(new byte[128 * 8]);
        Assert.Equal(0, count);

        buffer.Fill();
        Assert.Equal(1, count);
        buffer.Fill();
        buffer.LoadFrame(buffer.GetFrame());
        Assert.Equal(1, count);

        buffer.Clear();
        Assert.Equal(2, count);
    }

    [Fact]
    public void Changed_IsRaisedOutsideTheLock()
    {
        var buffer = new MonochromeFrameBuffer();
        var readFinished = false;
        buffer.Changed += (_, _) =>
        {
            var reader = Task.Run(() => buffer.GetPixel(0, 0));
            readFinished = reader.Wait(TimeSpan.FromSeconds(5));
        };

        buffer.SetPixel(0, 0);

        Assert.True(readFinished);
    }

    [Fact]
    public void DrawLine_FarAwayEnds_FinishesQuicklyAndLightsTheRow()
    {
        var buffer = new MonochromeFrameBuffer();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        buffer.DrawLine(int.MinValue / 2, 0, int.MaxValue / 2, 0);

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));
        for (var x = 0; x < 128; x++)
        {
            Assert.True(buffer.GetPixel(x, 0));
        }

        Assert.False(buffer.GetPixel(0, 1));
    }

    [Fact]
    public void ClippedShapes_DrawTheirInBoundsPixels()
    {
        var rectangle = new MonochromeFrameBuffer();
        rectangle.DrawRectangle(-10, -10, 500, 500);
        Assert.All(rectangle.GetFrame(), b => Assert.Equal(0, b));

        var partial = new MonochromeFrameBuffer();
        partial.DrawRectangle(-5, 10, 10, 4);
        Assert.True(partial.GetPixel(0, 10));
        Assert.True(partial.GetPixel(4, 13));
        Assert.True(partial.GetPixel(4, 11));
        Assert.False(partial.GetPixel(2, 11));
        Assert.False(partial.GetPixel(5, 10));

        var line = new MonochromeFrameBuffer();
        line.DrawLine(-10, 5, 10, 5);
        for (var x = 0; x < 128; x++)
        {
            Assert.Equal(x <= 10, line.GetPixel(x, 5));
        }

        // The columns 3 and 4 of the 'A' glyph are the columns 0 and 1 of the buffer.
        var text = new MonochromeFrameBuffer();
        text.DrawText(-3, 0, "A");
        byte[] glyph = [0x0E, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11];
        for (var row = 0; row < glyph.Length; row++)
        {
            Assert.Equal((glyph[row] & 0x02) != 0, text.GetPixel(0, row));
            Assert.Equal((glyph[row] & 0x01) != 0, text.GetPixel(1, row));
            Assert.False(text.GetPixel(2, row));
        }
    }

    [Fact]
    public void DrawText_FarOrigin_IsClippedQuicklyWithoutWrapping()
    {
        var buffer = new MonochromeFrameBuffer();

        buffer.DrawText(int.MaxValue - 3, 0, "AA");
        buffer.DrawText(0, int.MaxValue - 3, "AA");
        buffer.DrawText(int.MinValue, int.MinValue, "AA");

        Assert.DoesNotContain(buffer.GetFrame(), static b => b != 0);
    }
}