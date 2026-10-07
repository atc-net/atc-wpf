// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class AnsiSequenceParserTests
{
    private const string Esc = "\u001b";

    [Fact]
    public void ContainsEscapeSequence_Null_ReturnsFalse()
        => Assert.False(AnsiSequenceParser.ContainsEscapeSequence(null!));

    [Fact]
    public void ContainsEscapeSequence_Empty_ReturnsFalse()
        => Assert.False(AnsiSequenceParser.ContainsEscapeSequence(string.Empty));

    [Fact]
    public void ContainsEscapeSequence_PlainText_ReturnsFalse()
        => Assert.False(AnsiSequenceParser.ContainsEscapeSequence("hello [31m world"));

    [Fact]
    public void ContainsEscapeSequence_TextWithEscape_ReturnsTrue()
        => Assert.True(AnsiSequenceParser.ContainsEscapeSequence("a" + Esc + "[31mb"));

    [Fact]
    public void Parse_NullText_Throws()
        => Assert.Throws<ArgumentNullException>(
            () => AnsiSequenceParser.Parse(null!, AnsiSgrState.Default));

    [Fact]
    public void Parse_NullState_Throws()
        => Assert.Throws<ArgumentNullException>(
            () => AnsiSequenceParser.Parse("text", null!));

    [Fact]
    public void Parse_EmptyText_ReturnsNoRunsAndUnchangedState()
    {
        var (runs, newState) = AnsiSequenceParser.Parse(string.Empty, AnsiSgrState.Default);

        Assert.Empty(runs);
        Assert.Same(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_TextWithoutEscapes_ReturnsSingleUnstyledRun()
    {
        var (runs, newState) = AnsiSequenceParser.Parse("plain text", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.Equal("plain text", run.Text);
        AssertUnstyled(run);
        Assert.Same(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_TextWithoutEscapes_UsesIncomingState()
    {
        var state = AnsiSgrState.Default with { Bold = true, Italic = true };

        var (runs, newState) = AnsiSequenceParser.Parse("styled", state);

        var run = Assert.Single(runs);
        Assert.Equal("styled", run.Text);
        Assert.True(run.Bold);
        Assert.True(run.Italic);
        Assert.False(run.Underline);
        Assert.Same(state, newState);
    }

    [Fact]
    public void Parse_OnlyEscapeSequence_ReturnsNoRunsButUpdatesState()
    {
        var (runs, newState) = AnsiSequenceParser.Parse(Esc + "[1m", AnsiSgrState.Default);

        Assert.Empty(runs);
        Assert.True(newState.Bold);
    }

    [Fact]
    public void Parse_EscapesAtStartEndAndBackToBack_ProduceNoEmptyRuns()
    {
        var text = Esc + "[1m" + Esc + "[3mabc" + Esc + "[0m";

        var (runs, newState) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.Equal("abc", run.Text);
        Assert.True(run.Bold);
        Assert.True(run.Italic);
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_Bold_SetsBoldOnFollowingText()
    {
        var (runs, _) = AnsiSequenceParser.Parse("a" + Esc + "[1mb", AnsiSgrState.Default);

        Assert.Equal(2, runs.Count);
        Assert.Equal("a", runs[0].Text);
        Assert.False(runs[0].Bold);
        Assert.Equal("b", runs[1].Text);
        Assert.True(runs[1].Bold);
        Assert.False(runs[1].Italic);
        Assert.False(runs[1].Underline);
    }

    [Fact]
    public void Parse_Code22_ClearsBoldOnly()
    {
        var state = AnsiSgrState.Default with { Bold = true, Italic = true, Underline = true };

        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[22m", state);

        Assert.False(newState.Bold);
        Assert.True(newState.Italic);
        Assert.True(newState.Underline);
    }

    [Fact]
    public void Parse_Italic_SetsItalicOnFollowingText()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[3mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.True(run.Italic);
        Assert.False(run.Bold);
        Assert.False(run.Underline);
    }

    [Fact]
    public void Parse_Code23_ClearsItalicOnly()
    {
        var state = AnsiSgrState.Default with { Bold = true, Italic = true, Underline = true };

        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[23m", state);

        Assert.True(newState.Bold);
        Assert.False(newState.Italic);
        Assert.True(newState.Underline);
    }

    [Fact]
    public void Parse_Underline_SetsUnderlineOnFollowingText()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[4mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.True(run.Underline);
        Assert.False(run.Bold);
        Assert.False(run.Italic);
    }

    [Fact]
    public void Parse_Code24_ClearsUnderlineOnly()
    {
        var state = AnsiSgrState.Default with { Bold = true, Italic = true, Underline = true };

        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[24m", state);

        Assert.True(newState.Bold);
        Assert.True(newState.Italic);
        Assert.False(newState.Underline);
    }

    [Fact]
    public void Parse_Reset_ClearsAllAttributesAndColors()
    {
        var text = Esc + "[1;3;4;31;42mstyled" + Esc + "[0mplain";

        var (runs, newState) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        Assert.Equal(2, runs.Count);
        Assert.Equal("plain", runs[1].Text);
        AssertUnstyled(runs[1]);
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_EmptySgrParameters_ActsAsReset()
    {
        var state = AnsiSgrState.Default with { Bold = true, Underline = true };

        var (runs, newState) = AnsiSequenceParser.Parse(Esc + "[mx", state);

        AssertUnstyled(Assert.Single(runs));
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_ResetFollowedByAttributeInSameSequence_AppliesAttributeAfterReset()
    {
        var state = AnsiSgrState.Default with { Italic = true };

        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[0;1m", state);

        Assert.True(newState.Bold);
        Assert.False(newState.Italic);
    }

    [Fact]
    public void Parse_CombinedParameters_AppliesAllOfThem()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[1;3;4;31;42mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.True(run.Bold);
        Assert.True(run.Italic);
        Assert.True(run.Underline);
        AssertColor(run.Foreground, 0xC5, 0x0F, 0x1F);
        AssertColor(run.Background, 0x13, 0xA1, 0x0E);
    }

    [Theory]
    [InlineData(30, 0x00, 0x00, 0x00)]
    [InlineData(31, 0xC5, 0x0F, 0x1F)]
    [InlineData(32, 0x13, 0xA1, 0x0E)]
    [InlineData(33, 0xC1, 0x9C, 0x00)]
    [InlineData(34, 0x00, 0x37, 0xDA)]
    [InlineData(35, 0x88, 0x17, 0x98)]
    [InlineData(36, 0x3A, 0x96, 0xDD)]
    [InlineData(37, 0xCC, 0xCC, 0xCC)]
    [InlineData(90, 0x76, 0x76, 0x76)]
    [InlineData(91, 0xE7, 0x48, 0x56)]
    [InlineData(92, 0x16, 0xC6, 0x0C)]
    [InlineData(93, 0xF9, 0xF1, 0xA5)]
    [InlineData(94, 0x3B, 0x78, 0xFF)]
    [InlineData(95, 0xB4, 0x00, 0x9E)]
    [InlineData(96, 0x61, 0xD6, 0xD6)]
    [InlineData(97, 0xF2, 0xF2, 0xF2)]
    public void Parse_16ColorForegroundCode_SetsPaletteForeground(
        int code,
        byte r,
        byte g,
        byte b)
    {
        var text = Esc + "[" + code.ToString(CultureInfo.InvariantCulture) + "mx";

        var (runs, _) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Foreground, r, g, b);
        Assert.Null(run.Background);
    }

    [Theory]
    [InlineData(40, 0x00, 0x00, 0x00)]
    [InlineData(41, 0xC5, 0x0F, 0x1F)]
    [InlineData(42, 0x13, 0xA1, 0x0E)]
    [InlineData(43, 0xC1, 0x9C, 0x00)]
    [InlineData(44, 0x00, 0x37, 0xDA)]
    [InlineData(45, 0x88, 0x17, 0x98)]
    [InlineData(46, 0x3A, 0x96, 0xDD)]
    [InlineData(47, 0xCC, 0xCC, 0xCC)]
    [InlineData(100, 0x76, 0x76, 0x76)]
    [InlineData(101, 0xE7, 0x48, 0x56)]
    [InlineData(102, 0x16, 0xC6, 0x0C)]
    [InlineData(103, 0xF9, 0xF1, 0xA5)]
    [InlineData(104, 0x3B, 0x78, 0xFF)]
    [InlineData(105, 0xB4, 0x00, 0x9E)]
    [InlineData(106, 0x61, 0xD6, 0xD6)]
    [InlineData(107, 0xF2, 0xF2, 0xF2)]
    public void Parse_16ColorBackgroundCode_SetsPaletteBackground(
        int code,
        byte r,
        byte g,
        byte b)
    {
        var text = Esc + "[" + code.ToString(CultureInfo.InvariantCulture) + "mx";

        var (runs, _) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Background, r, g, b);
        Assert.Null(run.Foreground);
    }

    [Fact]
    public void Parse_Code39_ResetsForegroundOnly()
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[1;31;42m" + Esc + "[39m", AnsiSgrState.Default);

        Assert.Null(newState.Foreground);
        AssertColor(newState.Background, 0x13, 0xA1, 0x0E);
        Assert.True(newState.Bold);
    }

    [Fact]
    public void Parse_Code49_ResetsBackgroundOnly()
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[1;31;42m" + Esc + "[49m", AnsiSgrState.Default);

        AssertColor(newState.Foreground, 0xC5, 0x0F, 0x1F);
        Assert.Null(newState.Background);
        Assert.True(newState.Bold);
    }

    [Fact]
    public void Parse_256ColorForegroundLowIndex_MapsToBright16Palette()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[38;5;9mx", AnsiSgrState.Default);

        AssertColor(Assert.Single(runs).Foreground, 0xE7, 0x48, 0x56);
    }

    [Fact]
    public void Parse_256ColorForegroundIndexZero_MapsToBlack()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[38;5;0mx", AnsiSgrState.Default);

        AssertColor(Assert.Single(runs).Foreground, 0x00, 0x00, 0x00);
    }

    [Fact]
    public void Parse_256ColorBackgroundLowIndex_MapsToBright16Palette()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[48;5;12mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Background, 0x3B, 0x78, 0xFF);
        Assert.Null(run.Foreground);
    }

    [Fact]
    public void Parse_256ColorIndex_IsConsumedAndNotReinterpretedAsAttribute()
    {
        // The "1" is the colour index (red), not SGR bold; "3" afterwards is italic.
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[38;5;1;3m", AnsiSgrState.Default);

        AssertColor(newState.Foreground, 0xC5, 0x0F, 0x1F);
        Assert.False(newState.Bold);
        Assert.True(newState.Italic);
    }

    [Fact]
    public void Parse_256ColorExtendedIndex_StillAppliesFollowingParameters()
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[38;5;196;1m", AnsiSgrState.Default);

        Assert.True(newState.Bold);
        Assert.False(newState.Italic);
        Assert.False(newState.Underline);
    }

    [Fact]
    public void Parse_TrueColorForeground_SetsExactRgb()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[38;2;10;20;30mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Foreground, 10, 20, 30);
        Assert.Null(run.Background);
    }

    [Fact]
    public void Parse_TrueColorBackground_SetsExactRgb()
    {
        var (runs, _) = AnsiSequenceParser.Parse(Esc + "[48;2;255;128;0mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Background, 0xFF, 0x80, 0x00);
        Assert.Null(run.Foreground);
    }

    [Fact]
    public void Parse_TrueColorBrush_IsFrozen()
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[38;2;1;2;3m", AnsiSgrState.Default);

        Assert.NotNull(newState.Foreground);
        Assert.True(newState.Foreground.IsFrozen);
    }

    [Fact]
    public void Parse_TrueColorComponentsAbove255_AreClamped()
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[48;2;300;0;999m", AnsiSgrState.Default);

        AssertColor(newState.Background, 0xFF, 0x00, 0xFF);
    }

    [Fact]
    public void Parse_TrueColorComponents_AreConsumedAndFollowingParametersApplied()
    {
        // 1, 3 and 4 are colour components here, not bold/italic/underline; the trailing 1 is bold.
        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[38;2;1;3;4;1m", AnsiSgrState.Default);

        AssertColor(newState.Foreground, 1, 3, 4);
        Assert.True(newState.Bold);
        Assert.False(newState.Italic);
        Assert.False(newState.Underline);
    }

    [Fact]
    public void Parse_TrueColorForegroundAndBackgroundInOneSequence_SetsBoth()
    {
        var (_, newState) = AnsiSequenceParser.Parse(
            Esc + "[38;2;1;2;3;48;2;4;5;6m",
            AnsiSgrState.Default);

        AssertColor(newState.Foreground, 1, 2, 3);
        AssertColor(newState.Background, 4, 5, 6);
    }

    [Fact]
    public void Parse_ExtendedColorWithoutMode_LeavesStateUnchanged()
    {
        var state = AnsiSgrState.Default with { Italic = true };

        var (runs, newState) = AnsiSequenceParser.Parse(Esc + "[38mx", state);

        var run = Assert.Single(runs);
        Assert.Equal("x", run.Text);
        Assert.Null(newState.Foreground);
        Assert.Equal(state, newState);
    }

    [Fact]
    public void Parse_256ColorWithoutIndex_LeavesColorUnchanged()
    {
        var (runs, newState) = AnsiSequenceParser.Parse(Esc + "[48;5mx", AnsiSgrState.Default);

        Assert.Equal("x", Assert.Single(runs).Text);
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_UnknownSgrCode_IsIgnored()
    {
        var (runs, newState) = AnsiSequenceParser.Parse(Esc + "[5;7;31mx", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        AssertColor(run.Foreground, 0xC5, 0x0F, 0x1F);
        Assert.False(newState.Bold);
        Assert.False(newState.Italic);
        Assert.False(newState.Underline);
    }

    [Fact]
    public void Parse_NonSgrCsiSequence_IsStrippedWithoutChangingState()
    {
        var text = "a" + Esc + "[2J" + Esc + "[10;5H" + "b";

        var (runs, newState) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        Assert.Equal(2, runs.Count);
        Assert.Equal("a", runs[0].Text);
        Assert.Equal("b", runs[1].Text);
        AssertUnstyled(runs[1]);
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_UnterminatedEscapeSequence_DoesNotThrowOrChangeStyle()
    {
        var state = AnsiSgrState.Default with { Bold = true };
        var text = "abc" + Esc + "[31";

        var (runs, newState) = AnsiSequenceParser.Parse(text, state);

        var run = Assert.Single(runs);
        Assert.StartsWith("abc", run.Text, StringComparison.Ordinal);
        Assert.True(run.Bold);
        Assert.Null(run.Foreground);
        Assert.Same(state, newState);
    }

    [Fact]
    public void Parse_UnterminatedEscapeSequence_IsKeptAsLiteralText()
    {
        var text = "abc" + Esc + "[31";

        var (runs, _) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        Assert.Equal(text, Assert.Single(runs).Text);
    }

    [Fact]
    public void Parse_BareEscapeWithoutBracket_IsKeptAsLiteralText()
    {
        var text = "x" + Esc + "31my";

        var (runs, newState) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.Equal(text, run.Text);
        Assert.Null(run.Foreground);
        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_EmptyParameterInList_IsTreatedAsReset()
    {
        var state = AnsiSgrState.Default with { Underline = true };

        var (_, newState) = AnsiSequenceParser.Parse(Esc + "[;1m", state);

        Assert.True(newState.Bold);
        Assert.False(newState.Underline);
    }

    [Fact]
    public void Parse_MultipleRunsOnOneLine_SplitsAtEachStyleChange()
    {
        var text = "plain " + Esc + "[31mred " + Esc + "[1;44mbold-on-blue" + Esc + "[0m tail";

        var (runs, newState) = AnsiSequenceParser.Parse(text, AnsiSgrState.Default);

        Assert.Equal(4, runs.Count);

        Assert.Equal("plain ", runs[0].Text);
        AssertUnstyled(runs[0]);

        Assert.Equal("red ", runs[1].Text);
        AssertColor(runs[1].Foreground, 0xC5, 0x0F, 0x1F);
        Assert.Null(runs[1].Background);
        Assert.False(runs[1].Bold);

        Assert.Equal("bold-on-blue", runs[2].Text);
        AssertColor(runs[2].Foreground, 0xC5, 0x0F, 0x1F);
        AssertColor(runs[2].Background, 0x00, 0x37, 0xDA);
        Assert.True(runs[2].Bold);

        Assert.Equal(" tail", runs[3].Text);
        AssertUnstyled(runs[3]);

        Assert.Equal(AnsiSgrState.Default, newState);
    }

    [Fact]
    public void Parse_ReturnedState_CarriesStyleIntoNextCall()
    {
        var (firstRuns, firstState) = AnsiSequenceParser.Parse(
            "first " + Esc + "[1;32mstart",
            AnsiSgrState.Default);

        var (secondRuns, secondState) = AnsiSequenceParser.Parse("continued", firstState);

        Assert.Equal(2, firstRuns.Count);
        var run = Assert.Single(secondRuns);
        Assert.Equal("continued", run.Text);
        Assert.True(run.Bold);
        AssertColor(run.Foreground, 0x13, 0xA1, 0x0E);
        Assert.Same(firstState, secondState);
    }

    [Fact]
    public void Parse_ResetInLaterCall_ClearsCarriedState()
    {
        var (_, firstState) = AnsiSequenceParser.Parse(Esc + "[4;35m", AnsiSgrState.Default);

        var (runs, secondState) = AnsiSequenceParser.Parse(
            "still" + Esc + "[0mnormal",
            firstState);

        Assert.Equal(2, runs.Count);
        Assert.True(runs[0].Underline);
        AssertColor(runs[0].Foreground, 0x88, 0x17, 0x98);
        AssertUnstyled(runs[1]);
        Assert.Equal(AnsiSgrState.Default, secondState);
    }

    [Fact]
    public void Parse_DoesNotMutateIncomingState()
    {
        var state = AnsiSgrState.Default;

        _ = AnsiSequenceParser.Parse(Esc + "[1;3;4;31;42mx", state);

        Assert.Null(AnsiSgrState.Default.Foreground);
        Assert.Null(AnsiSgrState.Default.Background);
        Assert.False(AnsiSgrState.Default.Bold);
        Assert.False(AnsiSgrState.Default.Italic);
        Assert.False(AnsiSgrState.Default.Underline);
    }

    private static void AssertUnstyled(TerminalRun run)
    {
        Assert.Null(run.Foreground);
        Assert.Null(run.Background);
        Assert.False(run.Bold);
        Assert.False(run.Italic);
        Assert.False(run.Underline);
    }

    private static void AssertColor(
        Brush? brush,
        byte r,
        byte g,
        byte b)
    {
        var solid = Assert.IsType<SolidColorBrush>(brush);
        Assert.Equal(Color.FromRgb(r, g, b), solid.Color);
    }

    [Theory]
    [InlineData("[?25l")]
    [InlineData("[?2004h")]
    [InlineData("[?1049h")]
    public void Parse_PrivateModeSequence_IsStrippedFromTheText(string sequence)
    {
        var (runs, newState) = AnsiSequenceParser.Parse(Esc + sequence + "X", AnsiSgrState.Default);

        var run = Assert.Single(runs);
        Assert.Equal("X", run.Text);
        Assert.Same(AnsiSgrState.Default, newState);
    }

    [Theory]
    [InlineData("[38;2;1;2m")]
    [InlineData("[48;2;4m")]
    [InlineData("[38;2m")]
    public void Parse_TruncatedTruecolorSequence_DoesNotTurnItsNumbersIntoAttributes(
        string sequence)
    {
        var (_, newState) = AnsiSequenceParser.Parse(Esc + sequence + "X", AnsiSgrState.Default);

        Assert.False(newState.Bold);
        Assert.False(newState.Italic);
        Assert.False(newState.Underline);
        Assert.Null(newState.Foreground);
        Assert.Null(newState.Background);
    }
}