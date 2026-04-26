namespace Atc.Wpf.Forms.Tests.FontEditing;

public sealed class InMemoryFontPickerStorageTests
{
    [Fact]
    public void GetRecentFontFamilies_returns_empty_list_initially()
    {
        var sut = new InMemoryFontPickerStorage();

        Assert.Empty(sut.GetRecentFontFamilies());
    }

    [Fact]
    public void Record_adds_new_entry_at_the_front()
    {
        var sut = new InMemoryFontPickerStorage();

        sut.RecordRecentFontFamily("Arial");
        sut.RecordRecentFontFamily("Verdana");
        sut.RecordRecentFontFamily("Calibri");

        var actual = sut.GetRecentFontFamilies();

        Assert.Equal(new[] { "Calibri", "Verdana", "Arial" }, actual);
    }

    [Fact]
    public void Recording_an_existing_entry_promotes_it_to_the_front()
    {
        var sut = new InMemoryFontPickerStorage();

        sut.RecordRecentFontFamily("Arial");
        sut.RecordRecentFontFamily("Verdana");
        sut.RecordRecentFontFamily("Calibri");
        sut.RecordRecentFontFamily("Arial");

        var actual = sut.GetRecentFontFamilies();

        Assert.Equal(new[] { "Arial", "Calibri", "Verdana" }, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Record_ignores_blank_input(string? blank)
    {
        var sut = new InMemoryFontPickerStorage();

        sut.RecordRecentFontFamily(blank!);

        Assert.Empty(sut.GetRecentFontFamilies());
    }

    [Fact]
    public void Record_caps_history_at_MaxRecentItems()
    {
        var sut = new InMemoryFontPickerStorage();

        for (var i = 0; i < InMemoryFontPickerStorage.MaxRecentItems + 5; i++)
        {
            sut.RecordRecentFontFamily($"Font{i}");
        }

        var actual = sut.GetRecentFontFamilies();

        Assert.Equal(InMemoryFontPickerStorage.MaxRecentItems, actual.Count);
        Assert.Equal($"Font{InMemoryFontPickerStorage.MaxRecentItems + 4}", actual[0]);
    }

    [Fact]
    public void GetRecentFontFamilies_returns_a_snapshot_that_is_not_affected_by_later_writes()
    {
        var sut = new InMemoryFontPickerStorage();
        sut.RecordRecentFontFamily("Arial");

        var snapshot = sut.GetRecentFontFamilies();
        sut.RecordRecentFontFamily("Verdana");

        Assert.Single(snapshot);
        Assert.Equal("Arial", snapshot[0]);
    }
}