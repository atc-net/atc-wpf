namespace Atc.Wpf.Controls.Tests.Selectors;

[Collection(nameof(PackResourceTestGroup))]
public sealed class CardSelectorTemplateTests : IDisposable
{
    public CardSelectorTemplateTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Each_card_is_a_CardSelectorItem_with_the_card_size_and_a_Card_frame()
    {
        var (sut, items) = Realize(x =>
        {
            x.CardWidth = 120;
            x.CardHeight = 90;
        });

        var container = Container(sut, items[0]);

        Assert.Equal(120d, container.Width);
        Assert.Equal(90d, container.Height);
        Assert.NotNull(FindDescendant<Card>(container));
        Assert.Equal(4, sut.ItemContainerGenerator.Items.Count);
    }

    [StaFact]
    public void Disabled_cards_and_cards_beyond_the_limit_are_disabled()
    {
        var (sut, items) = Realize(x =>
        {
            x.IsEnabledMemberPath = nameof(CardSelectorTests.Plan.IsAvailable);
            x.MaxSelectionCount = 1;
        });
        items[3].IsAvailable = false;
        sut.RefreshItemStates();

        Assert.False(Container(sut, items[3]).IsEnabled);
        Assert.True(Container(sut, items[1]).IsEnabled);

        sut.RequestToggle(items[0]);

        Assert.True(Container(sut, items[0]).IsEnabled);
        Assert.False(Container(sut, items[1]).IsEnabled);
        Assert.True(Container(sut, items[0]).IsSelected);
    }

    [StaFact]
    public void Multiple_cards_are_check_boxes_with_the_toggle_pattern()
    {
        var (sut, items) = Realize(_ => { });
        var peer = ItemPeer(sut, items[1]);

        Assert.Equal(AutomationControlType.CheckBox, peer.GetAutomationControlType());
        Assert.Null(peer.GetPattern(PatternInterface.SelectionItem));
        var toggle = (IToggleProvider)peer.GetPattern(PatternInterface.Toggle);
        Assert.Equal(System.Windows.Automation.ToggleState.Off, toggle.ToggleState);

        toggle.Toggle();

        Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Equal(AutomationControlType.List, UIElementAutomationPeer.CreatePeerForElement(sut).GetAutomationControlType());
    }

    [StaFact]
    public void Single_cards_are_radio_buttons_with_the_selection_item_pattern()
    {
        var (sut, items) = Realize(x => x.SelectionMode = CardSelectionMode.Single);
        var peer = ItemPeer(sut, items[2]);

        Assert.Equal(AutomationControlType.RadioButton, peer.GetAutomationControlType());
        Assert.Null(peer.GetPattern(PatternInterface.Toggle));
        var selectionItem = (ISelectionItemProvider)peer.GetPattern(PatternInterface.SelectionItem);

        selectionItem.Select();

        Assert.True(selectionItem.IsSelected);
        Assert.Equal(["pro"], sut.SelectedKeys);
        var selection = (ISelectionProvider)UIElementAutomationPeer.CreatePeerForElement(sut).GetPattern(PatternInterface.Selection);
        Assert.False(selection.CanSelectMultiple);
        Assert.True(selection.IsSelectionRequired);
        Assert.Single(selection.GetSelection());
    }

    [StaFact]
    public void Card_name_comes_from_the_template_else_from_the_key()
    {
        var template = (DataTemplate)XamlReader.Parse(
            """
            <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
                <TextBlock AutomationProperties.Name="{Binding Title}" Text="{Binding Title}" />
            </DataTemplate>
            """);
        var (sut, items) = Realize(x => x.ItemTemplate = template);
        Assert.Equal("Standard", ItemPeer(sut, items[1]).GetName());

        var (plain, plainItems) = Realize(_ => { });
        Assert.Equal("standard", ItemPeer(plain, plainItems[1]).GetName());
    }

