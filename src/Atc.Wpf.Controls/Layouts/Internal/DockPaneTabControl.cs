namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// The tab strip of a <see cref="DockRegion"/> with panes; each docked <see cref="DockPane"/> gets a
/// <see cref="DockPaneTabItem"/> with a close button, a context menu, double-click and drag-out to float.
/// </summary>
internal sealed class DockPaneTabControl : TabControl
{
    protected override DependencyObject GetContainerForItemOverride()
        => new DockPaneTabItem();

    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is DockPaneTabItem;

    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is DockPaneTabItem tab && item is DockPane pane)
        {
            tab.Attach(pane);
        }
    }

    protected override void ClearContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        if (element is DockPaneTabItem tab)
        {
            tab.Detach();
        }

        base.ClearContainerForItemOverride(element, item);
    }
}