namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// An application shell with a collapsible navigation pane and a content area.
/// </summary>
/// <remarks>
/// Set <see cref="NavigationService"/> to let the view drive and follow an <see cref="INavigationService"/>:
/// invoking an item with a <see cref="NavigationViewItem.TargetViewModelType"/> navigates to it, the
/// current ViewModel is shown as <see cref="Content"/> (map it to a view with a DataTemplate), navigation
/// from elsewhere selects the matching item, and a navigation blocked by an <see cref="INavigationGuard"/>
/// keeps the previous selection.
/// </remarks>
[ContentProperty(nameof(MenuItems))]
[TemplatePart(Name = "PART_MenuItemsHost", Type = typeof(Panel))]
[TemplatePart(Name = "PART_FooterMenuItemsHost", Type = typeof(Panel))]
[TemplatePart(Name = "PART_BackButton", Type = typeof(ButtonBase))]
public partial class NavigationView : Control
{
    private Panel? menuItemsHost;
    private Panel? footerMenuItemsHost;
    private ButtonBase? backButton;
    private bool isSyncingSelection;
    private bool isRevertingSelection;

    /// <summary>
    /// The content shown next to the pane; with a <see cref="NavigationService"/> this is the current ViewModel.
    /// </summary>
    [DependencyProperty]
    private object? content;

    /// <summary>
    /// The template used to display <see cref="Content"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? contentTemplate;

    /// <summary>
    /// The template selector used to display <see cref="Content"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplateSelector? contentTemplateSelector;

    /// <summary>
    /// The title shown at the top of the pane when it is open.
    /// </summary>
    [DependencyProperty]
    private object? paneTitle;

    /// <summary>
    /// The background of the pane.
    /// </summary>
    [DependencyProperty]
    private Brush? paneBackground;

    /// <summary>
    /// Whether the pane is open (icons and labels) or compact (icons only).
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private bool isPaneOpen;

    /// <summary>
    /// The width of the pane when it is open.
    /// </summary>
    [DependencyProperty(DefaultValue = 240d)]
    private double openPaneLength;

    /// <summary>
    /// The width of the pane when it is compact; also the width of the icon column.
    /// </summary>
    [DependencyProperty(DefaultValue = 48d)]
    private double compactPaneLength;

    /// <summary>
    /// Whether the back button is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isBackButtonVisible;

    /// <summary>
    /// Whether back navigation is possible. Set by the view from <see cref="NavigationService"/>.
    /// </summary>
    [DependencyProperty]
    private bool isBackEnabled;

    /// <summary>
    /// The selected item, from <see cref="MenuItems"/> or <see cref="FooterMenuItems"/>.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedItemChanged))]
    private NavigationViewItem? selectedItem;

    /// <summary>
    /// The navigation service the view drives and follows.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnNavigationServiceChanged))]
    private INavigationService? navigationService;

