namespace Atc.Wpf.Controls.Tests.Sample;

public sealed class MarkdownFileLookupTests
{
    private static readonly string Root = Path.Combine(Path.GetTempPath(), "repo");

    [Fact]
    public void Find_IgnoresAFileWhoseNameOnlyEndsWithTheName()
    {
        var files = Files(
            Path.Combine("src", "Zoom", "ZoomScrollViewer_Readme.md"),
            Path.Combine("src", "Theming", "ScrollViewer_Readme.md"));

        var file = MarkdownFileLookup.Find(files, "ScrollViewer_Readme");

        Assert.Equal(files[1].FullName, file?.FullName);
    }

    [Fact]
    public void Find_ReturnsNullWhenOnlyALongerNameMatches()
    {
        var files = Files(Path.Combine("src", "Pickers", "WellKnownColorPicker_Readme.md"));

        Assert.Null(MarkdownFileLookup.Find(files, "ColorPicker_Readme"));
    }

    [Fact]
    public void Find_MatchesWholeFolderNames()
    {
        var files = Files(
            Path.Combine("docs", "ZoomLayouts", "@Readme.md"),
            Path.Combine("docs", "Layouts", "@Readme.md"));

        var file = MarkdownFileLookup.Find(files, Path.Combine("Layouts", "@Readme"));

        Assert.Equal(files[1].FullName, file?.FullName);
    }

    [Fact]
    public void Find_IgnoresCase()
    {
        var files = Files(Path.Combine("src", "Badge_Readme.md"));

        Assert.Equal(files[0].FullName, MarkdownFileLookup.Find(files, "badge_readme")?.FullName);
    }

    private static FileInfo[] Files(params string[] relativePaths)
        => relativePaths.Select(p => new FileInfo(Path.Combine(Root, p))).ToArray();
}