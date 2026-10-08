namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Exposes a <see cref="NavigationViewItem"/> to UI Automation as a selectable, invokable list item.
/// </summary>
public class NavigationViewItemAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider, ISelectionItemProvider
{
    public NavigationViewItemAutomationPeer(NavigationViewItem owner)
        : base(owner)
    {
    }

    public bool IsSelected
        => ((NavigationViewItem)Owner).IsSelected;

    public IRawElementProviderSimple? SelectionContainer
        => FindNavigationView(Owner) is { } view &&
           CreatePeerForElement(view) is { } peer
            ? ProviderFromPeer(peer)
            : null;

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface is PatternInterface.Invoke or PatternInterface.SelectionItem
            ? this
            : base.GetPattern(patternInterface);

    public void Invoke()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        ((NavigationViewItem)Owner).AutomationInvoke();
    }

    public void Select()
        => Invoke();

    public void AddToSelection()
        => throw new InvalidOperationException("A NavigationView has a single selected item.");

    public void RemoveFromSelection()
        => throw new InvalidOperationException("A NavigationView has a single selected item.");

    protected override string GetClassNameCore()
        => nameof(NavigationViewItem);

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