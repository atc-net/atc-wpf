namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// Exposes the column filter button as an Invoke button named "Filter {column}". While its pop-up is open, the
/// pop-up content (mode buttons, value list, Clear filter, OK, Cancel) is a child of the button, because the pop-up
/// lives in its own window and is not otherwise below the grid in the UI Automation tree.
/// </summary>
internal sealed class DataGridFilterButtonAutomationPeer : ButtonAutomationPeer
{
    public DataGridFilterButtonAutomationPeer(DataGridFilterButton owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "DataGridFilterButton";

    /// <inheritdoc />
    protected override List<AutomationPeer>? GetChildrenCore()
    {
        var content = ((DataGridFilterButton)Owner).OpenPopupContent;
        if (content is null)
        {
            return base.GetChildrenCore();
        }

        var children = base.GetChildrenCore() ?? [];
        children.Add(CreatePeerForElement(content));
        return children;
    }
}