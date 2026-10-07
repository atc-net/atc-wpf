namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalMessageRoutingTests
{
    [Theory]
    [InlineData(null, null, true)]
    [InlineData(null, "first", true)]
    [InlineData("first", "first", true)]
    [InlineData("first", "second", false)]
    [InlineData("first", null, false)]
    [InlineData("first", "FIRST", false)]
    public void Accepts_RoutesAddressedMessagesOnlyToTheMatchingViewer(
        string? messageTerminalId,
        string? viewerTerminalId,
        bool expected)
        => Assert.Equal(expected, TerminalMessageRouting.Accepts(messageTerminalId, viewerTerminalId));
}