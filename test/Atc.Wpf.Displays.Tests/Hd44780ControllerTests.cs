namespace Atc.Wpf.Displays.Tests;

public sealed class Hd44780ControllerTests
{
    [Fact]
    public void ClearDisplay_FillsDdramWithSpacesAndHomesTheCursor()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);
        controller.Command(0x80 | 0x05);
        controller.Data((byte)'X');
        buffer.ShiftOffset = 3;

        controller.Command(0x01);

        Assert.All(buffer.GetDdram(0), code => Assert.Equal((byte)' ', code));
        Assert.Equal(0, buffer.CursorAddress);
        Assert.Equal(0, buffer.ShiftOffset);
        Assert.True(controller.IsIncrement);
    }

    [Fact]
    public void ReturnHome_ResetsCursorAndShift()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);
        controller.Command(0x80 | 0x45);
        buffer.ShiftOffset = 5;

        controller.Command(0x02);

        Assert.Equal(0, buffer.CursorAddress);
        Assert.Equal(0, buffer.ShiftOffset);
    }

    [Fact]
    public void SetDdramAddress_ThenData_WritesThere()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x80 | 0x54);
        controller.Data((byte)'A');

        Assert.Equal((byte)'A', buffer.GetCode(3, 0));
    }

    [Fact]
    public void EntryModeDecrement_MovesLeft()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x04);
        controller.Command(0x80 | 0x05);
        controller.Data((byte)'C');
        controller.Data((byte)'B');
        controller.Data((byte)'A');

        Assert.False(controller.IsIncrement);
        Assert.Equal((byte)'C', buffer.GetCode(0, 5));
        Assert.Equal((byte)'B', buffer.GetCode(0, 4));
        Assert.Equal((byte)'A', buffer.GetCode(0, 3));
        Assert.Equal(2, buffer.CursorAddress);
    }

    [Fact]
    public void EntryModeShift_ShiftsTheDisplayOnEachWrite()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x07);
        controller.Data((byte)'A');
        Assert.Equal(1, buffer.ShiftOffset);
        controller.Data((byte)'B');

        Assert.True(controller.IsDisplayShiftOnEntry);
        Assert.True(controller.IsIncrement);
        Assert.Equal(2, buffer.ShiftOffset);

        controller.Command(0x05);
        controller.Data((byte)'C');
        Assert.False(controller.IsIncrement);
        Assert.Equal(1, buffer.ShiftOffset);
    }

    [Fact]
    public void DisplayControl_SetsDisplayCursorAndBlink()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x0F);
        Assert.True(buffer.IsDisplayOn);
        Assert.Equal(CharacterDisplayCursor.UnderlineAndBlink, buffer.CursorMode);

        controller.Command(0x0E);
        Assert.Equal(CharacterDisplayCursor.Underline, buffer.CursorMode);

        controller.Command(0x0D);
        Assert.Equal(CharacterDisplayCursor.Blink, buffer.CursorMode);

        controller.Command(0x0C);
        Assert.Equal(CharacterDisplayCursor.None, buffer.CursorMode);

        controller.Command(0x08);
        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void CursorOrDisplayShift_MovesCursorOrShiftsDisplay()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x14);
        Assert.Equal(1, buffer.CursorAddress);
        Assert.Equal(0, buffer.ShiftOffset);

        controller.Command(0x10);
        Assert.Equal(0, buffer.CursorAddress);

        controller.Command(0x18);
        Assert.Equal(1, buffer.ShiftOffset);
        Assert.Equal(0, buffer.CursorAddress);

        controller.Command(0x1C);
        Assert.Equal(0, buffer.ShiftOffset);
    }

    [Fact]
    public void SetCgramAddress_ThenData_DefinesACustomGlyph()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);
        byte[] rows = [0x0E, 0x11, 0x11, 0x1F, 0x1B, 0x1B, 0x1F, 0x00];

        controller.Command(0x40 | (2 << 3));
        foreach (var row in rows)
        {
            controller.Data(row);
        }

        Assert.Equal(rows, buffer.GetCustomGlyph(2));
        Assert.Equal(new byte[8], buffer.GetCustomGlyph(3));
    }

    [Fact]
    public void SetCgramAddress_WrapsWithinTheCgram()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x40 | 0x3F);
        controller.Data(0x01);
        controller.Data(0x02);

        Assert.Equal(0x01, buffer.GetCustomGlyph(7)[7]);
        Assert.Equal(0x02, buffer.GetCustomGlyph(0)[0]);
    }

    [Fact]
    public void DataAfterCgram_ThenSetDdram_WritesTextAgain()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x40);
        controller.Data(0x1F);
        controller.Command(0x80);
        controller.Data((byte)'H');
        controller.Data((byte)'i');

        Assert.Equal((byte)'H', buffer.GetCode(0, 0));
        Assert.Equal((byte)'i', buffer.GetCode(0, 1));
        Assert.Equal(0x1F, buffer.GetCustomGlyph(0)[0]);
        Assert.Equal(0, buffer.GetCustomGlyph(0)[1]);
    }

    [Fact]
    public void FunctionSet_OneLine_SwitchesTheModeAndKeepsTheBufferRows()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x20);

        Assert.False(controller.IsTwoLineMode);
        Assert.Equal(4, buffer.Rows);
        Assert.Equal(new string(' ', 20), buffer.GetVisibleText(CharacterRom.A00).Split('\n')[1]);

        controller.Command(0x38);
        Assert.True(controller.IsTwoLineMode);
    }

    [Fact]
    public void FortyByFour_SecondControllerWritesRowsThreeAndFour()
    {
        var buffer = new CharacterDisplayBuffer(40, 4);
        var controller = new Hd44780Controller(buffer, 1);

        controller.Data((byte)'Z');

        Assert.Equal((byte)'Z', buffer.GetCode(2, 0));
        Assert.Equal((byte)' ', buffer.GetCode(0, 0));
        Assert.Equal(1, buffer.CursorController);
    }

    [Fact]
    public void PrintSequenceFromLiquidCrystal_ShowsTheText()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Write([0x38, 0x0C, 0x06, 0x01], isData: false);
        controller.Write("Temp 21.5"u8, isData: true);

        var row0 = buffer.GetVisibleText(CharacterRom.A00).Split('\n')[0];
        Assert.Equal("Temp 21.5           ", row0);
        Assert.Equal(9, buffer.CursorAddress);
    }

    [Fact]
    public void Write_RaisesChangedOncePerSpan()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.Write("Hello"u8, isData: true);

        Assert.Equal(1, count);
    }

    [Fact]
    public void TwoLineMode_StepsFrom0x27To0x40()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x80 | 0x27);
        controller.Data((byte)'A');

        Assert.Equal(0x40, buffer.CursorAddress);
    }

    [Fact]
    public void TwoLineMode_StepsFrom0x67ToZero()
    {
        var buffer = new CharacterDisplayBuffer(40, 4);
        var controller = new Hd44780Controller(buffer);

        controller.Command(0x80 | 0x67);
        controller.Data((byte)'A');

        Assert.Equal(0, buffer.CursorAddress);
    }

    [Fact]
    public void TwoLineMode_Decrement_StepsFrom0x40To0x27AndZeroTo0x67()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);
        var controller = new Hd44780Controller(buffer);
        controller.Command(0x04);

        controller.Command(0x80 | 0x40);
        controller.Data((byte)'A');
        Assert.Equal(0x27, buffer.CursorAddress);

        controller.Command(0x80);
        controller.Data((byte)'B');
        Assert.Equal(0x67, buffer.CursorAddress);
    }

    [Fact]
    public void OneLineMode_RunsLinearlyFromZeroTo0x4F()
    {
        var buffer = new CharacterDisplayBuffer(40, 1);
        var controller = new Hd44780Controller(buffer);
        controller.Command(0x20);

        controller.Command(0x80 | 0x27);
        controller.Data((byte)'A');
        controller.Data((byte)'B');
        Assert.Equal(0x29, GetLinearAddress(buffer));

        controller.Command(0x80 | 0x4F);
        controller.Data((byte)'C');
        Assert.Equal(0, buffer.CursorAddress);

        controller.Command(0x04);
        controller.Command(0x80);
        controller.Data((byte)'D');
        Assert.Equal(0x4F, GetLinearAddress(buffer));
    }

    [Fact]
    public void ControllersKeepTheirOwnAddressCounters()
    {
        var buffer = new CharacterDisplayBuffer(40, 4);
        var first = new Hd44780Controller(buffer, 0);
        var second = new Hd44780Controller(buffer, 1);

        first.Command(0x80 | 0x03);
        second.Command(0x80 | 0x40);
        first.Data((byte)'A');
        second.Data((byte)'B');

        Assert.Equal((byte)'A', buffer.GetCode(0, 3));
        Assert.Equal((byte)'B', buffer.GetCode(3, 0));
    }

    [Fact]
    public void Constructor_WithInvalidArguments_Throws()
    {
        var buffer = new CharacterDisplayBuffer(20, 4);

        Assert.Throws<ArgumentNullException>(() => new Hd44780Controller(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Hd44780Controller(buffer, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Hd44780Controller(buffer, 2));
    }

    [Fact]
    public void AnyByteValue_NeverThrows()
    {
        var buffer = new CharacterDisplayBuffer(40, 4);
        var controller = new Hd44780Controller(buffer);

        for (var value = 0; value <= byte.MaxValue; value++)
        {
            controller.Command((byte)value);
            controller.Data((byte)value);
        }

        Assert.InRange(buffer.CursorAddress, 0, 0x7F);
    }

    private static int GetLinearAddress(CharacterDisplayBuffer buffer)
        => buffer.CursorAddress < 0x40
            ? buffer.CursorAddress
            : buffer.CursorAddress - 0x40 + 0x28;
}