namespace Atc.Wpf.Components.Tests.EventArgs;

public sealed class TerminalReceivedDataEventArgsTests
{
    [Fact]
    public void Lines_exposes_the_input_array_in_order()
    {
        var lines = new[] { "first", "second", "third" };

        var args = new TerminalReceivedDataEventArgs(lines);

        Assert.Equal(3, args.Lines.Count);
        Assert.Equal("first", args.Lines[0]);
        Assert.Equal("second", args.Lines[1]);
        Assert.Equal("third", args.Lines[2]);
    }

    [Fact]
    public void Empty_input_is_allowed_and_reports_zero_lines()
    {
        var args = new TerminalReceivedDataEventArgs([]);

        Assert.Empty(args.Lines);
    }

    [Fact]
    public void ToString_includes_the_line_count()
    {
        var args = new TerminalReceivedDataEventArgs(["a", "b"]);

        Assert.Equal("Lines.Count: 2", args.ToString());
    }
}