namespace Atc.Wpf.Network.Tests.Scanner;

/// <summary>
/// Sorting by clicking a column header of <see cref="NetworkScannerView"/>; the sample app shows every
/// data binding error in a dialog, so loading the view must not log one.
/// </summary>
public sealed class NetworkScannerViewSortTests
{
    [StaFact]
    public void Loading_the_view_logs_no_binding_errors()
    {
        using var trace = BindingTraceCapture.Start();
        using var viewModel = new NetworkScannerViewModel();

        // The sample app creates the view first and puts it in the sample viewer afterwards.
        var view = new NetworkScannerView { DataContext = viewModel };
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        var window = Show(view);
        window.Close();

        Assert.DoesNotContain(trace.Events, x => x.Message.Contains("UcNetworkScanner", StringComparison.Ordinal));
    }

    [StaFact]
    public void Clicking_a_column_header_sorts_by_that_column()
    {
        using var viewModel = new NetworkScannerViewModel();
        var window = Show(new NetworkScannerView { DataContext = viewModel });
        var header = FindHeaders(window).Single(x => Equals(x.Tag, "Hostname"));

        header.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, header));
        window.Close();

        var sort = Assert.Single(CollectionViewSource.GetDefaultView(viewModel.Entries).SortDescriptions);
        Assert.Equal("Hostname", sort.PropertyName);
    }

    private static Window Show(NetworkScannerView view)
    {
        var window = new Window
        {
            Width = 1200,
            Height = 600,
            Left = -3000,
            ShowInTaskbar = false,
            Resources = CreateAppResources(),
            Content = view,
        };
        window.Show();
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        return window;
    }

    private static ResourceDictionary CreateAppResources()
    {
        // The same dictionaries as the sample app's App.xaml; the busy overlay that hosts the list needs them.
        // pack://application URIs need the Application type initialized (not an instance).
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var resources = new ResourceDictionary();
        foreach (var uri in new[]
                 {
                     "pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml",
                     "pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml",
                     "pack://application:,,,/Atc.Wpf.Components;component/Styles/Controls.xaml",
                 })
        {
            resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(uri, UriKind.Absolute) });
        }

        return resources;
    }

    private static List<GridViewColumnHeader> FindHeaders(
        DependencyObject parent)
    {
        var headers = new List<GridViewColumnHeader>();
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is GridViewColumnHeader { Tag: not null } header)
            {
                headers.Add(header);
            }

            headers.AddRange(FindHeaders(child));
        }

        return headers;
    }
}