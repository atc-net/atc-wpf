namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes <see cref="TreeSelectComboBox"/> to UI Automation as a combo box with the expand/collapse pattern and a
/// read-only value (the summary text). While the pop-up is open, its check tree is a child of this element; in chips
/// mode the chip remove buttons and the "+N" button are children too.
/// </summary>
public sealed class TreeSelectComboBoxAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeSelectComboBoxAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The control.</param>
    public TreeSelectComboBoxAutomationPeer(TreeSelectComboBox owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public ExpandCollapseState ExpandCollapseState
        => OwnerControl.IsDropDownOpen
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    /// <inheritdoc />
    public string Value => OwnerControl.SummaryText;

    private TreeSelectComboBox OwnerControl => (TreeSelectComboBox)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface is PatternInterface.ExpandCollapse or PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Expand()
    {
        EnsureEnabled();
        OwnerControl.SetCurrentValue(TreeSelectComboBox.IsDropDownOpenProperty, true);
    }

    /// <inheritdoc />
    public void Collapse()
    {
        EnsureEnabled();
        OwnerControl.SetCurrentValue(TreeSelectComboBox.IsDropDownOpenProperty, false);
    }

    /// <inheritdoc />
    public void SetValue(string value)
        => throw new InvalidOperationException("The summary text is read-only; tick the nodes in the pop-up instead.");

    /// <summary>
    /// Raises the expand/collapse state change and refreshes the children (the pop-up tree).
    /// </summary>
    internal void RaiseExpandCollapseStateChanged(
        bool oldValue,
        bool newValue)
    {
        ResetChildrenCache();
        RaisePropertyChangedEvent(
            ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
            oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
            newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
    }

    /// <summary>
    /// Raises the value change for a new summary text.
    /// </summary>
    internal void RaiseValueChanged(
        string oldValue,
        string newValue)
        => RaisePropertyChangedEvent(
            ValuePatternIdentifiers.ValueProperty,
            oldValue,
            newValue);

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(TreeSelectComboBox);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.ComboBox;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? TextBoxHelper.GetWatermark(OwnerControl) ?? string.Empty
            : name;
    }

    /// <inheritdoc />
    protected override List<AutomationPeer>? GetChildrenCore()
    {
        var children = OwnerControl.GetChipAutomationPeers();
        if (OwnerControl.IsDropDownOpen)
        {
            children.Add(CreatePeerForElement(OwnerControl.PartTree));
        }

        return children.Count == 0 ? null : children;
    }

    private void EnsureEnabled()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }
    }
}