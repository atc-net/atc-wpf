namespace Atc.Wpf.Components.Tests.Notifications;

public sealed class TrayIconMessageDecoderTests
{
    [Theory]
    [InlineData(0x0400, "Click")]
    [InlineData(0x0401, "Click")]
    [InlineData(0x0203, "DoubleClick")]
    [InlineData(0x007B, "ContextMenu")]
    [InlineData(0x0200, "None")]
    [InlineData(0x0202, "None")]
    [InlineData(0x0205, "None")]
    public void Decode_MapsTheNotificationToAnAction(
        int notification,
        string expected)
        => Assert.Equal(expected, TrayIconMessageDecoder.Decode(notification).ToString());

    [Fact]
    public void Decode_IgnoresTheIconIdInTheHighWord()
        => Assert.Equal(TrayIconAction.Click, TrayIconMessageDecoder.Decode((7 << 16) | 0x0400));

    [Fact]
    public void TruncateToolTip_KeepsAShortText()
        => Assert.Equal("Status: OK", TrayIconMessageDecoder.TruncateToolTip("Status: OK"));

    [Fact]
    public void TruncateToolTip_CutsALongTextToTheShellLimit()
    {
        var result = TrayIconMessageDecoder.TruncateToolTip(new string('x', 200));

        Assert.Equal(127, result.Length);
    }
}