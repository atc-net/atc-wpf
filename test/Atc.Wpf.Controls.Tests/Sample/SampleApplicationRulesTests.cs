namespace Atc.Wpf.Controls.Tests.Sample;

public sealed class SampleApplicationRulesTests
{
    private const string TraceVariable = "MY_SAMPLE_TRACE_BINDING_ERRORS";

    [Fact]
    public void ShouldTraceBindingErrors_WithADebugger_IsTrue()
        => Assert.True(SampleApplicationRules.ShouldTraceBindingErrors(
            isDebuggerAttached: true,
            variableName: null,
            _ => null));

    [Fact]
    public void ShouldTraceBindingErrors_VariableSetToOne_IsTrue()
        => Assert.True(SampleApplicationRules.ShouldTraceBindingErrors(
            isDebuggerAttached: false,
            TraceVariable,
            name => name == TraceVariable ? "1" : null));

    [Theory]
    [InlineData("0")]
    [InlineData("true")]
    [InlineData(null)]
    public void ShouldTraceBindingErrors_VariableNotOne_IsFalse(string? value)
        => Assert.False(SampleApplicationRules.ShouldTraceBindingErrors(
            isDebuggerAttached: false,
            TraceVariable,
            _ => value));

    [Fact]
    public void ShouldTraceBindingErrors_NoVariableName_IsFalse()
        => Assert.False(SampleApplicationRules.ShouldTraceBindingErrors(
            isDebuggerAttached: false,
            variableName: null,
            _ => "1"));

    [Fact]
    public void StyleDictionaries_AreThemingControlsAndComponents_InOrder()
        => Assert.Equal(
            [
                "pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml",
                "pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml",
                "pack://application:,,,/Atc.Wpf.Components;component/Styles/Controls.xaml",
            ],
            SampleApplicationRules.StyleDictionaries.Select(x => x.OriginalString).ToArray());

    [Fact]
    public void MissingStyleDictionaries_NothingMerged_AreAllThree()
        => Assert.Equal(
            SampleApplicationRules.StyleDictionaries,
            SampleApplicationRules.MissingStyleDictionaries([]));

    [Fact]
    public void MissingStyleDictionaries_ThemingMergedWithAnotherSpelling_AreTheOtherTwo()
    {
        var missing = SampleApplicationRules.MissingStyleDictionaries(
        [
            new Uri("/atc.wpf.theming;component/Styles/Default.xaml", UriKind.Relative),
            new Uri("/Fluent;Component/Themes/Generic.xaml", UriKind.Relative),
            null,
        ]);

        Assert.Equal(SampleApplicationRules.StyleDictionaries.Skip(1), missing);
    }

    [Fact]
    public void MissingStyleDictionaries_AllMerged_IsEmpty()
        => Assert.Empty(SampleApplicationRules.MissingStyleDictionaries(SampleApplicationRules.StyleDictionaries));

    [StaFact]
    public void MergedSources_IncludesNestedDictionaries()
    {
        var nested = new ResourceDictionary();
        nested.MergedDictionaries.Add(new ResourceDictionary());
        var root = new ResourceDictionary();
        root.MergedDictionaries.Add(nested);

        Assert.Equal(2, SampleApplicationRules.MergedSources(root).Count());
    }

    [Fact]
    public void ExceptionDialogGate_LetsOneDialogThroughAtATime()
    {
        var sut = new SampleApplicationRules.ExceptionDialogGate();

        Assert.True(sut.TryEnter());
        Assert.False(sut.TryEnter());

        sut.Exit();

        Assert.True(sut.TryEnter());
    }
}