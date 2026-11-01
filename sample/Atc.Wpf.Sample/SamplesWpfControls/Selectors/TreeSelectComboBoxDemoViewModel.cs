namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the TreeSelectComboBox dependency properties of these types.")]
public partial class TreeSelectComboBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Checked Keys Strategy", "Behavior", 1)]
    [ObservableProperty]
    private CheckedKeysStrategy checkedKeysStrategy = CheckedKeysStrategy.Leaves;

    [PropertyDisplay("Is Enabled", "Behavior", 2)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Display Mode", "Layout", 1)]
    [ObservableProperty]
    private MultiSelectDisplayMode displayMode = MultiSelectDisplayMode.Text;

    [PropertyDisplay("Show Select All Row", "Layout", 2)]
    [ObservableProperty]
    private bool showSelectAllRow;

    [PropertyDisplay("Max Names In Summary", "Layout", 3)]
    [ObservableProperty]
    private int maxNamesInSummary = 2;

    [PropertyDisplay("Filter Mode", "Filter", 1)]
    [ObservableProperty]
    private TreeFilterMode filterMode = TreeFilterMode.KeepAncestors;

    public TreeSelectComboBoxDemoViewModel()
    {
        Regions = CheckTreeSampleData.CreateRegions();
        RegionKeys = ["dk-cph", "se-sto"];
        ChipRegions = CheckTreeSampleData.CreateRegions();
        ChipRegionKeys = ["dk", "de-berlin", "ch"];
        Permissions = CheckTreeSampleData.CreatePermissions();
        PermissionKeys = ["orders"];
        Drives = CheckTreeSampleData.CreateDrives();
        DriveKeys = [];
        DisabledRegions = CheckTreeSampleData.CreateRegions();
        DisabledRegionKeys = ["dk"];
    }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Regions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> RegionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> ChipRegions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> ChipRegionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Permissions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> PermissionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Drives { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> DriveKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> DisabledRegions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> DisabledRegionKeys { get; set; }

    /// <summary>
    /// Gets the loader of the file-system tree (600 ms per folder).
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> LoadFolder
        => CheckTreeSampleData.LoadFolderAsync;
}