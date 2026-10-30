namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes <see cref="MultiColumnComboBox"/> to UI Automation as a combo box with the expand/collapse pattern and a
/// read-only value (the summary text). While the pop-up is open, its filter box, its grid (a data grid whose rows hold
/// a check box named after the row's display value) and, in deferred mode, OK and Cancel are children of this element;
/// in chips mode the chip remove buttons and the "+N" button are children too.
/// </summary>
public sealed class MultiColumnComboBoxAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiColumnComboBoxAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The control.</param>
    public MultiColumnComboBoxAutomationPeer(MultiColumnComboBox owner)
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

    private MultiColumnComboBox OwnerControl => (MultiColumnComboBox)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface is PatternInterface.ExpandCollapse or PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Expand()
    {
        EnsureEnabled();
        OwnerControl.SetCurrentValue(MultiColumnComboBox.IsDropDownOpenProperty, true);
    }

    /// <inheritdoc />
    public void Collapse()
    {
        EnsureEnabled();
        OwnerControl.SetCurrentValue(MultiColumnComboBox.IsDropDownOpenProperty, false);
    }

    /// <inheritdoc />
    public void SetValue(string value)
        => throw new InvalidOperationException("The summary text is read-only; tick the rows in the pop-up instead.");

    /// <summary>
    /// Raises the expand/collapse state change and refreshes the children (the pop-up rows).
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
        => nameof(MultiColumnComboBox);

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
        // Closed: only the chips' remove buttons and the "+N" button (chips mode). Open: also the filter box, the grid
        // and, in deferred mode, the OK and Cancel buttons.
        var children = OwnerControl.GetChipAutomationPeers();
        if (OwnerControl.IsDropDownOpen)
        {
            if (OwnerControl.IsFilterShown)
            {
                children.Add(CreatePeerForElement(OwnerControl.PartFilter));
            }

            children.Add(CreatePeerForElement(OwnerControl.PartGrid));
            if (OwnerControl.PartCommitFooter.Visibility == Visibility.Visible)
            {
                children.Add(CreatePeerForElement(OwnerControl.PartOkButton));
                children.Add(CreatePeerForElement(OwnerControl.PartCancelButton));
            }
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