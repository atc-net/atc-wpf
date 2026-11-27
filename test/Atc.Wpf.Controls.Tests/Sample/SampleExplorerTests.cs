namespace Atc.Wpf.Controls.Tests.Sample;

[Collection(nameof(PackResourceTestGroup))]
public sealed class SampleExplorerTests
{
    [Fact]
    public void SampleExplorerIds_NameTabsAndTrees()
    {
        Assert.Equal("TabWpfUI", SampleExplorerIds.Tab("WpfUI"));
        Assert.Equal("StvSamplesWpfUI", SampleExplorerIds.Tree("WpfUI"));
        Assert.Equal("TbSampleFilter", SampleExplorerIds.FilterTextBox);
        Assert.Equal("SamplesTabControl", SampleExplorerIds.TabControl);
        Assert.Equal("SampleRightToLeft", SampleExplorerIds.RightToLeftCheckBox);
    }

    [StaFact]
    public void BuildCategories_CreatesATabAndATreePerCategory_InOrder()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("WpfUI", typeof(BetaTreeView)));

        sut.BuildCategories();

        Assert.Equal(2, sut.SampleTreeViews.Count);
        Assert.IsType<AlphaTreeView>(sut.SampleTreeViews[0]);
        Assert.IsType<BetaTreeView>(sut.SampleTreeViews[1]);
        Assert.Equal(
            ["StvSamplesWpf", "StvSamplesWpfUI"],
            sut.SampleTreeViews.Select(AutomationProperties.GetAutomationId).ToArray());
        Assert.Equal(
            ["TabWpf", "TabWpfUI"],
            sut.CategoryTabs.Select(AutomationProperties.GetAutomationId).ToArray());
    }

    [StaFact]
    public void BuildCategories_TreeTypeNotATreeView_Throws()
    {
        var sut = CreateExplorer(("WpfUI", typeof(NotATree)));

        var exception = Assert.Throws<InvalidOperationException>(sut.BuildCategories);

        Assert.Contains("WpfUI", exception.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(NotATree), exception.Message, StringComparison.Ordinal);
    }

    [StaFact]
    public void BuildCategories_DuplicateKey_Throws()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("Wpf", typeof(BetaTreeView)));

        var exception = Assert.Throws<InvalidOperationException>(sut.BuildCategories);

        Assert.Contains("Wpf", exception.Message, StringComparison.Ordinal);
    }

    [StaFact]
    public void SelectingASample_SendsSampleItemMessage_AndSelectsItsTab()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("WpfUI", typeof(BetaTreeView)));
        SampleItemMessage? sent = null;
        sut.SendSampleItemMessage = message => sent = message;
        sut.BuildCategories();

        var item = sut.SampleTreeViews[1].Items.OfType<TreeViewItem>().First().Items.OfType<SampleTreeViewItem>().First();
        item.IsSelected = true;

        Assert.NotNull(sent);
        Assert.Equal("Beta", sent.Header);
        Assert.Equal("B.BetaView", sent.SampleItemPath);
        Assert.True(sut.CategoryTabs[1].IsSelected);
    }

    [StaFact]
    public void SelectingATab_ShowsOnlyItsTree()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("WpfUI", typeof(BetaTreeView)));
        sut.BuildCategories();

        sut.CategoryTabs[1].IsSelected = true;

        Assert.Equal(Visibility.Collapsed, sut.SampleTreeViews[0].Visibility);
        Assert.Equal(Visibility.Visible, sut.SampleTreeViews[1].Visibility);
    }

    [StaFact]
    public void FilterText_SetsTheTabBadgesToTheMatchCount()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("WpfUI", typeof(BetaTreeView)));
        sut.BuildCategories();

        sut.FilterTextBox.Text = "Alpha";

        Assert.Equal(1, sut.CategoryBadges[0].BadgeContent);
        Assert.Null(sut.CategoryBadges[1].BadgeContent);
        Assert.Equal(Visibility.Collapsed, sut.SampleTreeViews[1].Items.OfType<TreeViewItem>().First().Visibility);

        sut.FilterTextBox.Text = string.Empty;

        Assert.Null(sut.CategoryBadges[0].BadgeContent);
    }

    [StaFact]
    public void FilterText_DeselectsTheTab_AndShowsAllTrees()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)), ("WpfUI", typeof(BetaTreeView)));
        sut.BuildCategories();
        sut.CategoryTabs[1].IsSelected = true;

        sut.FilterTextBox.Text = "Alpha";

        Assert.False(sut.CategoryTabs[1].IsSelected);
        Assert.Equal(Visibility.Visible, sut.SampleTreeViews[0].Visibility);
        Assert.Equal(Visibility.Visible, sut.SampleTreeViews[1].Visibility);
    }

    [StaFact]
    public void CategoryItemStyle_DoesNotReachHostedSamplePages()
    {
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)));

        sut.BuildCategories();

        Assert.False(sut.Resources.Contains(typeof(TreeViewItem)));
        Assert.True(sut.TreePanel.Resources.Contains(typeof(TreeViewItem)));
        Assert.True(sut.TreePanel.Resources.Contains(typeof(SampleTreeViewItem)));
    }

    [StaFact]
    public void BuildCategories_BasesTheItemStylesOnTheAtcStyleFoundThen()
    {
        var baseStyle = new Style(typeof(TreeViewItem));
        var sut = CreateExplorer(("Wpf", typeof(AlphaTreeView)));
        sut.Resources["AtcApps.Styles.TreeViewItem"] = baseStyle;

        sut.BuildCategories();

        Assert.Same(baseStyle, ((Style)sut.TreePanel.Resources[typeof(TreeViewItem)]).BasedOn);
        Assert.Same(baseStyle, ((Style)sut.TreePanel.Resources[typeof(SampleTreeViewItem)]).BasedOn);
    }

    // The selectors reserve an (empty) validation row under their drop-downs; without hiding it the drop-downs sit
    // above the middle of the row and the vertically centred "Right to left" check box looks lower.
    [StaFact]
    public void HeaderSelectors_HideTheirValidationArea_SoTheyLineUpWithTheRightToLeftCheckBox()
    {
        var sut = new SampleExplorer();

        Assert.True(sut.LanguageSelector.HideAreas.HasFlag(Atc.Wpf.Forms.LabelControlHideAreasType.Validation));
        Assert.True(sut.ThemeAndAccentSelectors.HideAreas.HasFlag(Atc.Wpf.Forms.LabelControlHideAreasType.Validation));
        Assert.Equal(VerticalAlignment.Center, sut.RightToLeftCheckBox.VerticalAlignment);
        Assert.Equal(VerticalAlignment.Center, sut.LanguageSelector.VerticalAlignment);
        Assert.Equal(VerticalAlignment.Center, sut.ThemeAndAccentSelectors.VerticalAlignment);
    }

    [StaFact]
    public void StatusText_IsShownInTheStatusBar()
    {
        var sut = new SampleExplorer { StatusText = "Offline sample data" };

        Assert.Equal("Offline sample data", sut.StatusTextBlock.Text);
    }

    private static SampleExplorer CreateExplorer(
        params (string Key, Type TreeType)[] categories)
    {
        var explorer = new SampleExplorer();
        foreach (var (key, treeType) in categories)
        {
            explorer.Categories.Add(new SampleCategory { Key = key, Header = key, TreeType = treeType });
        }

        return explorer;
    }

    private sealed class AlphaTreeView : TreeView
    {
        public AlphaTreeView()
        {
            var category = new TreeViewItem { Header = "Letters" };
            category.Items.Add(new SampleTreeViewItem { Header = "Alpha", SamplePath = "A.AlphaView" });
            Items.Add(category);
        }
    }

    private sealed class BetaTreeView : TreeView
    {
        public BetaTreeView()
        {
            var category = new TreeViewItem { Header = "Letters" };
            category.Items.Add(new SampleTreeViewItem { Header = "Beta", SamplePath = "B.BetaView" });
            Items.Add(category);
        }
    }

    private sealed class NotATree : Control;
}