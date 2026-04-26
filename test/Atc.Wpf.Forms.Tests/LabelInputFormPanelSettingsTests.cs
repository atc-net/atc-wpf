namespace Atc.Wpf.Forms.Tests;

public sealed class LabelInputFormPanelSettingsTests
{
    [Fact]
    public void Defaults_match_documented_initial_state()
    {
        var sut = new LabelInputFormPanelSettings();

        Assert.Equal(new Size(1920, 1200), sut.MaxSize);
        Assert.Equal(Orientation.Vertical, sut.ControlOrientation);
        Assert.False(sut.UseGroupBox);
        Assert.Equal(320, sut.ControlWidth);
    }

    [Fact]
    public void Properties_round_trip_assignments()
    {
        var sut = new LabelInputFormPanelSettings
        {
            MaxSize = new Size(800, 600),
            ControlOrientation = Orientation.Horizontal,
            UseGroupBox = true,
            ControlWidth = 240,
        };

        Assert.Equal(new Size(800, 600), sut.MaxSize);
        Assert.Equal(Orientation.Horizontal, sut.ControlOrientation);
        Assert.True(sut.UseGroupBox);
        Assert.Equal(240, sut.ControlWidth);
    }

    [Fact]
    public void ToString_includes_all_property_names()
    {
        var sut = new LabelInputFormPanelSettings();

        var actual = sut.ToString();

        Assert.Contains(nameof(LabelInputFormPanelSettings.MaxSize), actual, StringComparison.Ordinal);
        Assert.Contains(nameof(LabelInputFormPanelSettings.ControlOrientation), actual, StringComparison.Ordinal);
        Assert.Contains(nameof(LabelInputFormPanelSettings.UseGroupBox), actual, StringComparison.Ordinal);
        Assert.Contains(nameof(LabelInputFormPanelSettings.ControlWidth), actual, StringComparison.Ordinal);
    }
}