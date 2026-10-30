namespace Atc.Wpf.Components.Tests.Selectors;

/// <summary>
/// Runs the real DualListSelector template (with the theme), so the row wrappers, the "Select all" rows and
/// the virtualizing lists are checked as shipped.
/// </summary>
[Collection(nameof(DualListSelectorThemedTestGroup))]
public sealed class DualListSelectorThemedTests : IDisposable
{
    public DualListSelectorThemedTests()
        => RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void CheckBoxStyle_RowCheckBoxesToggleTheRows_AndTheListsVirtualize()
    {
        var sut = new DualListSelector
        {
            ItemSelectionStyle = DualListItemSelectionStyle.CheckBox,
            ListMaxHeight = 200,
            AvailableItems = DualListSelectorTestHost.CreateItems("A", 5000),
        };

        var window = CreateWindow(sut);
        window.Show();
        try
        {
            window.UpdateLayout();
            var list = (ListBox)sut.Template.FindName("PART_AvailableListBox", sut);
            var selectAll = (CheckBox)sut.Template.FindName("PART_AvailableSelectAll", sut);

            Assert.Equal(SelectionMode.Multiple, list.SelectionMode);
            Assert.Equal(Visibility.Visible, ((FrameworkElement)selectAll.Parent).Visibility);
            Assert.Null(list.ItemContainerGenerator.ContainerFromIndex(4000));

            var firstRow = (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(0);
            var rowCheckBox = FindVisualChild<CheckBox>(firstRow);
            Assert.NotNull(rowCheckBox);
            Assert.Equal(Visibility.Visible, rowCheckBox.Visibility);
            Assert.Equal("A 0", AutomationProperties.GetName(rowCheckBox));

            DualListSelectorTestHost.ToggleViaAutomation(rowCheckBox);

            Assert.True(firstRow.IsSelected);
            Assert.Null(selectAll.IsChecked);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void OneWay_SelectedRowsHaveARemoveButton_ExceptLockedRows_WhichShowALock()
    {
        var sut = new DualListSelector
        {
            IsOneWay = true,
            ListMaxHeight = 200,
            SelectedItems =
            [
                new DualListSelectorItem { Identifier = "id", Name = "Id", IsLocked = true },
                new DualListSelectorItem { Identifier = "name", Name = "Name" },
            ],
        };

        var window = CreateWindow(sut);
        window.Show();
        try
        {
            window.UpdateLayout();
            var list = (ListBox)sut.Template.FindName("PART_SelectedListBox", sut);
            var lockedRow = (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(0);
            var plainRow = (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(1);

            Assert.Equal(Visibility.Collapsed, FindVisualChild<Button>(lockedRow)!.Visibility);
            Assert.Equal(Visibility.Visible, FindVisualChild<System.Windows.Shapes.Path>(lockedRow, "LockIcon")!.Visibility);

            var remove = FindVisualChild<Button>(plainRow)!;
            Assert.Equal(Visibility.Visible, remove.Visibility);
            Assert.Equal(
                string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Remove1, "Name"),
                AutomationProperties.GetName(remove));

            ((IInvokeProvider)new ButtonAutomationPeer(remove).GetPattern(PatternInterface.Invoke)).Invoke();
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

            Assert.Equal(["id"], sut.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
            Assert.Equal(["name"], sut.AvailableItems.Select(x => x.Identifier), StringComparer.Ordinal);

            var moveBack = (FrameworkElement)sut.Template.FindName("MoveToAvailableButton", sut);
            Assert.Equal(Visibility.Collapsed, moveBack.Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window CreateWindow(DualListSelector sut)
    {
        var dualListStyles = LoadDualListSelectorStylesWithTheme();
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(dualListStyles);
        sut.Style = (Style)dualListStyles["AtcApps.Styles.DualListSelector"];

        return new Window
        {
            Resources = resources,
            Content = sut,
            Width = 800,
            Height = 500,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
    }

    /// <summary>
    /// Loads the shipped DualListSelector.xaml with the theme ListBox styles merged in front of it, so its StaticResource
    /// references to them resolve as they do in an application that merges both (a test has no application
    /// resources to fall back to).
    /// </summary>
    private static ResourceDictionary LoadDualListSelectorStylesWithTheme()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Atc.Wpf.slnx")) &&
               !Directory.Exists(Path.Combine(directory.FullName, "src", "Atc.Wpf.Components")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        var xaml = File.ReadAllText(Path.Combine(directory.FullName, "src", "Atc.Wpf.Components", "Selectors", "DualListSelector.xaml"));
        xaml = xaml
            .Replace("clr-namespace:Atc.Wpf.Components.Resources\"", "clr-namespace:Atc.Wpf.Components.Resources;assembly=Atc.Wpf.Components\"", StringComparison.Ordinal)
            .Replace("clr-namespace:Atc.Wpf.Components.Selectors\"", "clr-namespace:Atc.Wpf.Components.Selectors;assembly=Atc.Wpf.Components\"", StringComparison.Ordinal)
            .Replace(
                "<ResourceDictionary.MergedDictionaries>",
                "<ResourceDictionary.MergedDictionaries><ResourceDictionary Source=\"pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/ListBox.xaml\" />",
                StringComparison.Ordinal);

        return (ResourceDictionary)XamlReader.Parse(xaml);
    }

    private static T? FindVisualChild<T>(
        DependencyObject parent,
        string? name = null)
        where T : FrameworkElement
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match && (name is null || string.Equals(match.Name, name, StringComparison.Ordinal)))
            {
                return match;
            }

            var nested = FindVisualChild<T>(child, name);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }
}