    static NavigationView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationView),
            new FrameworkPropertyMetadata(typeof(NavigationView)));
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationView"/> class.</summary>
    public NavigationView()
    {
        MenuItems.CollectionChanged += OnItemsCollectionChanged;
        FooterMenuItems.CollectionChanged += OnItemsCollectionChanged;
    }

    /// <summary>
    /// Occurs when <see cref="SelectedItem"/> changes.
    /// </summary>
    public event EventHandler<NavigationViewSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when an item is invoked, also when it does not select or navigate.
    /// </summary>
    public event EventHandler<NavigationViewItemInvokedEventArgs>? ItemInvoked;

    /// <summary>
    /// Gets the items shown at the top of the pane.
    /// </summary>
    public ObservableCollection<NavigationViewItem> MenuItems { get; } = [];

    /// <summary>
    /// Gets the items shown at the bottom of the pane, such as Settings.
    /// </summary>
    public ObservableCollection<NavigationViewItem> FooterMenuItems { get; } = [];

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (backButton is not null)
        {
            backButton.Click -= OnBackButtonClick;
        }

        base.OnApplyTemplate();

        menuItemsHost = GetTemplateChild("PART_MenuItemsHost") as Panel;
        footerMenuItemsHost = GetTemplateChild("PART_FooterMenuItemsHost") as Panel;
        backButton = GetTemplateChild("PART_BackButton") as ButtonBase;

        if (backButton is not null)
        {
            backButton.Click += OnBackButtonClick;
        }

        RebuildItemsHosts();
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new NavigationViewAutomationPeer(this);

    /// <summary>
    /// Navigates back through <see cref="NavigationService"/>.
    /// </summary>
    /// <returns>True if the navigation succeeded; otherwise, false.</returns>
    public bool GoBack()
        => NavigationService is { CanGoBack: true } service &&
           service.GoBack();

    private static void OnSelectedItemChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((NavigationView)d).OnSelectedItemChanged(
            (NavigationViewItem?)e.OldValue,
            (NavigationViewItem?)e.NewValue);

    private static void OnNavigationServiceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var view = (NavigationView)d;

        if (e.OldValue is INavigationService oldService)
        {
            oldService.Navigated -= view.OnNavigated;
        }

        if (e.NewValue is INavigationService newService)
        {
            newService.Navigated += view.OnNavigated;
        }

        view.SyncWithNavigationService();
    }

    private void OnSelectedItemChanged(
        NavigationViewItem? oldItem,
        NavigationViewItem? newItem)
    {
        if (isRevertingSelection)
        {
            return;
        }

        if (!isSyncingSelection &&
            newItem?.TargetViewModelType is { } targetViewModelType &&
            NavigationService is { } service &&
            !service.NavigateTo(targetViewModelType, newItem.NavigationParameters))
        {
            // Blocked, for example by a navigation guard: keep the previous selection as if nothing happened.
            isRevertingSelection = true;
            try
            {
                SetCurrentValue(SelectedItemProperty, oldItem);
            }
            finally
            {
                isRevertingSelection = false;
            }

            return;
        }

        foreach (var item in AllItems())
        {
            item.IsSelected = ReferenceEquals(item, newItem);
        }

        SelectionChanged?.Invoke(
            this,
            new NavigationViewSelectionChangedEventArgs(oldItem, newItem));
    }

    private void OnItemsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (NavigationViewItem item in e.OldItems)
            {
                item.Click -= OnItemClick;
                if (ReferenceEquals(item, SelectedItem))
                {
                    SetSelectedItemWithoutNavigating(null);
                }
            }
        }

        if (e.NewItems is not null)
        {
            foreach (NavigationViewItem item in e.NewItems)
            {
                item.Click -= OnItemClick;
                item.Click += OnItemClick;
            }
        }

        RebuildItemsHosts();
    }

    private void OnItemClick(
        object sender,
        RoutedEventArgs e)
    {
        var item = (NavigationViewItem)sender;

        ItemInvoked?.Invoke(
            this,
            new NavigationViewItemInvokedEventArgs(item));

        if (item.SelectsOnInvoked)
        {
            SetCurrentValue(SelectedItemProperty, item);
        }
    }

    private void OnBackButtonClick(
        object sender,
        RoutedEventArgs e)
        => GoBack();

    private void OnNavigated(
        object? sender,
        NavigatedEventArgs e)
        => SyncWithNavigationService();

    /// <summary>
    /// Shows the service's current ViewModel and selects the item that targets it.
    /// </summary>
    private void SyncWithNavigationService()
    {
        var service = NavigationService;
        SetCurrentValue(IsBackEnabledProperty, service?.CanGoBack == true);

        var currentViewModel = service?.CurrentViewModel;
        if (currentViewModel is null)
        {
            return;
        }

        SetCurrentValue(ContentProperty, currentViewModel);

        var viewModelType = currentViewModel.GetType();
        var match = SelectedItem?.TargetViewModelType == viewModelType
            ? SelectedItem
            : AllItems().FirstOrDefault(x => x.TargetViewModelType == viewModelType);

        if (!ReferenceEquals(match, SelectedItem))
        {
            SetSelectedItemWithoutNavigating(match);
        }
    }

    private void SetSelectedItemWithoutNavigating(NavigationViewItem? item)
    {
        isSyncingSelection = true;
        try
        {
            SetCurrentValue(SelectedItemProperty, item);
        }
        finally
        {
            isSyncingSelection = false;
        }
    }

    private IEnumerable<NavigationViewItem> AllItems()
        => MenuItems.Concat(FooterMenuItems);

    private void RebuildItemsHosts()
    {
        RebuildItemsHost(menuItemsHost, MenuItems);
        RebuildItemsHost(footerMenuItemsHost, FooterMenuItems);
    }

    private static void RebuildItemsHost(
        Panel? host,
        IEnumerable<NavigationViewItem> items)
    {
        if (host is null)
        {
            return;
        }

        host.Children.Clear();
        foreach (var item in items)
        {
            if (VisualTreeHelper.GetParent(item) is Panel previousHost)
            {
                previousHost.Children.Remove(item);
            }

            host.Children.Add(item);
        }
    }
}