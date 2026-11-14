namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class CharacterDisplayBufferTests
{
    [Theory]
    [InlineData(16, 2, 1, 0, 0x40, 0)]
    [InlineData(20, 4, 2, 0, 0x14, 0)]
    [InlineData(20, 4, 3, 5, 0x59, 0)]
    [InlineData(16, 4, 2, 0, 0x10, 0)]
    [InlineData(40, 4, 2, 0, 0x00, 1)]
    [InlineData(40, 4, 3, 39, 0x67, 1)]
    [InlineData(24, 3, 2, 0, 0x00, 1)]
    [InlineData(40, 3, 2, 0, 0x00, 1)]
    public void AddressOf_FollowsTheModuleLayout(
        int columns,
        int rows,
        int row,
        int column,
        int address,
        int controller)
    {
        Assert.Equal(
            address,
            CharacterDisplayBuffer.AddressOf(row, column, columns, rows, out var c));
        Assert.Equal(controller, c);
    }

    [Theory]
    [InlineData(20, 4, 1)]
    [InlineData(40, 4, 2)]
    [InlineData(24, 3, 2)]
    [InlineData(40, 3, 2)]
    [InlineData(20, 3, 1)]
    [InlineData(40, 2, 1)]
    [InlineData(16, 2, 1)]
    public void ControllerCount_IsTwoOnlyForWideFourLineModules(
        int columns,
        int rows,
        int expected)
        => Assert.Equal(expected, new CharacterDisplayBuffer(columns, rows).ControllerCount);

    [Theory]
    [InlineData(0, 2)]
    [InlineData(41, 2)]
    [InlineData(16, 0)]
    [InlineData(16, 5)]
    public void Constructor_WithInvalidSize_Throws(
        int columns,
        int rows)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterDisplayBuffer(columns, rows));

    [Fact]
    public void WriteVisible_PastTheLastColumn_IsDropped()
    {
        var buffer = new CharacterDisplayBuffer(8, 1);

        buffer.WriteVisible(0, 0, "123456789", CharacterRom.A00);

        Assert.Equal("12345678", buffer.GetVisibleText(CharacterRom.A00));
    }

    [Fact]
    public void GetVisibleText_JoinsRowsWithNewLine()
    {
        var buffer = new CharacterDisplayBuffer(4, 2);

        buffer.WriteVisible(0, 0, "ab", CharacterRom.A00);
        buffer.WriteVisible(1, 1, "cd", CharacterRom.A00);

        Assert.Equal("ab  \n cd ", buffer.GetVisibleText(CharacterRom.A00));
    }

    [Fact]
    public void GetVisibleText_ReturnsCustomCodesAsControlCharacters()
    {
        var buffer = new CharacterDisplayBuffer(4, 1);

        buffer.WriteVisible(0, 0, "\u0003", CharacterRom.A00);

        Assert.Equal('\u0003', buffer.GetVisibleText(CharacterRom.A00)[0]);
        Assert.Equal(3, buffer.GetCode(0, 0));
    }

    [Fact]
    public void GetVisibleText_ReturnsTheYenSignOfA00()
    {
        var buffer = new CharacterDisplayBuffer(4, 1);

        buffer.WriteVisible(0, 0, "¥", CharacterRom.A00);

        Assert.Equal(0x5C, buffer.GetCode(0, 0));
        Assert.Equal('¥', buffer.GetVisibleText(CharacterRom.A00)[0]);
    }

    [Fact]
    public void ShiftOffset_BringsHiddenDdramIntoView()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        buffer.SetDdram(0, 16, (byte)'X');

        buffer.ShiftOffset = 1;

        Assert.Equal((byte)'X', buffer.GetCode(0, 15));
    }

    [Fact]
    public void ShiftOffset_Negative_WrapsWithinTheLine()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        buffer.SetDdram(0, 39, (byte)'Z');

        buffer.ShiftOffset = -1;

        Assert.Equal((byte)'Z', buffer.GetCode(0, 0));
    }

    [Fact]
    public void CustomGlyph_RoundTrips()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        byte[] bits = [0x01, 0x02, 0x04, 0x08, 0x10, 0x0A, 0x15, 0x1F];

        for (var row = 0; row < bits.Length; row++)
        {
            buffer.SetCgram(3, row, bits[row]);
        }

        Assert.Equal(bits, buffer.GetCustomGlyph(3));
    }

    [Fact]
    public void SetCgram_MasksToFiveBits()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        buffer.SetCgram(0, 2, 0xFF);

        Assert.Equal(0x1F, buffer.GetCustomGlyph(0)[2]);
    }

    [Fact]
    public void GetCustomGlyph_ReturnsACopy()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        var glyph = buffer.GetCustomGlyph(1);
        glyph[0] = 0x1F;

        Assert.Equal(0, buffer.GetCustomGlyph(1)[0]);
    }

    [Fact]
    public void Changed_IsRaisedOncePerBatch()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        using (buffer.BeginBatch())
        {
            buffer.WriteVisible(0, 0, "a", CharacterRom.A00);
            buffer.WriteVisible(0, 1, "b", CharacterRom.A00);
            buffer.SetDdram(0, 5, (byte)'c');
        }

        Assert.Equal(1, count);
    }

    [Fact]
    public void Changed_IsRaisedOncePerWriteOutsideABatch()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        buffer.WriteVisible(0, 0, "a", CharacterRom.A00);
        buffer.WriteVisible(0, 1, "b", CharacterRom.A00);

        Assert.Equal(2, count);
    }

    [Fact]
    public void Resize_KeepsDdramContent()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        buffer.WriteVisible(0, 0, "HELLO", CharacterRom.A00);

        buffer.Resize(20, 2);

        Assert.StartsWith("HELLO", buffer.GetVisibleText(CharacterRom.A00), StringComparison.Ordinal);
        Assert.Equal(20, buffer.Columns);
    }

    [Fact]
    public void Clear_BlanksTheDisplay()
    {
        var buffer = new CharacterDisplayBuffer(4, 2);
        buffer.WriteVisible(0, 0, "abcd", CharacterRom.A00);
        buffer.WriteVisible(1, 0, "efgh", CharacterRom.A00);

        buffer.Clear();

        Assert.Equal("    \n    ", buffer.GetVisibleText(CharacterRom.A00));
    }

    [Fact]
    public void GetDdram_ReturnsACopyOf80Bytes()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        var ddram = buffer.GetDdram(0);
        ddram[0] = 0x41;

        Assert.Equal(80, ddram.Length);
        Assert.Equal(0x20, buffer.GetDdram(0)[0]);
    }

    [Fact]
    public void SetDdram_WrapsAtTheEndOfTheLine()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        buffer.SetDdram(0, 0x28, (byte)'W');

        Assert.Equal((byte)'W', buffer.GetCode(0, 0));
    }

    [Fact]
    public void GetCursorCell_FindsTheVisibleCell()
    {
        var buffer = new CharacterDisplayBuffer(16, 2)
        {
            CursorAddress = 0x43,
        };

        Assert.Equal((1, 3), buffer.GetCursorCell());
    }

    [Fact]
    public void GetCursorCell_OffScreen_IsNull()
    {
        var buffer = new CharacterDisplayBuffer(16, 2)
        {
            CursorAddress = 0x14,
        };

        Assert.Null(buffer.GetCursorCell());
    }

    [Fact]
    public void GetCursorCell_FollowsTheShift()
    {
        var buffer = new CharacterDisplayBuffer(16, 2)
        {
            CursorAddress = 0x05,
            ShiftOffset = 2,
        };

        Assert.Equal((0, 3), buffer.GetCursorCell());
    }

    [Fact]
    public void GetCursorCell_OnTheSecondController_Matches()
    {
        var buffer = new CharacterDisplayBuffer(40, 4)
        {
            CursorController = 1,
            CursorAddress = 0x41,
        };

        Assert.Equal((3, 1), buffer.GetCursorCell());
    }

    [Fact]
    public void Buffer_WritesFromManyThreads_KeepEveryCharacter()
    {
        var buffer = new CharacterDisplayBuffer(8, 1);

        Parallel.For(
            0,
            8,
            column =>
            {
                var text = ((char)('A' + column)).ToString();
                for (var i = 0; i < 1000; i++)
                {
                    buffer.WriteVisible(0, column, text, CharacterRom.A00);
                }
            });

        Assert.Equal("ABCDEFGH", buffer.GetVisibleText(CharacterRom.A00));
    }

    [Theory]
    [InlineData(CharacterRom.A00, 0xDF)]
    [InlineData(CharacterRom.A02, 0xB0)]
    public void GetVisibleText_ReadsTheDegreeSignBack(
        CharacterRom rom,
        byte code)
    {
        var buffer = new CharacterDisplayBuffer(8, 1);

        buffer.WriteVisible(0, 0, "21.5°C", rom);

        Assert.Equal(code, buffer.GetCode(0, 4));
        Assert.StartsWith("21.5°C", buffer.GetVisibleText(rom), StringComparison.Ordinal);
    }

    [Fact]
    public void Clear_OnTheSecondController_HomesTheCursorOnControllerZero()
    {
        var buffer = new CharacterDisplayBuffer(40, 4)
        {
            CursorController = 1,
            CursorAddress = 0x45,
        };

        buffer.Clear();

        Assert.Equal(0, buffer.CursorController);
        Assert.Equal(0, buffer.CursorAddress);
        Assert.Equal((0, 0), buffer.GetCursorCell());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0x80)]
    public void SetDdram_OutsideTheAddressCounterRange_Throws(int address)
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.SetDdram(0, address, (byte)'X'));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0x80)]
    public void CursorAddress_OutsideTheAddressCounterRange_Throws(int address)
    {
        var buffer = new CharacterDisplayBuffer(16, 2);

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.CursorAddress = address);
    }

    [Fact]
    public void SetDdram_TheLastAddressOfTheCounter_WrapsWithinTheSecondLine()
    {
        var buffer = new CharacterDisplayBuffer(40, 2);

        buffer.SetDdram(0, 0x7F, (byte)'X');

        Assert.Equal((byte)'X', buffer.GetCode(1, 0x7F - 0x40 - 40));
    }

    [Theory]
    [InlineData(-1, 0, 16, 2)]
    [InlineData(2, 0, 16, 2)]
    [InlineData(0, -1, 16, 2)]
    [InlineData(0, 16, 16, 2)]
    [InlineData(0, 0, 0, 2)]
    [InlineData(0, 0, 41, 2)]
    [InlineData(0, 0, 16, 0)]
    [InlineData(0, 0, 16, 5)]
    public void AddressOf_OutsideTheModule_Throws(
        int row,
        int column,
        int columns,
        int rows)
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => CharacterDisplayBuffer.AddressOf(row, column, columns, rows, out _));
}