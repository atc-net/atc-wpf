namespace Atc.Wpf.Controls.Tests.DataDisplay;

[Collection(nameof(PackResourceTestGroup))]
public sealed class BreadcrumbTests
{
    private const string Dictionary = "DataDisplay/Breadcrumb.xaml";

    [StaFact]
    public void OnlyTheLastItem_IsMarkedLast_AsItemsChange()
    {
        var sut = Create("Home", "Products", "Laptops");

        Assert.Equal([false, false, true], sut.Items.Select(x => x.IsLast));

        var last = sut.Items[2];
        sut.Items.Remove(last);

        Assert.Equal([false, true], sut.Items.Select(x => x.IsLast));
    }

    [StaFact]
    public void ClickingAnItem_RaisesItemClickedWithItsIndex_AndRunsItsCommand()
    {
        var sut = Create("Home", "Products", "Laptops");
        var command = new RecordingCommand();
        sut.Items[1].Command = command;
        sut.Items[1].CommandParameter = "p";
        var clicked = new List<BreadcrumbItemClickedEventArgs>();
        sut.ItemClicked += (_, e) => clicked.Add(e);

        Click(sut.Items[1]);

        var args = Assert.Single(clicked);
        Assert.Same(sut.Items[1], args.Item);
        Assert.Equal(1, args.Index);
        Assert.Equal(["p"], command.Parameters);
    }

    [StaFact]
    public void ClickingTheLastItem_DoesNothing()
    {
        var sut = Create("Home", "Laptops");
        var command = new RecordingCommand();
        sut.Items[1].Command = command;
        var clicked = 0;
        sut.ItemClicked += (_, _) => clicked++;

        Click(sut.Items[1]);

        Assert.Equal(0, clicked);
        Assert.Empty(command.Parameters);
    }

    [StaFact]
    public void ARemovedItem_NoLongerRaisesItemClicked()
    {
        var sut = Create("Home", "Products", "Laptops");
        var removed = sut.Items[0];
        sut.Items.Remove(removed);
        var clicked = 0;
        sut.ItemClicked += (_, _) => clicked++;

        Click(removed);

        Assert.Equal(0, clicked);
    }

    // Clear() raises a Reset change that carries no old items.
    [StaFact]
    public void ItemsCleared_TheOldItemsNoLongerRaiseItemClicked()
    {
        var sut = Create("Home", "Products", "Laptops");
        var old = sut.Items[0];
        sut.Items.Clear();
        var clicked = 0;
        sut.ItemClicked += (_, _) => clicked++;

        Click(old);

        Assert.Equal(0, clicked);
    }

    // The item style makes every item a tab stop, so a keyboard user must be able to activate it.
    [StaTheory]
    [InlineData(Key.Enter)]
    [InlineData(Key.Space)]
    public void Keyboard_EnterOrSpaceOnAnItem_ActivatesIt(Key key)
    {
        var sut = Create("Home", "Products", "Laptops");
        var command = new RecordingCommand();
        sut.Items[0].Command = command;
        var clicked = new List<int>();
        sut.ItemClicked += (_, e) => clicked.Add(e.Index);

        KeyPress.Send(sut.Items[0], key);

        Assert.Equal([0], clicked);
        Assert.Single(command.Parameters);
    }

    [StaFact]
    public void Keyboard_EnterOnTheLastItem_DoesNothing()
    {
        var sut = Create("Home", "Laptops");
        var clicked = 0;
        sut.ItemClicked += (_, _) => clicked++;

        KeyPress.Send(sut.Items[1], Key.Enter);

        Assert.Equal(0, clicked);
    }

