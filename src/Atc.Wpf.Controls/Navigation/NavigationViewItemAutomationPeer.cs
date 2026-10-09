namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Exposes a <see cref="NavigationViewItem"/> to UI Automation as a selectable, invokable list item.
/// </summary>
public class NavigationViewItemAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider, ISelectionItemProvider
{
    /// <summary>Initializes a new instance of the <see cref="NavigationViewItemAutomationPeer"/> class.</summary>
    public NavigationViewItemAutomationPeer(NavigationViewItem owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public bool IsSelected
        => ((NavigationViewItem)Owner).IsSelected;

    /// <inheritdoc />
    public IRawElementProviderSimple? SelectionContainer
        => FindNavigationView(Owner) is { } view &&
           CreatePeerForElement(view) is { } peer
            ? ProviderFromPeer(peer)
            : null;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface is PatternInterface.Invoke or PatternInterface.SelectionItem
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Invoke()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        ((NavigationViewItem)Owner).AutomationInvoke();
    }

    /// <inheritdoc />
    public void Select()
        => Invoke();

    /// <inheritdoc />
    public void AddToSelection()
        => throw new InvalidOperationException("A NavigationView has a single selected item.");

    /// <inheritdoc />
    public void RemoveFromSelection()
        => throw new InvalidOperationException("A NavigationView has a single selected item.");

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(NavigationViewItem);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.ListItem;

    private static NavigationView? FindNavigationView(DependencyObject item)
    {
        var current = VisualTreeHelper.GetParent(item);
        while (current is not null and not NavigationView)
        {
            current = VisualTreeHelper.GetParent(current);
        }

        return current as NavigationView;
    }
}