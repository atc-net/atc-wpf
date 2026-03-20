namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

public partial class FontIconViewerDemoViewModel : ViewModelBase
{
    private const int BatchSize = 200;

    private readonly ObservableCollection<IconItem> allIcons = [];
    private readonly DebounceDispatcher debounceDispatcher = new();
    private ICollectionView? iconsView;

    [PropertyDisplay("FA Brand", "Font Awesome", 1)]
    [ObservableProperty]
    private bool filterFontAwesomeBrand = true;

    [PropertyDisplay("FA Regular", "Font Awesome", 2)]
    [ObservableProperty]
    private bool filterFontAwesomeRegular = true;

    [PropertyDisplay("FA Solid", "Font Awesome", 3)]
    [ObservableProperty]
    private bool filterFontAwesomeSolid = true;

    [PropertyDisplay("FA7 Brand", "Font Awesome 7", 1)]
    [ObservableProperty]
    private bool filterFontAwesomeBrand7 = true;

    [PropertyDisplay("FA7 Regular", "Font Awesome 7", 2)]
    [ObservableProperty]
    private bool filterFontAwesomeRegular7 = true;

    [PropertyDisplay("FA7 Solid", "Font Awesome 7", 3)]
    [ObservableProperty]
    private bool filterFontAwesomeSolid7 = true;

    [PropertyDisplay("Bootstrap", "Other Icons", 1)]
    [ObservableProperty]
    private bool filterBootstrap = true;

    [PropertyDisplay("IcoFont", "Other Icons", 2)]
    [ObservableProperty]
    private bool filterIcoFont = true;

    [PropertyDisplay("Material Design", "Other Icons", 3)]
    [ObservableProperty]
    private bool filterMaterialDesign = true;

    [PropertyDisplay("Weather", "Other Icons", 4)]
    [ObservableProperty]
    private bool filterWeather = true;

    [PropertyDisplay("Icon Color", "Appearance", 1)]
    [ObservableProperty]
    private Color iconColor = Colors.Green;

    [PropertyDisplay("Filter", "Search", 1)]
    [ObservableProperty]
    private string filterText = string.Empty;

    [PropertyDisplay("Icon Count", "Search", 2)]
    [PropertyEditorHint(EditorHint.ReadOnly)]
    [ObservableProperty]
    private string iconCountText = string.Empty;

    public ICollectionView IconsView => iconsView!;

    public async Task InitializeAsync()
    {
        IsBusy = true;
        IconCountText = "Loading icons...";

        // Build icon data on background thread
        var items = await Task.Run(BuildAllIcons).ConfigureAwait(true);

        // Add items in batches, yielding to the dispatcher so the UI stays responsive
        for (var i = 0; i < items.Count; i += BatchSize)
        {
            var end = System.Math.Min(i + BatchSize, items.Count);
            for (var j = i; j < end; j++)
            {
                allIcons.Add(items[j]);
            }

            IconCountText = $"Loading... {end} of {items.Count}";

            await Dispatcher.Yield(DispatcherPriority.Background);
        }

        // Create the view after all items are loaded
        iconsView = CollectionViewSource.GetDefaultView(allIcons);
        iconsView.Filter = FilterPredicate;
        RaisePropertyChanged(nameof(IconsView));

        UpdateIconCount();

        IsBusy = false;
    }

    public async Task RefreshFilterAsync()
    {
        IsBusy = true;

        // Yield so the BusyOverlay renders before the heavy refresh
        await Dispatcher.Yield(DispatcherPriority.Background);

        iconsView?.Refresh();
        UpdateIconCount();

        IsBusy = false;
    }

    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "Fire-and-forget in debounce callback")]
    public void DebouncedRefreshFilter()
    {
#pragma warning disable CS4014 // Because this call is not awaited
        debounceDispatcher.Debounce(
            300,
            _ => RefreshFilterAsync());
#pragma warning restore CS4014
    }

    private bool FilterPredicate(object obj)
    {
        if (obj is not IconItem item)
        {
            return false;
        }

        if (!IsFamilyEnabled(item.FamilyPrefix))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(FilterText))
        {
            return true;
        }

        return item.EnumKey.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - enum dispatch.")]
    private bool IsFamilyEnabled(string familyPrefix)
        => familyPrefix switch
        {
            nameof(FontAwesomeBrandType) => FilterFontAwesomeBrand,
            nameof(FontAwesomeRegularType) => FilterFontAwesomeRegular,
            nameof(FontAwesomeSolidType) => FilterFontAwesomeSolid,
            nameof(FontAwesomeBrand7Type) => FilterFontAwesomeBrand7,
            nameof(FontAwesomeRegular7Type) => FilterFontAwesomeRegular7,
            nameof(FontAwesomeSolid7Type) => FilterFontAwesomeSolid7,
            nameof(FontBootstrapType) => FilterBootstrap,
            nameof(IcoFontType) => FilterIcoFont,
            nameof(FontMaterialDesignType) => FilterMaterialDesign,
            nameof(FontWeatherType) => FilterWeather,
            _ => false,
        };

    private void UpdateIconCount()
    {
        var visibleCount = iconsView?.Cast<object>().Count() ?? 0;
        IconCountText = $"Show {visibleCount} of {allIcons.Count} icons";
    }

    private static List<IconItem> BuildAllIcons()
    {
        var items = new List<IconItem>(10000);

        BuildIconFamily<FontAwesomeBrandType>(items);
        BuildIconFamily<FontAwesomeRegularType>(items);
        BuildIconFamily<FontAwesomeSolidType>(items);
        BuildIconFamily<FontAwesomeBrand7Type>(items);
        BuildIconFamily<FontAwesomeRegular7Type>(items);
        BuildIconFamily<FontAwesomeSolid7Type>(items);
        BuildIconFamily<FontBootstrapType>(items);
        BuildIconFamily<IcoFontType>(items);
        BuildIconFamily<FontMaterialDesignType>(items);
        BuildIconFamily<FontWeatherType>(items);

        return items;
    }

    private static void BuildIconFamily<T>(List<IconItem> items)
        where T : struct, Enum
    {
        var familyPrefix = typeof(T).Name;
        var dict = Enum<T>.ToDictionaryWithStringKey(
            useDescriptionAttribute: true,
            includeDefault: false);

        foreach (var pair in dict)
        {
            items.Add(new IconItem(
                familyPrefix,
                Enum<T>.Parse(pair.Key),
                pair.Key,
                pair.Key.Humanize(),
                pair.Value));
        }
    }
}