    [StaFact]
    public void Rendered_ShowsTheItemsWithASeparatorBetweenEach()
    {
        var sut = Create("Home", "Products", "Laptops");
        sut.Separator = ">";

        var window = Show(sut);
        try
        {
            var children = Host(sut).Children.Cast<UIElement>().ToList();

            Assert.Equal(5, children.Count);
            Assert.Same(sut.Items[0], children[0]);
            Assert.Equal(">", Assert.IsType<ContentPresenter>(children[1]).Content);
            Assert.Same(sut.Items[1], children[2]);
            Assert.Same(sut.Items[2], children[4]);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Rendered_ChangingTheSeparator_RebuildsTheTrail()
    {
        var sut = Create("Home", "Laptops");

        var window = Show(sut);
        try
        {
            sut.Separator = "|";

            Assert.Equal("|", Assert.IsType<ContentPresenter>(Host(sut).Children[1]).Content);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Rendered_Overflow_KeepsTheFirstAndLastItems_AndListsTheHiddenOnesInAMenu()
    {
        var sut = Create("Root", "A", "B", "C", "D");
        sut.OverflowMode = BreadcrumbOverflowMode.Collapsed;
        sut.MaxVisibleItems = 2;
        var clicked = new List<int>();
        sut.ItemClicked += (_, e) => clicked.Add(e.Index);

        var window = Show(sut);
        try
        {
            var children = Host(sut).Children.Cast<UIElement>().ToList();

            Assert.Equal(7, children.Count);
            Assert.Same(sut.Items[0], children[0]);
            var overflow = Assert.IsType<Button>(children[2]);
            Assert.Same(sut.Items[3], children[4]);
            Assert.Same(sut.Items[4], children[6]);

            overflow.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            var menu = Assert.IsType<ContextMenu>(overflow.ContextMenu ?? FindOpenMenu(overflow));
            var entries = menu.Items.Cast<MenuItem>().ToList();
            Assert.Equal(["A", "B"], entries.Select(x => x.Header));

            entries[1].RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Assert.Equal([2], clicked);
            menu.IsOpen = false;
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Rendered_FewerItemsThanTheOverflowLimit_ShowsThemAll()
    {
        var sut = Create("Root", "A", "B");
        sut.OverflowMode = BreadcrumbOverflowMode.Collapsed;
        sut.MaxVisibleItems = 2;

        var window = Show(sut);
        try
        {
            Assert.Equal(5, Host(sut).Children.Count);
            Assert.DoesNotContain(Host(sut).Children.Cast<UIElement>(), x => x is Button);
        }
        finally
        {
            window.Close();
        }
    }

    private static Breadcrumb Create(params string[] items)
    {
        var sut = new Breadcrumb();
        foreach (var item in items)
        {
            sut.Items.Add(new BreadcrumbItem { Content = item });
        }

        return sut;
    }

    private static void Click(BreadcrumbItem item)
        => item.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseLeftButtonUpEvent,
        });

    private static StackPanel Host(Breadcrumb sut)
        => ThemedHost.Part<StackPanel>(sut, "PART_ItemsHost");

    private static ContextMenu? FindOpenMenu(Button button)
        => PresentationSource.CurrentSources
            .OfType<PresentationSource>()
            .Select(x => x.RootVisual)
            .OfType<DependencyObject>()
            .SelectMany(Descendants)
            .OfType<ContextMenu>()
            .FirstOrDefault(x => ReferenceEquals(x.PlacementTarget, button));

    private static IEnumerable<DependencyObject> Descendants(
        DependencyObject root)
    {
        yield return root;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            foreach (var child in Descendants(VisualTreeHelper.GetChild(root, i)))
            {
                yield return child;
            }
        }
    }

    private static Window Show(Breadcrumb sut)
        => ThemedHost.Show(
            sut,
            Dictionary,
            (typeof(Breadcrumb), "AtcApps.Styles.Breadcrumb"),
            (typeof(BreadcrumbItem), "AtcApps.Styles.BreadcrumbItem"));

    private sealed class RecordingCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => _ = value;
            remove => _ = value;
        }

        public List<object?> Parameters { get; } = [];

        public bool CanExecute(object? parameter)
            => true;

        public void Execute(object? parameter)
            => Parameters.Add(parameter);
    }
}