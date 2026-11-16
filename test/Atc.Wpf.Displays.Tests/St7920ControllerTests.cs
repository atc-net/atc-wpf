namespace Atc.Wpf.Displays.Tests;

[Collection(nameof(TraceTestGroup))]
public sealed class St7920ControllerTests
{
    private const int Width = 128;
    private const int Height = 64;

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new St7920Controller(null!));

    [Fact]
    public void Constructor_ResizesTheBufferTo128x64_AndPowersOnDark()
    {
        // ref: §ST7920.1: the 128 x 64 module; power-on display control D = 0 (display off). The RAM content at power-on is ⚠️ not stated.
        var buffer = new MonochromeFrameBuffer(96, 16);
        buffer.Fill();

        using var controller = new St7920Controller(buffer);

        Assert.Equal(Width, buffer.Width);
        Assert.Equal(Height, buffer.Height);
        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void TextLine_AddressesPerNote()
    {
        // ref: §ST7920.2 (P 21: the first line is 80H..8FH; two bytes per address, AC + 1 after the second byte, P 22),
        // §ST7920.4 (word w: high byte glyph at column 16 w, low byte glyph at 16 w + 8, 16 rows tall).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);

        controller.WriteData("AB"u8);
        controller.WriteData("CD"u8);

        AssertGlyph(buffer, 0, 0, (byte)'A');
        AssertGlyph(buffer, 8, 0, (byte)'B');
        AssertGlyph(buffer, 16, 0, (byte)'C');
        AssertGlyph(buffer, 24, 0, (byte)'D');
        Assert.DoesNotContain(LitRows(buffer), y => y >= 16);
    }

    [Fact]
    public void TextLine_OtherLineAddresses_EachDrawInTheirOwn16RowBand()
    {
        // ref: §ST7920.4: 0x90, 0x88 and 0x98 are the other three text lines of a 128 x 64 module, 16 rows each. Which band each one
        // is (the line order) is ⚠️ derived and not asserted here; only that each address draws inside one band of its own.
        var bands = new List<int>();
        foreach (var address in new byte[] { 0x90, 0x88, 0x98 })
        {
            var buffer = new MonochromeFrameBuffer();
            using var controller = new St7920Controller(buffer);
            controller.WriteCommands([0x01, 0x0C, address]);
            controller.WriteData("AA"u8);

            var bandsHit = LitRows(buffer).Select(y => y / 16).Distinct().ToList();
            Assert.Single(bandsHit);
            bands.Add(bandsHit[0]);
        }

        Assert.DoesNotContain(0, bands);
        Assert.Equal(3, bands.Distinct().Count());
    }

    [Fact]
    public void Glyph_A_DrawsTheRomBitmap()
    {
        // ref: §ST7920.10: the glyph shapes are ⚠️ approximate; only that the table glyph is drawn, that 0x20 is empty and that two
        // printable codes differ are asserted.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);

        controller.WriteData([0x41, 0x20]);

        AssertGlyph(buffer, 0, 0, 0x41);
        Assert.Equal(CountBits(St7920RomTables.GetGlyph(0x41)), CountLit(buffer));
        Assert.Equal(0, CountBits(St7920RomTables.GetGlyph(0x20)));
        Assert.False(St7920RomTables.GetGlyph(0x41).SequenceEqual(St7920RomTables.GetGlyph(0x42)));
        Assert.True(CountBits(St7920RomTables.GetGlyph(0x41)) > 0);
    }

    [Fact]
    public void FullWidthCodes_DrawBlank()
    {
        // ref: §ST7920.4: a code of 0xA1 or more is a 16 x 16 CGROM character combined with the next byte; the mirror draws it blank.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);

        controller.WriteData([0xB0, 0xA1, 0xA1, 0xA1, (byte)'A', (byte)'A']);

        Assert.DoesNotContain(LitPixels(buffer), p => p.X < 32);
        AssertGlyph(buffer, 32, 0, (byte)'A');
        AssertGlyph(buffer, 40, 0, (byte)'A');
    }

    [Fact]
    public void ExtendedSet_GdramWrite_LightsPixels()
    {
        // ref: §ST7920.3 (0x34 sets RE = 1, then 0x36 sets G = 1; the GDRAM address is two instructions: vertical, then horizontal),
        // §ST7920.5 (first byte = left 8 pixels, MSB leftmost; the horizontal address increments per word and wraps from 15 to 0 in the row).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x34, 0x36]);

        // Row 3, word 2: 0x81 0x01 lights x = 32, 39 and 47; the next word (3) is filled.
        controller.WriteCommands([0x80 | 3, 0x80 | 2]);
        controller.WriteData([0x81, 0x01, 0xFF, 0xFF]);

        Assert.True(buffer.GetPixel(32, 3));
        Assert.False(buffer.GetPixel(33, 3));
        Assert.True(buffer.GetPixel(39, 3));
        Assert.True(buffer.GetPixel(47, 3));
        for (var x = 48; x < 64; x++)
        {
            Assert.True(buffer.GetPixel(x, 3));
        }

        // Row 10, word 15 then the wrap to word 0 of the same row.
        controller.WriteCommands([0x80 | 10, 0x80 | 15]);
        controller.WriteData([0x00, 0x00, 0x80, 0x00]);

        Assert.True(buffer.GetPixel(0, 10));

        // ⚠️ derived (§ST7920.5): the 128 x 64 fold, words 8..15 of GDRAM row r show as glass row r + 32. Both libraries in scope
        // (ESPHome goto_xy_, U8g2 u8x8_d_st7920_common) send their bottom half this way (R14).
        controller.WriteCommands([0x80 | 5, 0x80 | 8]);
        controller.WriteData([0x00, 0x01]);

        Assert.True(buffer.GetPixel(15, 37));
        Assert.False(buffer.GetPixel(14, 37));
    }

    [Fact]
    public void TextAndGraphics_CombinePerNote()
    {
        // ref: §ST7920.7: the overlap rule is ⚠️ unverified and not asserted; only pixels where one layer is blank are checked,
        // and G = 0 hides the graphics while the text stays.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);
        controller.WriteData([(byte)'A', 0x20]);

        controller.WriteCommands([0x34, 0x36, 0x80 | 20, 0x80 | 0]);
        controller.WriteData([0xFF, 0xFF]);

        AssertGlyph(buffer, 0, 0, (byte)'A');
        for (var x = 0; x < 16; x++)
        {
            Assert.True(buffer.GetPixel(x, 20));
        }

        // 0x34 while RE = 1 is the extended function set with G = 0.
        controller.WriteCommand(0x34);

        AssertGlyph(buffer, 0, 0, (byte)'A');
        Assert.False(buffer.GetPixel(0, 20));
        Assert.Equal(CountBits(St7920RomTables.GetGlyph((byte)'A')), CountLit(buffer));
    }

    [Fact]
    public void Clear_Home_DisplayOff()
    {
        // ref: §ST7920.1 (power-on D = 0), §ST7920.2 (0x0C display on, 0x01 fills DDRAM with 0x20 and sets AC = 0,
        // 0x02 sets AC = 0 and keeps DDRAM, 0x08 display off). Graphics stay off (G = 0), so GDRAM plays no part.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        Assert.False(buffer.IsDisplayOn);

        controller.WriteCommands([0x01, 0x0C, 0x80]);
        controller.WriteData("AB"u8);
        Assert.True(buffer.IsDisplayOn);
        Assert.True(CountLit(buffer) > 0);

        controller.WriteCommand(0x01);
        Assert.Equal(0, CountLit(buffer));

        controller.WriteData("CD"u8);
        AssertGlyph(buffer, 0, 0, (byte)'C');

        controller.WriteData("EF"u8);
        controller.WriteCommand(0x02);
        controller.WriteData("GH"u8);
        AssertGlyph(buffer, 0, 0, (byte)'G');
        AssertGlyph(buffer, 8, 0, (byte)'H');
        AssertGlyph(buffer, 16, 0, (byte)'E');

        controller.WriteCommand(0x08);
        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void Reset_ReturnsToPowerOn()
    {
        // ref: §ST7920.1, §ST7920.9 item 4.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);
        controller.WriteData("AB"u8);

        controller.Reset();

        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void Call_RaisesChangedAtMostOnce()
    {
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommands([0x01, 0x0C, 0x80]);
        Assert.Equal(1, count);

        controller.WriteData("ABCDEFGH"u8);
        Assert.Equal(2, count);

        controller.WriteCommand(0x0C);
        Assert.Equal(2, count);
    }

    [Fact]
    public void Dispose_MakesTheControllerInert()
    {
        var buffer = new MonochromeFrameBuffer();
        var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);
        controller.WriteData("AB"u8);
        var before = buffer.GetFrame();

        controller.Dispose();
        controller.WriteCommand(0x01);
        controller.WriteData("CD"u8);
        controller.Reset();

        Assert.Equal(before, buffer.GetFrame());
        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void BufferResized_IsDrawnAgain()
    {
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        controller.WriteCommands([0x01, 0x0C, 0x80]);
        controller.WriteData("AB"u8);

        buffer.Resize(64, 32);

        AssertGlyph(buffer, 0, 0, (byte)'A');
        AssertGlyph(buffer, 8, 0, (byte)'B');
    }

    internal static void AssertGlyph(
        MonochromeFrameBuffer buffer,
        int left,
        int top,
        byte code)
    {
        var glyph = St7920RomTables.GetGlyph(code);
        for (var row = 0; row < 16; row++)
        {
            var bits = glyph.IsEmpty ? 0 : glyph[row];
            for (var column = 0; column < 8; column++)
            {
                var expected = (bits & (0x80 >> column)) != 0;
                Assert.True(
                    expected == buffer.GetPixel(left + column, top + row),
                    $"code 0x{code:X2}: pixel ({left + column}, {top + row}) should be {(expected ? "on" : "off")}");
            }
        }
    }

    internal static int CountLit(MonochromeFrameBuffer buffer)
        => LitPixels(buffer).Count();

    internal static IEnumerable<(int X, int Y)> LitPixels(
        MonochromeFrameBuffer buffer)
    {
        for (var y = 0; y < buffer.Height; y++)
        {
            for (var x = 0; x < buffer.Width; x++)
            {
                if (buffer.GetPixel(x, y))
                {
                    yield return (x, y);
                }
            }
        }
    }

    private static IEnumerable<int> LitRows(MonochromeFrameBuffer buffer)
        => LitPixels(buffer).Select(p => p.Y).Distinct();

    private static int CountBits(ReadOnlySpan<byte> glyph)
    {
        var count = 0;
        foreach (var value in glyph)
        {
            count += System.Numerics.BitOperations.PopCount(value);
        }

        return count;
    }
}