namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the CheckTreeView dependency properties of these types.")]
public partial class CheckTreeViewDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Checked Keys Strategy", "Behavior", 1)]
    [ObservableProperty]
    private CheckedKeysStrategy checkedKeysStrategy = CheckedKeysStrategy.Parents;

    [PropertyDisplay("Is Check Linked", "Behavior", 2)]
    [ObservableProperty]
    private bool isCheckLinked = true;

    [PropertyDisplay("Is Enabled", "Behavior", 3)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Show Select All Row", "Layout", 1)]
    [ObservableProperty]
    private bool showSelectAllRow = true;

    [PropertyDisplay("Filter Mode", "Filter", 1)]
    [ObservableProperty]
    private TreeFilterMode filterMode = TreeFilterMode.KeepAncestors;

    public CheckTreeViewDemoViewModel()
    {
        Permissions = CheckTreeSampleData.CreatePermissions();
        PermissionKeys = [];
        Catalog = CheckTreeSampleData.CreateCatalog(10, 30, 33);
        CatalogKeys = [];
        Drives = CheckTreeSampleData.CreateDrives();
        DriveKeys = [];
    }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Permissions { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> PermissionKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Catalog { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> CatalogKeys { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Drives { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> DriveKeys { get; set; }

    /// <summary>
    /// Gets the loader of the file-system tree (600 ms per folder).
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> LoadFolder
        => CheckTreeSampleData.LoadFolderAsync;
}