namespace Atc.Wpf.Controls.Tests.Selectors;

[Collection(nameof(PackResourceTestGroup))]
public sealed class MultiColumnComboBoxThemedTests : IDisposable
{
    public MultiColumnComboBoxThemedTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void With_the_themed_grid_styles_ticked_rows_get_the_selected_background()
    {
        var resources = CreateThemedGridResources();
        var tickedBrush = new SolidColorBrush(Colors.Orange);
        resources["AtcApps.Brushes.Accent3"] = tickedBrush;

        var sut = new MultiColumnComboBox
        {
            KeyMemberPath = "Number",
            DisplayMemberPath = "Name",
            AutoGenerateColumns = true,
            ItemsSource = new List<Row> { new("1", "One"), new("2", "Two"), new("3", "Three") },
        };
        sut.SelectedKeys.Add("2");

        var window = new Window
        {
            Resources = resources,
            Content = sut,
            Width = 300,
            Height = 100,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            window.UpdateLayout();
            Assert.Same(resources["AtcApps.Styles.DataGridRow"], sut.PartGrid.RowStyle.BasedOn);
            Assert.Same(resources["AtcApps.Styles.DataGridCell"], sut.PartGrid.CellStyle.BasedOn);

            sut.IsDropDownOpen = true;
            sut.PartPopupBorder.UpdateLayout();

            var rows = FindVisualChildren<DataGridRow>(sut.PartGrid).ToList();
            Assert.Equal(3, rows.Count);
            Assert.Same(tickedBrush, rows.Single(x => x.Item is Row { Number: "2" }).Background);
            Assert.NotSame(tickedBrush, rows.Single(x => x.Item is Row { Number: "1" }).Background);

            sut.ToggleItem(rows.Single(x => x.Item is Row { Number: "1" }).Item);
            Assert.Same(tickedBrush, rows.Single(x => x.Item is Row { Number: "1" }).Background);
        }
        finally
        {
            sut.IsDropDownOpen = false;
            window.Close();
        }
    }

    private static ResourceDictionary CreateThemedGridResources()
    {
        var gridStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/DataGrid.xaml", UriKind.Absolute),
        };

        // Only the row and cell styles: the themed column header uses theme-manager types that must not be created on
        // a second test thread.
        return new ResourceDictionary
        {
            ["AtcApps.Styles.DataGridRow"] = gridStyles["AtcApps.Styles.DataGridRow"],
            ["AtcApps.Styles.DataGridCell"] = gridStyles["AtcApps.Styles.DataGridCell"],
        };
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private sealed record Row(string Number, string Name);
}