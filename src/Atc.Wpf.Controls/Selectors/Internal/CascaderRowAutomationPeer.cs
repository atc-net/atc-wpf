#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The automation peer of a <see cref="CascaderComboBox"/> row: a check box with a three-state toggle, named after the
/// node text. A row with children (loaded or not) also has expand/collapse: expanded means active, its children shown
/// in the next column. The "please wait" row has neither.
/// </summary>
internal sealed class CascaderRowAutomationPeer : ItemAutomationPeer, IToggleProvider, IExpandCollapseProvider
{
    private readonly CascaderColumnListAutomationPeer listPeer;

    public CascaderRowAutomationPeer(
        object item,
        CascaderColumnListAutomationPeer itemsControlAutomationPeer)
        : base(item, itemsControlAutomationPeer)
        => listPeer = itemsControlAutomationPeer;

    /// <inheritdoc />
    public ToggleState ToggleState
        => Node?.IsChecked switch
        {
            true => ToggleState.On,
            false => ToggleState.Off,
            _ => ToggleState.Indeterminate,
        };

    /// <inheritdoc />
    public ExpandCollapseState ExpandCollapseState
        => Node is { } node && View.IsActive(node)
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

    private CheckTreeNode? Node => Item as CheckTreeNode;

    private CascaderColumnList List => (CascaderColumnList)listPeer.Owner;

    private CascaderColumnView View => List.OwnerView;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        if (Node is not { } node || View.IsPlaceholder(node))
        {
            return base.GetPattern(patternInterface);
        }

        return patternInterface switch
        {
            PatternInterface.Toggle => this,
            PatternInterface.ExpandCollapse when !List.IsFilterResults && CascaderColumnView.HasChildren(node) => this,
            _ => base.GetPattern(patternInterface),
        };
    }

    /// <inheritdoc />
    public void Toggle()
    {
        if (Node is not { } node)
        {
            return;
        }

        EnsureEnabled(node, isToggle: true);
        View.ToggleNode(node);
    }

    /// <inheritdoc />
    public void Expand()
    {
        if (Node is not { } node)
        {
            return;
        }

        EnsureEnabled(node);
        View.Activate(node);
    }

    /// <inheritdoc />
    public void Collapse()
    {
        if (Node is not { } node)
        {
            return;
        }

        EnsureEnabled(node);
        View.Deactivate(node);
    }

    /// <summary>
    /// Raises the toggle-state change of the row.
    /// </summary>
    internal void RaiseToggleStateChanged(
        bool? oldValue,
        bool? newValue)
        => RaisePropertyChangedEvent(
            TogglePatternIdentifiers.ToggleStateProperty,
            ToToggleState(oldValue),
            ToToggleState(newValue));

    /// <summary>
    /// Raises the expand/collapse-state change of the row (active means expanded).
    /// </summary>
    internal void RaiseExpandCollapseStateChanged(
        bool wasActive,
        bool isActive)
    {
        if (Node is not { } node || List.IsFilterResults || !CascaderColumnView.HasChildren(node))
        {
            return;
        }

        RaisePropertyChangedEvent(
            ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
            wasActive ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
            isActive ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
    }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => Node is { } node && View.IsPlaceholder(node)
            ? AutomationControlType.Text
            : AutomationControlType.CheckBox;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "CascaderRow";

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? Node?.Text ?? string.Empty
            : name;
    }

    /// <inheritdoc />
    protected override string GetHelpTextCore()
        => List.IsFilterResults && Node is { } node
            ? View.GetFullPath(node)
            : base.GetHelpTextCore();

    private static ToggleState ToToggleState(bool? value)
        => value switch
        {
            true => ToggleState.On,
            false => ToggleState.Off,
            _ => ToggleState.Indeterminate,
        };

    private void EnsureEnabled(
        CheckTreeNode node,
        bool isToggle = false)
    {
        if (!View.IsEnabled || (isToggle && !node.IsEnabled))
        {
            throw new ElementNotEnabledException();
        }
    }
}