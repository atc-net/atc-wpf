namespace Atc.Wpf.Components.Monitoring;

public partial class ApplicationMonitorView
{
    private const double TailDetectionThresholdPixels = 2d;

    private readonly ContextMenu? defaultContextMenu;
    private bool isAtTail = true;

    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showAutoScrollInToolbar;

    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnAutoScrollChanged))]
    private bool autoScroll;

    [DependencyProperty(DefaultValue = true)]
    private bool showSearchInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showPauseInToolbar;

    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnIsPausedChanged))]
    private bool isPaused;

    /// <summary>
    /// <c>true</c> while the user has manually scrolled away from the tail of
    /// the list. Auto-scroll is suppressed while this is <c>true</c>; the
    /// "Jump to live" overlay appears (bound to this DP).
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isDetachedFromTail;

    /// <summary>
    /// Count of new entries received since the user detached from the tail.
    /// Reset to <c>0</c> when the user scrolls back to the tail or clicks
    /// <em>Jump to live</em>.
    /// </summary>
    [DependencyProperty(DefaultValue = 0)]
    private int newSinceDetached;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnEnableContextMenuChanged))]
    private bool enableContextMenu;

    [DependencyProperty(DefaultValue = 150d)]
    private double areaColumnWidth;

    [DependencyProperty(DefaultValue = 400d)]
    private double messageColumnWidth;

    /// <summary>
    /// Maximum number of entries kept in the buffer. When exceeded, the oldest
    /// entries are dropped (ring-buffer semantics). Set to <c>0</c> for no cap
    /// (unbounded — only safe for short-running apps). Default <c>10000</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 10000,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnMaxEntriesChanged))]
    private int maxEntries;

    public ApplicationMonitorView()
    {
        InitializeComponent();

        defaultContextMenu = LvEntries.ContextMenu;
        ApplyContextMenuState();

        Loaded += OnApplicationMonitorViewLoaded;
        Unloaded += OnApplicationMonitorViewUnloaded;
        DataContextChanged += OnApplicationMonitorViewDataContextChanged;
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

    private static void OnAutoScrollChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ApplicationMonitorView view &&
            view.DataContext is ApplicationMonitorViewModel vm &&
            vm.AutoScroll != (bool)e.NewValue)
        {
            vm.AutoScroll = (bool)e.NewValue;
        }
    }

    private static void OnMaxEntriesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ApplicationMonitorView view &&
            view.DataContext is ApplicationMonitorViewModel vm &&
            vm.MaxEntries != (int)e.NewValue)
        {
            vm.MaxEntries = (int)e.NewValue;
        }
    }

    private static void OnIsPausedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ApplicationMonitorView view &&
            view.DataContext is ApplicationMonitorViewModel vm &&
            vm.IsPaused != (bool)e.NewValue)
        {
            vm.IsPaused = (bool)e.NewValue;
        }
    }

    private void OnApplicationMonitorViewDataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ApplicationMonitorViewModel oldVm)
        {
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is ApplicationMonitorViewModel newVm)
        {
            newVm.PropertyChanged += OnViewModelPropertyChanged;
            newVm.AutoScroll = AutoScroll;
            newVm.MaxEntries = MaxEntries;
            newVm.IsPaused = IsPaused;
        }
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (sender is not ApplicationMonitorViewModel vm)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(ApplicationMonitorViewModel.AutoScroll)
                when AutoScroll != vm.AutoScroll:
                AutoScroll = vm.AutoScroll;
                break;

            case nameof(ApplicationMonitorViewModel.MaxEntries)
                when MaxEntries != vm.MaxEntries:
                MaxEntries = vm.MaxEntries;
                break;

            case nameof(ApplicationMonitorViewModel.IsPaused)
                when IsPaused != vm.IsPaused:
                IsPaused = vm.IsPaused;
                break;
        }
    }

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

            if (!isAtTail)
            {
                NewSinceDetached++;
                return;
            }

            LvEntries.ScrollIntoView(
                obj.Direction == ListSortDirection.Ascending
                    ? LvEntries.Items[^1]!
                    : LvEntries.Items[0]!);
        });
    }

    /// <summary>
    /// Tracks whether the user is "at the tail" of the list. When they scroll
    /// away from the tail we suppress further auto-scroll and surface the
    /// <em>Jump to live</em> overlay; when they return to the tail we resume
    /// auto-scroll and clear the new-entry counter.
    /// </summary>
    private void OnListViewScrollChanged(
        object sender,
        ScrollChangedEventArgs e)
    {
        if (e.OriginalSource is not ScrollViewer sv)
        {
            return;
        }

        var direction = (DataContext as ApplicationMonitorViewModel)?.SortDirection
                        ?? ListSortDirection.Ascending;

        // For ascending sort the tail is the bottom (max VerticalOffset).
        // For descending sort the tail is the top (VerticalOffset == 0).
        var atTail = direction == ListSortDirection.Ascending
            ? sv.VerticalOffset >= sv.ScrollableHeight - TailDetectionThresholdPixels
            : sv.VerticalOffset <= TailDetectionThresholdPixels;

        var wasAtTail = isAtTail;
        isAtTail = atTail;

        if (atTail && !wasAtTail)
        {
            IsDetachedFromTail = false;
            NewSinceDetached = 0;
        }
        else if (!atTail && wasAtTail)
        {
            IsDetachedFromTail = true;
        }
    }

    private void OnJumpToLiveClick(
        object sender,
        RoutedEventArgs e)
        => JumpToLive();

    /// <summary>
    /// Programmatically scrolls to the tail of the list (bottom for
    /// ascending sort, top for descending) and resets the detached state.
    /// Safe to call when there are no entries.
    /// </summary>
    public void JumpToLive()
    {
        if (LvEntries.Items.Count <= 0)
        {
            return;
        }

        var direction = (DataContext as ApplicationMonitorViewModel)?.SortDirection
                        ?? ListSortDirection.Ascending;

        LvEntries.ScrollIntoView(
            direction == ListSortDirection.Ascending
                ? LvEntries.Items[^1]!
                : LvEntries.Items[0]!);

        IsDetachedFromTail = false;
        NewSinceDetached = 0;
    }
}