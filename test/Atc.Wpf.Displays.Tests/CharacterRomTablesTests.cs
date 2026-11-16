namespace Atc.Wpf.Displays.Tests;

public sealed class CharacterRomTablesTests
{
    [Fact]
    public void A00_Glyph_A_IsTheDatasheetBitmap()
        => Assert.Equal(
            new byte[] { 0x0E, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x00 },
            CharacterRomTables.GetGlyph(CharacterRom.A00, 0x41).ToArray());

    [Fact]
    public void A02_Glyph_A_IsTheDatasheetBitmap()
        => Assert.Equal(
            new byte[] { 0x00, 0x04, 0x0A, 0x11, 0x11, 0x1F, 0x11, 0x11 },
            CharacterRomTables.GetGlyph(CharacterRom.A02, 0x41).ToArray());

    [Fact]
    public void A02_AccentedLetter_SharesTheBaselineOfItsPlainLetter()
    {
        var e = CharacterRomTables.GetGlyph(
            CharacterRom.A02,
            CharacterRomTables.Map(CharacterRom.A02, 'e'));
        var eAcute = CharacterRomTables.GetGlyph(
            CharacterRom.A02,
            CharacterRomTables.Map(CharacterRom.A02, 'é'));

        Assert.Equal(LowestLitRow(e), LowestLitRow(eAcute));
    }

    [Theory]
    [InlineData('A', 0x41)]
    [InlineData('z', 0x7A)]
    [InlineData('0', 0x30)]
    [InlineData(' ', 0x20)]
    public void Map_Ascii_IsItsOwnCode(
        char c,
        byte code)
    {
        Assert.Equal(code, CharacterRomTables.Map(CharacterRom.A00, c));
        Assert.Equal(code, CharacterRomTables.Map(CharacterRom.A02, c));
    }

    [Fact]
    public void A00_HasTheJapaneseRomSpecials()
    {
        Assert.Equal(0x5C, CharacterRomTables.Map(CharacterRom.A00, '¥'));
        Assert.Equal(0x7E, CharacterRomTables.Map(CharacterRom.A00, '→'));
        Assert.Equal(0x7F, CharacterRomTables.Map(CharacterRom.A00, '←'));
        Assert.Equal(0xDF, CharacterRomTables.Map(CharacterRom.A00, '°'));
    }

    [Theory]
    [InlineData('æ')]
    [InlineData('ø')]
    [InlineData('å')]
    [InlineData('Æ')]
    [InlineData('Ø')]
    [InlineData('Å')]
    [InlineData('é')]
    [InlineData('ü')]
    public void A02_HasTheEuropeanLetters(char c)
        => Assert.NotEqual(
            CharacterRomTables.UnknownCode,
            CharacterRomTables.Map(CharacterRom.A02, c));

    [Theory]
    [InlineData(CharacterRom.A00, '\uD83D')]
    [InlineData(CharacterRom.A02, '中')]
    public void Map_CharacterOutsideTheRom_IsTheUnknownCode(
        CharacterRom rom,
        char c)
        => Assert.Equal(
            CharacterRomTables.UnknownCode,
            CharacterRomTables.Map(rom, c));

    [Theory]
    [InlineData('\u0000', 0x00)]
    [InlineData('\u0007', 0x07)]
    public void Map_CustomGlyphCodes_PassThrough(
        char c,
        byte code)
        => Assert.Equal(code, CharacterRomTables.Map(CharacterRom.A00, c));

    private static int LowestLitRow(ReadOnlySpan<byte> glyph)
    {
        for (var row = glyph.Length - 1; row >= 0; row--)
        {
            if (glyph[row] != 0)
            {
                return row;
            }
        }

        return -1;
    }
}