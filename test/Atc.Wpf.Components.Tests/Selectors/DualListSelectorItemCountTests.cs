namespace Atc.Wpf.Components.Tests.Selectors;

/// <summary>
/// The footer counts are template parts the control updates in code, so they must follow
/// changes made to the collections from outside the control as well.
/// </summary>
public sealed class DualListSelectorItemCountTests
{
    private const string TemplateXaml = """
        <ControlTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            TargetType="Control">
            <StackPanel>
                <ListBox x:Name="PART_AvailableListBox" />
                <ListBox x:Name="PART_SelectedListBox" />
                <TextBlock x:Name="PART_AvailableItemCount" />
                <TextBlock x:Name="PART_SelectedItemCount" />
                <TextBlock x:Name="PART_AvailableEmptyText" />
                <TextBlock x:Name="PART_SelectedEmptyText" />
            </StackPanel>
        </ControlTemplate>
        """;

    [StaFact]
    public void AvailableItemsChangedFromOutside_UpdatesTheCount()
    {
        var selector = CreateTemplatedSelector();
        var count = (TextBlock)selector.Template.FindName("PART_AvailableItemCount", selector);
        var before = count.Text;

        selector.AvailableItems.Add(new DualListSelectorItem { Identifier = "a", Name = "A" });
        selector.AvailableItems.Add(new DualListSelectorItem { Identifier = "b", Name = "B" });

        Assert.NotEqual(before, count.Text, StringComparer.Ordinal);
        Assert.Contains("2", count.Text, StringComparison.Ordinal);
    }

    [StaFact]
    public void SelectedItemsRebound_UpdatesTheCount()
    {
        var selector = CreateTemplatedSelector();
        var count = (TextBlock)selector.Template.FindName("PART_SelectedItemCount", selector);

        selector.SelectedItems =
        [
            new DualListSelectorItem { Identifier = "a", Name = "A" },
            new DualListSelectorItem { Identifier = "b", Name = "B" },
            new DualListSelectorItem { Identifier = "c", Name = "C" },
        ];

        Assert.Contains("3", count.Text, StringComparison.Ordinal);

        // Changes to the new collection are followed too; the old one is no longer listened to.
        selector.SelectedItems.Add(new DualListSelectorItem { Identifier = "d", Name = "D" });

        Assert.Contains("4", count.Text, StringComparison.Ordinal);
    }

    private static DualListSelector CreateTemplatedSelector()
    {
        var selector = new DualListSelector
        {
            Template = (ControlTemplate)System.Windows.Markup.XamlReader.Parse(TemplateXaml),
        };
        selector.ApplyTemplate();
        return selector;
    }
}