namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class ChipTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void New_properties_default_to_the_previous_behaviour()
    {
        var sut = new Chip();

        Assert.Null(sut.ShowCheckMark);
        Assert.Null(sut.SelectedBorderBrush);
        Assert.False(sut.IsSelectable);
        Assert.False(sut.IsSelected);
    }

    [StaFact]
    public void A_selectable_chip_is_a_check_box_with_toggle_in_UI_Automation()
    {
        var sut = new Chip { Content = "Error", IsSelectable = true };
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.CheckBox, peer.GetAutomationControlType());
        Assert.Equal("Error", peer.GetName());
        var toggle = Assert.IsAssignableFrom<IToggleProvider>(peer.GetPattern(PatternInterface.Toggle));
        Assert.Equal(System.Windows.Automation.ToggleState.Off, toggle.ToggleState);

        toggle.Toggle();

        Assert.True(sut.IsSelected);
        Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
    }

    [StaFact]
    public void A_plain_chip_has_no_toggle_pattern()
    {
        var sut = new Chip { Content = "Tag" };
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Null(peer.GetPattern(PatternInterface.Toggle));
        Assert.Null(peer.GetPattern(PatternInterface.SelectionItem));
    }

    [StaFact]
    public void A_toggle_hook_decides_instead_of_the_chip()
    {
        var requested = 0;
        var sut = new Chip
        {
            IsSelectable = true,
            ToggleRequested = _ => requested++,
        };

        ((IToggleProvider)UIElementAutomationPeer.CreatePeerForElement(sut).GetPattern(PatternInterface.Toggle)).Toggle();
        sut.PerformClick();

        Assert.Equal(2, requested);
        Assert.False(sut.IsSelected);
    }

    [StaFact]
    public void A_radio_item_is_a_radio_button_with_selection_item_in_UI_Automation()
    {
        var sut = new Chip { Content = "Open", IsRadioItem = true };
        sut.ToggleRequested = chip => chip.IsSelected = !chip.IsSelected;
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.RadioButton, peer.GetAutomationControlType());
        Assert.Null(peer.GetPattern(PatternInterface.Toggle));
        var item = Assert.IsAssignableFrom<ISelectionItemProvider>(peer.GetPattern(PatternInterface.SelectionItem));

        item.Select();

        Assert.True(sut.IsSelected);
        Assert.True(item.IsSelected);

        item.Select();

        Assert.True(sut.IsSelected);
    }

    [StaFact]
    public void Clicking_a_selectable_chip_without_a_hook_still_toggles_it()
    {
        var sut = new Chip { IsSelectable = true };
        var clicks = 0;
        sut.Click += (_, _) => clicks++;

        sut.PerformClick();

        Assert.True(sut.IsSelected);
        Assert.Equal(1, clicks);
    }
}