namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the CascaderComboBox dependency properties of these types.")]
public partial class CascaderComboBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Checked Keys Strategy", "Behavior", 1)]
    [ObservableProperty]
    private CheckedKeysStrategy checkedKeysStrategy = CheckedKeysStrategy.Parents;

    [PropertyDisplay("Expand Trigger", "Behavior", 2)]
    [ObservableProperty]
    private CascaderExpandTrigger expandTrigger = CascaderExpandTrigger.Click;

    [PropertyDisplay("Is Enabled", "Behavior", 3)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Display Mode", "Layout", 1)]
    [ObservableProperty]
    private MultiSelectDisplayMode displayMode = MultiSelectDisplayMode.Chips;

    [PropertyDisplay("Show Full Path", "Layout", 2)]
    [ObservableProperty]
    private bool showFullPath = true;

    [PropertyDisplay("Column Width", "Layout", 3)]
    [ObservableProperty]
    private double columnWidth = 160;

    public CascaderComboBoxDemoViewModel()
    {
        Regions = CascaderSampleData.CreateRegions();
        RegionKeys = [];
        Products = CascaderSampleData.CreateProducts();
        ProductKeys = ["electronics.audio"];
        Sites = CascaderSampleData.CreateSites();
        SiteKeys = [];
        LimitRegions = CascaderSampleData.CreateRegions();
        LimitRegionKeys = ["dk-jut-aar"];
        DisabledRegions = CascaderSampleData.CreateRegions();
        DisabledRegionKeys = ["dk"];
    }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Regions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> RegionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Products { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> ProductKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Sites { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SiteKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> LimitRegions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> LimitRegionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> DisabledRegions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> DisabledRegionKeys { get; set; }

    /// <summary>
    /// Gets the loader of the site › building › room tree (700 ms per level).
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> LoadSiteChildren
        => CascaderSampleData.LoadSiteChildrenAsync;
}