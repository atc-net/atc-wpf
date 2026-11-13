namespace Atc.Wpf.Theming.Tests.Helpers;

// The test host has no Application and therefore no theme: the helpers must return their documented fallbacks instead of throwing.
public sealed class ThemeManagerHelperTests
{
    [Fact]
    public void GetColorByResourceKey_WithoutATheme_ReturnsDeepPink()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        Assert.Equal(Colors.DeepPink, ThemeManagerHelper.GetColorByResourceKey(AtcAppsColorKeyType.Accent));
    }

    [Fact]
    public void GetBrushByResourceKey_WithoutATheme_ReturnsDeepPink()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        Assert.Same(Brushes.DeepPink, ThemeManagerHelper.GetBrushByResourceKey(AtcAppsBrushKeyType.Accent));
    }

    [Fact]
    public void GetPrimaryAccentColor_WithoutATheme_ReturnsDeepPink()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        Assert.Equal(Colors.DeepPink, ThemeManagerHelper.GetPrimaryAccentColor());
    }

    [Fact]
    public void GetPrimaryAccentBrush_WithoutATheme_ReturnsAFrozenDeepPinkBrush()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        var brush = ThemeManagerHelper.GetPrimaryAccentBrush();

        Assert.Equal(Colors.DeepPink, brush.Color);
        Assert.True(brush.IsFrozen);
    }

    // A caller that must tell "missing" from a real colour uses the TryGet overloads.
    [Fact]
    public void TryGetColorByResourceKey_WithoutATheme_ReturnsFalse()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        var found = ThemeManagerHelper.TryGetColorByResourceKey("Accent", out var color);

        Assert.False(found);
        Assert.Equal(default, color);
    }

    [Fact]
    public void TryGetBrushByResourceKey_WithoutATheme_ReturnsFalse()
    {
        Assert.SkipWhen(Application.Current is not null, "An Application exists in this test host.");

        var found = ThemeManagerHelper.TryGetBrushByResourceKey("Accent", out var brush);

        Assert.False(found);
        Assert.Null(brush);
    }
}