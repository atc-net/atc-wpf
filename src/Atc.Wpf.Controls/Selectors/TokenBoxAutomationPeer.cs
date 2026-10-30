namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes <see cref="TokenBox"/> to UI Automation like an editable combo box: the value is the typed text (setting a
/// value with a separator or a new line is handled as a paste), and the expand/collapse pattern opens or closes the
/// suggestions. Its children are the token list (a list whose items are the tokens, each with a "Remove {name}"
/// button), the edit control the user types in, the Clear button (when shown) and, while open, the suggestion list.
/// </summary>
public sealed class TokenBoxAutomationPeer : FrameworkElementAutomationPeer, IValueProvider, IExpandCollapseProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBoxAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The control.</param>
    public TokenBoxAutomationPeer(TokenBox owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public ExpandCollapseState ExpandCollapseState
        => OwnerControl.IsDropDownOpen
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

    /// <inheritdoc />
    public bool IsReadOnly => !OwnerControl.IsEnabled || OwnerControl.IsAtLimit;

    /// <inheritdoc />
    public string Value => OwnerControl.Text ?? string.Empty;

    private TokenBox OwnerControl => (TokenBox)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface is PatternInterface.ExpandCollapse or PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Expand()
    {
        EnsureEnabled();
        OwnerControl.UpdateSuggestions(force: true);
    }

    /// <inheritdoc />
    public void Collapse()
    {
        EnsureEnabled();
        OwnerControl.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    /// <inheritdoc />
    public void SetValue(string value)
    {
        EnsureEnabled();
        if (IsReadOnly)
        {
            throw new InvalidOperationException("The token box is at its selection limit.");
        }

        OwnerControl.SetTextFromAutomation(value);
    }

    /// <summary>
    /// Raises the expand/collapse state change and refreshes the children (the suggestion list).
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
    /// Raises the value change for new typed text.
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
        => nameof(TokenBox);

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
    protected override bool IsKeyboardFocusableCore()
        => OwnerControl.IsEnabled;

    /// <inheritdoc />
    protected override bool HasKeyboardFocusCore()
        => OwnerControl.IsKeyboardFocusWithin;

    /// <inheritdoc />
    protected override void SetFocusCore()
    {
        // The caret belongs in the text box; focusing the field moves it there.
        if (!OwnerControl.PartTextBox.Focus())
        {
            throw new InvalidOperationException("The token box cannot take the keyboard focus.");
        }
    }

    /// <inheritdoc />
    protected override List<AutomationPeer>? GetChildrenCore()
    {
        var children = new List<AutomationPeer>();
        AddPeer(children, OwnerControl.ChipPanel);
        AddPeer(children, OwnerControl.PartTextBox);
        if (OwnerControl.PartClearButton.Visibility == Visibility.Visible)
        {
            AddPeer(children, OwnerControl.PartClearButton);
        }

        if (OwnerControl.IsDropDownOpen)
        {
            AddPeer(children, OwnerControl.PartSuggestionList);
        }

        return children;
    }

    private static void AddPeer(
        List<AutomationPeer> peers,
        UIElement element)
    {
        if (CreatePeerForElement(element) is { } peer)
        {
            peers.Add(peer);
        }
    }

    private void EnsureEnabled()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }
    }
}