    [StaFact]
    public void Arrow_keys_move_in_two_dimensions_and_skip_disabled_cards()
    {
        // 3 cards of 100 + 2 gaps of 10 fit on a row of 330: basic, standard, pro / enterprise, extra.
        var (sut, items) = Realize(
            x =>
            {
                x.CardWidth = 100;
                x.ItemSpacing = 10;
                x.IsEnabledMemberPath = nameof(CardSelectorTests.Plan.IsAvailable);
            },
            width: 330,
            extra: true);
        items[1].IsAvailable = false;
        sut.RefreshItemStates();

        Assert.Equal(110d, Container(sut, items[1]).TranslatePoint(default, sut).X - Container(sut, items[0]).TranslatePoint(default, sut).X);
        Assert.Same(Container(sut, items[2]), sut.GetNeighbour(Container(sut, items[0]), Key.Right));
        Assert.Same(Container(sut, items[3]), sut.GetNeighbour(Container(sut, items[0]), Key.Down));
        Assert.Same(Container(sut, items[4]), sut.GetNeighbour(Container(sut, items[2]), Key.Down));
        Assert.Same(Container(sut, items[0]), sut.GetNeighbour(Container(sut, items[3]), Key.Up));
        Assert.Same(Container(sut, items[0]), sut.GetNeighbour(Container(sut, items[2]), Key.Left));
        Assert.Null(sut.GetNeighbour(Container(sut, items[0]), Key.Up));
    }

    [StaFact]
    public void Space_toggles_the_focused_card_and_arrows_select_in_Single()
    {
        var (sut, items) = Realize(_ => { });
        Assert.True(sut.HandleKey(Container(sut, items[1]), Key.Space, ModifierKeys.None));
        Assert.Equal(["standard"], sut.SelectedKeys);

        var (single, singleItems) = Realize(x => x.SelectionMode = CardSelectionMode.Single);
        Assert.True(single.HandleKey(Container(single, singleItems[0]), Key.Right, ModifierKeys.None));
        Assert.Equal(["standard"], single.SelectedKeys);
    }

    [StaFact]
    public void Selected_cards_show_a_check_mark_or_a_radio_dot()
    {
        var (sut, items) = Realize(_ => { });
        sut.RequestToggle(items[0]);
        sut.UpdateLayout();
        var selected = Container(sut, items[0]);

        Assert.Equal(Visibility.Visible, Part(selected, "PART_Indicator").Visibility);
        Assert.Equal(Visibility.Visible, Part(selected, "PART_CheckMark").Visibility);
        Assert.Equal(Visibility.Collapsed, Part(selected, "PART_RadioDot").Visibility);
        Assert.Equal(Visibility.Collapsed, Part(Container(sut, items[1]), "PART_Indicator").Visibility);

        sut.ShowCheckMark = false;
        sut.UpdateLayout();
        Assert.Equal(Visibility.Collapsed, Part(selected, "PART_Indicator").Visibility);

        var (single, singleItems) = Realize(x => x.SelectionMode = CardSelectionMode.Single);
        single.RequestToggle(singleItems[1]);
        single.UpdateLayout();
        var radio = Container(single, singleItems[1]);

        Assert.Equal(Visibility.Visible, Part(radio, "PART_Indicator").Visibility);
        Assert.Equal(Visibility.Collapsed, Part(radio, "PART_CheckMark").Visibility);
        Assert.Equal(Visibility.Visible, Part(radio, "PART_RadioDot").Visibility);
    }

    private static FrameworkElement Part(
        CardSelectorItem container,
        string name)
        => (FrameworkElement)container.Template.FindName(name, container);

    private static (CardSelector Sut, System.Collections.ObjectModel.ObservableCollection<CardSelectorTests.Plan> Items) Realize(
        Action<CardSelector> configure,
        double width = 900,
        bool extra = false)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Selectors/CardSelector.xaml", UriKind.Absolute),
        };

        var items = CardSelectorTests.Plans();
        if (extra)
        {
            items.Add(new CardSelectorTests.Plan("extra", "Extra"));
        }

        var sut = new CardSelector
        {
            Style = (Style)dictionary["AtcApps.Styles.CardSelector"],
            KeyMemberPath = nameof(CardSelectorTests.Plan.Id),
            ItemsSource = items,
        };
        configure(sut);
        sut.ApplyTemplate();
        sut.Measure(new Size(width, 600));
        sut.Arrange(new Rect(0, 0, width, 600));
        sut.UpdateLayout();
        return (sut, items);
    }

    private static CardSelectorItem Container(
        CardSelector sut,
        object item)
        => Assert.IsType<CardSelectorItem>(sut.ItemContainerGenerator.ContainerFromItem(item));

    private static AutomationPeer ItemPeer(
        CardSelector sut,
        object item)
    {
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);
        return peer.GetChildren().OfType<ItemAutomationPeer>().Single(x => ReferenceEquals(x.Item, item));
    }

    private static T? FindDescendant<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                return match;
            }

            var nested = FindDescendant<T>(child);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }
}