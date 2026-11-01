namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the CheckListBox dependency properties of these types.")]
public partial class CheckListBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Item Style", "Appearance", 1)]
    [ObservableProperty]
    private Atc.Wpf.Controls.Selectors.CheckListItemStyle itemStyle = Atc.Wpf.Controls.Selectors.CheckListItemStyle.CheckBox;

    [PropertyDisplay("Show Select All Row", "Appearance", 2)]
    [ObservableProperty]
    private bool showSelectAllRow = true;

    [PropertyDisplay("Show Clear Button", "Appearance", 3)]
    [ObservableProperty]
    private bool showClearButton;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    public CheckListBoxDemoViewModel()
    {
        Fruits = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["apple"] = "Apple",
            ["banana"] = "Banana",
            ["cherry"] = "Cherry",
            ["date"] = "Date",
            ["elderberry"] = "Elderberry",
        };

        ExportColumns = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["id"] = "Id",
            ["name"] = "Name",
            ["email"] = "E-mail",
            ["phone"] = "Phone",
            ["company"] = "Company",
            ["created"] = "Created",
            ["modified"] = "Modified",
        };
        SelectedExportColumns = ["id", "name", "email"];

        Notifications = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["mentions"] = "When someone mentions me",
            ["replies"] = "Replies to my comments",
            ["assigned"] = "Tasks assigned to me",
            ["digest"] = "Weekly digest",
            ["marketing"] = "Product news",
        };
        SelectedNotifications = ["mentions", "assigned"];

        InitializePermissions();

        Months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
            .Where(x => x.Length > 0)
            .Select((name, index) => (Key: (index + 1).ToString(CultureInfo.InvariantCulture), Name: name))
            .ToDictionary(x => x.Key, x => x.Name, StringComparer.Ordinal);

        ManyItems = Enumerable.Range(1, 10_000)
            .ToDictionary(
                i => i.ToString(CultureInfo.InvariantCulture),
                i => string.Create(CultureInfo.InvariantCulture, $"Item {i:00000}"),
                StringComparer.Ordinal);
    }

    public Dictionary<string, string> Fruits { get; }

    public Dictionary<string, string> ExportColumns { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedExportColumns { get; set; }

    public Dictionary<string, string> Notifications { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedNotifications { get; set; }

    public Dictionary<string, string> Permissions { get; private set; }

    public Dictionary<string, string> PermissionGroups { get; private set; }

    public System.Collections.ObjectModel.ObservableCollection<string> LockedPermissions { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedPermissions { get; set; }

    public Dictionary<string, string> Months { get; }

    public Dictionary<string, string> ManyItems { get; }

    [MemberNotNull(nameof(Permissions), nameof(PermissionGroups), nameof(LockedPermissions), nameof(SelectedPermissions))]
    private void InitializePermissions()
    {
        Permissions = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["users.read"] = "Read users",
            ["users.write"] = "Edit users",
            ["users.delete"] = "Delete users",
            ["orders.read"] = "Read orders",
            ["orders.write"] = "Edit orders",
            ["reports.read"] = "Read reports",
            ["reports.export"] = "Export reports",
        };
        PermissionGroups = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["users.read"] = "Users",
            ["users.write"] = "Users",
            ["users.delete"] = "Users",
            ["orders.read"] = "Orders",
            ["orders.write"] = "Orders",
            ["reports.read"] = "Reports",
            ["reports.export"] = "Reports",
        };
        LockedPermissions = ["users.read"];
        SelectedPermissions = ["users.read", "orders.read"];
    }
}