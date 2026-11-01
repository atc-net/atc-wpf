namespace Atc.Wpf.Network.Tests.Vnc;

public sealed class KeySymMapperTests
{
    [Theory]
    [InlineData('a', 0x0061u)]
    [InlineData('@', 0x0040u)]
    [InlineData(' ', 0x0020u)]
    [InlineData('æ', 0x00E6u)]
    [InlineData('ø', 0x00F8u)]
    [InlineData('Å', 0x00C5u)]
    [InlineData('ß', 0x00DFu)]
    [InlineData('€', 0x010020ACu)]
    [InlineData('ł', 0x01000142u)]
    public void TryGetKeySym_Character_UsesLatin1OrUnicodeKeySym(
        char character,
        uint expected)
    {
        Assert.True(KeySymMapper.TryGetKeySym(character, out var keySym));
        Assert.Equal(expected, keySym);
    }

    [Theory]
    [InlineData('\r')]
    [InlineData('\t')]
    [InlineData('\b')]
    [InlineData('\u007F')]
    [InlineData('\u0085')]
    [InlineData('\uD83D')]
    public void TryGetKeySym_ControlOrLoneSurrogate_ReturnsFalse(char character)
    {
        Assert.False(KeySymMapper.TryGetKeySym(character, out var keySym));
        Assert.Equal(0u, keySym);
    }

    [Fact]
    public void TryGetKeySym_RuneOutsideTheBmp_UsesUnicodeKeySym()
    {
        Assert.True(KeySymMapper.TryGetKeySym(new System.Text.Rune(0x1F600), out var keySym));
        Assert.Equal(0x0101F600u, keySym);
    }

    [Fact]
    public void TryGetKeySym_Key_StillMapsSpecialKeys()
    {
        Assert.True(KeySymMapper.TryGetKeySym(Key.Return, out var keySym));
        Assert.Equal(0xFF0Du, keySym);
    }
}