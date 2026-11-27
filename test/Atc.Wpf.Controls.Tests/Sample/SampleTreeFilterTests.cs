namespace Atc.Wpf.Controls.Tests.Sample;

public sealed class SampleTreeFilterTests
{
    [Theory]
    [InlineData("NiceWindow", "NW", true)]
    [InlineData("NiceWindow", "nw", false)]
    [InlineData("LabelToggleSwitch", "TS", true)]
    [InlineData("ImageButton", "button", true)]
    [InlineData("ImageButton", "IB", true)]
    [InlineData("ImageButton", "BI", false)]
    [InlineData(null, "x", false)]
    [InlineData("Badge", "", false)]
    public void Matches_TextOrPascalCaseInitials(
        string? text,
        string filter,
        bool expected)
        => Assert.Equal(expected, SampleTreeFilter.Matches(text, filter));

    [StaFact]
    public void ApplyFilter_HidesNonMatches_ShowsAndExpandsTheirParents()
    {
        var (items, category, imageButton, toggleSwitch) = CreateTree();
        category.IsExpanded = false;

        var anyVisible = SampleTreeFilter.ApplyFilter(items, "IB");

        Assert.True(anyVisible);
        Assert.Equal(Visibility.Visible, imageButton.Visibility);
        Assert.Equal(Visibility.Collapsed, toggleSwitch.Visibility);
        Assert.Equal(Visibility.Visible, category.Visibility);
        Assert.True(category.IsExpanded);
    }

    [StaFact]
    public void ApplyFilter_NoMatch_HidesEverything()
    {
        var (items, category, _, _) = CreateTree();

        var anyVisible = SampleTreeFilter.ApplyFilter(items, "zzz");

        Assert.False(anyVisible);
        Assert.Equal(Visibility.Collapsed, category.Visibility);
    }

    [StaFact]
    public void ApplyFilter_WhitespaceFilter_ShowsEverything()
    {
        var (items, category, imageButton, toggleSwitch) = CreateTree();
        SampleTreeFilter.ApplyFilter(items, "IB");

        var anyVisible = SampleTreeFilter.ApplyFilter(items, "   ");

        Assert.True(anyVisible);
        Assert.Equal(Visibility.Visible, category.Visibility);
        Assert.Equal(Visibility.Visible, imageButton.Visibility);
        Assert.Equal(Visibility.Visible, toggleSwitch.Visibility);
    }

    [StaFact]
    public void ApplyFilter_MatchesTheSamplePath()
    {
        var (items, _, imageButton, toggleSwitch) = CreateTree();

        SampleTreeFilter.ApplyFilter(items, "ImageButtonView");

        Assert.Equal(Visibility.Visible, imageButton.Visibility);
        Assert.Equal(Visibility.Collapsed, toggleSwitch.Visibility);
    }

    [StaFact]
    public void ApplyFilter_NonStringHeader_DoesNotThrowOrMatchTheElementType()
    {
        var item = new TreeViewItem { Header = new StackPanel() };

        var anyVisible = SampleTreeFilter.ApplyFilter(new[] { item }, "Stack");

        Assert.False(anyVisible);
        Assert.Equal(Visibility.Collapsed, item.Visibility);
    }

    [StaFact]
    public void CountMatches_CountsOnlySampleItems()
    {
        var (items, _, _, _) = CreateTree();

        Assert.Equal(1, SampleTreeFilter.CountMatches(items, "Button"));
    }

    [StaFact]
    public void CountMatches_EmptyFilter_IsZero()
    {
        var (items, _, _, _) = CreateTree();

        Assert.Equal(0, SampleTreeFilter.CountMatches(items, " "));
    }

    [StaFact]
    public void SetExpanded_ExpandsAndCollapsesRecursively()
    {
        var category = new TreeViewItem { Header = "Outer" };
        var inner = new TreeViewItem { Header = "Inner" };
        category.Items.Add(inner);

        SampleTreeFilter.SetExpanded(new[] { category }, isExpanded: true);
        Assert.True(category.IsExpanded);
        Assert.True(inner.IsExpanded);

        SampleTreeFilter.SetExpanded(new[] { category }, isExpanded: false);
        Assert.False(category.IsExpanded);
        Assert.False(inner.IsExpanded);
    }

    private static (TreeViewItem[] Items, TreeViewItem Category, SampleTreeViewItem ImageButton, SampleTreeViewItem ToggleSwitch) CreateTree()
    {
        var category = new TreeViewItem { Header = "Buttons" };
        var imageButton = new SampleTreeViewItem { Header = "ImageButton", SamplePath = "Inputs.ImageButtonView" };
        var toggleSwitch = new SampleTreeViewItem { Header = "ToggleSwitch", SamplePath = "Inputs.ToggleSwitchView" };
        category.Items.Add(imageButton);
        category.Items.Add(toggleSwitch);
        return ([category], category, imageButton, toggleSwitch);
    }
}