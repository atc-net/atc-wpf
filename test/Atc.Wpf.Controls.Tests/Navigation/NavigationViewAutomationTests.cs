namespace Atc.Wpf.Controls.Tests.Navigation;

public sealed class NavigationViewAutomationTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void ItemPeer_Invoke_SelectsTheItem()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var sut = new NavigationView();
        sut.MenuItems.Add(home);
        var peer = UIElementAutomationPeer.CreatePeerForElement(home);

        ((IInvokeProvider)peer.GetPattern(PatternInterface.Invoke)).Invoke();

        Assert.Same(home, sut.SelectedItem);
    }

    [StaFact]
    public void ItemPeer_SelectionItem_ReportsAndSelects()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var sut = new NavigationView();
        sut.MenuItems.Add(home);
        var selectionItem = (ISelectionItemProvider)UIElementAutomationPeer
            .CreatePeerForElement(home)
            .GetPattern(PatternInterface.SelectionItem);
        var before = selectionItem.IsSelected;

        selectionItem.Select();

        Assert.False(before);
        Assert.True(selectionItem.IsSelected);
    }

    [StaFact]
    public void ItemPeer_HasTheContentAsNameAndListItemControlType()
    {
        var home = new NavigationViewItem { Content = "Home" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(home);

        Assert.Equal("Home", peer.GetName());
        Assert.Equal(AutomationControlType.ListItem, peer.GetAutomationControlType());
        Assert.Equal(nameof(NavigationViewItem), peer.GetClassName());
    }

    [StaFact]
    public void ViewPeer_IsAPane()
    {
        var sut = new NavigationView();

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.Pane, peer.GetAutomationControlType());
        Assert.Equal(nameof(NavigationView), peer.GetClassName());
    }
}