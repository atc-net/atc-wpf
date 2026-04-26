namespace Atc.Wpf.Components.Monitoring;

public partial class ApplicationMonitorView
{
    private readonly ContextMenu? defaultContextMenu;

    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showAutoScrollInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showSearchInToolbar;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnEnableContextMenuChanged))]
    private bool enableContextMenu;

    public ApplicationMonitorView()
    {
        InitializeComponent();

        defaultContextMenu = LvEntries.ContextMenu;
        ApplyContextMenuState();

        Loaded += OnApplicationMonitorViewLoaded;
        Unloaded += OnApplicationMonitorViewUnloaded;
    }

    private void OnApplicationMonitorViewLoaded(
        object sender,
        RoutedEventArgs e)
        => Messenger.Default.Register<ApplicationMonitorScrollEvent>(this, OnApplicationMonitorScrollEvent);

    private void OnApplicationMonitorViewUnloaded(
        object sender,
        RoutedEventArgs e)
        => Messenger.Default.UnRegister<ApplicationMonitorScrollEvent>(this, OnApplicationMonitorScrollEvent);

    private static void OnEnableContextMenuChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ApplicationMonitorView)d).ApplyContextMenuState();

    private void ApplyContextMenuState()
        => LvEntries.ContextMenu = EnableContextMenu
            ? defaultContextMenu
            : null;

    /// <summary>
    /// Pushes the ListView's multi-selection into the ViewModel so its copy
    /// commands' CanExecute can react. WPF's ListView.SelectedItems is not
    /// directly bindable to a collection property, so this thin shim is the
    /// pragmatic bridge.
    /// </summary>
    private void OnEntriesSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DataContext is ApplicationMonitorViewModel viewModel)
        {
            viewModel.SetSelectedEntries(LvEntries.SelectedItems.OfType<ApplicationEventEntry>());
        }
    }

    private void OnApplicationMonitorScrollEvent(
        ApplicationMonitorScrollEvent obj)
    {
        _ = Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
        {
            if (LvEntries.Items.Count <= 0)
            {
                return;
            }

            LvEntries.ScrollIntoView(
                obj.Direction == ListSortDirection.Ascending
                    ? LvEntries.Items[^1]!
                    : LvEntries.Items[0]!);
        });
    }
}