namespace Atc.Wpf.Sample.SamplesWpfComponents.Selectors;

public partial class DualListSelectorView
{
    public DualListSelectorView()
    {
        InitializeComponent();

        DataContext = new DualListSelectorDemoViewModel();

        PopulateSampleData();
        PopulateFeatureSamples();
    }

    private void PopulateSampleData()
    {
        InteractiveDualListSelector.AvailableItems =
        [
            new() { Identifier = "read", Name = "Read", Description = "View resources and data" },
            new() { Identifier = "write", Name = "Write", Description = "Create and modify resources" },
            new() { Identifier = "delete", Name = "Delete", Description = "Remove resources permanently" },
            new() { Identifier = "admin", Name = "Admin", Description = "Full administrative access" },
            new() { Identifier = "export", Name = "Export", Description = "Export data to external formats" },
            new() { Identifier = "import", Name = "Import", Description = "Import data from external sources" },
            new() { Identifier = "audit", Name = "Audit", Description = "View audit logs and history" },
            new() { Identifier = "config", Name = "Configuration", Description = "Modify system configuration" },
            new() { Identifier = "backup", Name = "Backup", Description = "Create and restore backups" },
            new() { Identifier = "notify", Name = "Notifications", Description = "Manage notification settings" },
            new() { Identifier = "superadmin", Name = "Super Admin", Description = "Reserved — cannot be assigned", IsEnabled = false },
        ];

        InteractiveDualListSelector.SelectedItems =
        [
            new() { Identifier = "login", Name = "Login", Description = "Basic authentication access", SortOrderNumber = 0, IsLocked = true },
            new() { Identifier = "profile", Name = "Profile", Description = "View and edit own profile", SortOrderNumber = 1, Tag = "default-permission" },
        ];
    }

    private void PopulateFeatureSamples()
    {
        CheckBoxDualList.AvailableItems = CreateItems(
            "Apple", "Apricot", "Avocado", "Banana", "Cherry", "Grape", "Mango", "Orange");
        CheckBoxDualList.ItemsTransferred += (_, _) =>
            SelectedItemsLog.Text = string.Join(", ", CheckBoxDualList.SelectedItems.Select(x => x.Name));

        OneWayDualList.AvailableItems = CreateItems(
            "Backend", "Bug", "Docs", "Frontend", "Performance", "Security", "UX");
        OneWayDualList.SelectedItems = CreateItems("Feature", "Needs review");

        LockedColumnsDualList.AvailableItems = CreateItems("Phone", "Address", "Created", "Modified", "Notes");
        LockedColumnsDualList.SelectedItems =
        [
            new() { Identifier = "id", Name = "Id", Description = "Always shown", IsLocked = true },
            new() { Identifier = "name", Name = "Name", Description = "Always shown", IsLocked = true },
            new() { Identifier = "email", Name = "E-mail" },
            new() { Identifier = "country", Name = "Country" },
        ];

        LimitDualList.AvailableItems = CreateItems(
            "Id", "Name", "E-mail", "Phone", "Address", "Zip", "City", "Country", "Created", "Modified");

        GroupedDualList.AvailableItems =
        [
            new() { Identifier = "users.read", Name = "Read users", Group = "Users" },
            new() { Identifier = "users.edit", Name = "Edit users", Group = "Users" },
            new() { Identifier = "users.delete", Name = "Delete users", Group = "Users" },
            new() { Identifier = "orders.read", Name = "Read orders", Group = "Orders" },
            new() { Identifier = "orders.edit", Name = "Edit orders", Group = "Orders" },
            new() { Identifier = "reports.read", Name = "Read reports", Group = "Reports" },
            new() { Identifier = "reports.export", Name = "Export reports", Group = "Reports" },
        ];

        LargeDualList.AvailableItems = new ObservableCollection<DualListSelectorItem>(
            Enumerable
                .Range(1, 5000)
                .Select(i => new DualListSelectorItem
                {
                    Identifier = i.ToString(CultureInfo.InvariantCulture),
                    Name = string.Create(CultureInfo.InvariantCulture, $"Item {i:0000}"),
                }));
    }

    private static ObservableCollection<DualListSelectorItem> CreateItems(
        params string[] names)
        => new(names.Select(name => new DualListSelectorItem
        {
            Identifier = name.ToLowerInvariant(),
            Name = name,
        }));
}