namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorItemTests
{
    [Fact]
    public void Defaults_match_documented_initial_state()
    {
        var item = new DualListSelectorItem();

        Assert.Null(item.Identifier);
        Assert.Equal(string.Empty, item.Name);
        Assert.Null(item.Description);
        Assert.Null(item.SortOrderNumber);
        Assert.True(item.IsEnabled);
        Assert.Null(item.Tag);
    }

    [Fact]
    public void ToString_returns_Name_only()
    {
        var item = new DualListSelectorItem
        {
            Identifier = "id-42",
            Name = "Display name",
            Description = "Verbose description",
        };

        Assert.Equal("Display name", item.ToString());
    }
}