namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public partial class DualListSelectorView
{
    public DualListSelectorView()
    {
        InitializeComponent();
        PopulateSampleData();

        CbLayoutMode.SelectionChanged += (_, _) =>
        {
            DualListSelectorControl.LayoutMode = CbLayoutMode.SelectedIndex == 0
                ? DualListSelectorLayoutMode.AvailableFirst
                : DualListSelectorLayoutMode.SelectedFirst;
        };
    }

    private void PopulateSampleData()
    {
        DualListSelectorControl.AvailableItems =
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
            new() { Identifier = "superadmin", Name = "Super Admin", Description = "Reserved \u2014 cannot be assigned", IsEnabled = false },
        ];

        DualListSelectorControl.SelectedItems =
        [
            new() { Identifier = "login", Name = "Login", Description = "Basic authentication access", SortOrderNumber = 0 },
            new() { Identifier = "profile", Name = "Profile", Description = "View and edit own profile", SortOrderNumber = 1, Tag = "default-permission" },
        ];
    }

    private void ResetOnClick(
        object sender,
        RoutedEventArgs e)
    {
        CbLayoutMode.SelectedIndex = 0;
        CbShowReorderButtons.IsChecked = true;
        CbShowTransferAllButtons.IsChecked = true;
        CbAutoRecalculateSortOrder.IsChecked = true;
        CbShowFilter.IsChecked = true;
        CbAllowDragDrop.IsChecked = true;
        CbAllowMultiSelect.IsChecked = true;
        CbShowItemCount.IsChecked = true;
        PopulateSampleData();
    }
}