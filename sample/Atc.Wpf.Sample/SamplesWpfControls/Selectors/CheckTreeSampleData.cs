namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// Sample trees for the check tree pages: permissions, regions, a product catalog and a lazily loaded file system.
/// </summary>
public static class CheckTreeSampleData
{
    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreatePermissions()
        =>
        [
            Node(
                "orders",
                "Orders",
                Node("orders.view", "View orders"),
                Node("orders.create", "Create orders"),
                Node("orders.approve", "Approve orders"),
                Node("orders.delete", "Delete orders")),
            Node(
                "invoices",
                "Invoices",
                Node("invoices.view", "View invoices"),
                Node("invoices.send", "Send invoices"),
                Node("invoices.credit", "Credit invoices")),
            Node(
                "admin",
                "Administration",
                Node(
                    "admin.users",
                    "Users",
                    Node("admin.users.view", "View users"),
                    Node("admin.users.edit", "Edit users"),
                    Disabled(Node("admin.users.impersonate", "Impersonate users (licensed add-on)"))),
                Node(
                    "admin.settings",
                    "Settings",
                    Node("admin.settings.view", "View settings"),
                    Node("admin.settings.edit", "Edit settings"))),
        ];

    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateRegions()
        =>
        [
            Node(
                "nordics",
                "Nordics",
                Node(
                    "dk",
                    "Denmark",
                    Node("dk-aarhus", "Aarhus"),
                    Node("dk-cph", "Copenhagen"),
                    Node("dk-odense", "Odense")),
                Node(
                    "se",
                    "Sweden",
                    Node("se-got", "Gothenburg"),
                    Node("se-malmo", "Malmö"),
                    Node("se-sto", "Stockholm")),
                Node(
                    "no",
                    "Norway",
                    Node("no-bergen", "Bergen"),
                    Node("no-oslo", "Oslo"))),
            Node(
                "dach",
                "DACH",
                Node(
                    "de",
                    "Germany",
                    Node("de-berlin", "Berlin"),
                    Node("de-hamburg", "Hamburg"),
                    Node("de-munich", "Munich")),
                Node(
                    "at",
                    "Austria",
                    Node("at-vienna", "Vienna")),
                Node(
                    "ch",
                    "Switzerland",
                    Node("ch-basel", "Basel"),
                    Node("ch-zurich", "Zurich"))),
        ];

    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateCatalog(
        int categoryCount,
        int groupCount,
        int productCount)
    {
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>();
        for (var c = 0; c < categoryCount; c++)
        {
            var category = new CheckTreeNode(Key("c", c), string.Create(CultureInfo.InvariantCulture, $"Category {c + 1}"));
            for (var g = 0; g < groupCount; g++)
            {
                var group = new CheckTreeNode(
                    Key(category.Key + "-g", g),
                    string.Create(CultureInfo.InvariantCulture, $"Group {c + 1}.{g + 1}"));
                for (var p = 0; p < productCount; p++)
                {
                    group.Children.Add(new CheckTreeNode(
                        Key(group.Key + "-p", p),
                        string.Create(CultureInfo.InvariantCulture, $"Product {c + 1}.{g + 1}.{p + 1}")));
                }

                category.Children.Add(group);
            }

            roots.Add(category);
        }

        return roots;
    }

    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateDrives()
        =>
        [
            Folder("C:", "C:"),
            Folder("D:", "D:"),
        ];

    /// <summary>
    /// Loads the sub-folders and files of a folder: simulates a slow file system (600 ms).
    /// </summary>
    /// <param name="folder">The folder node.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The child nodes.</returns>
    public static async Task<IReadOnlyList<CheckTreeNode>> LoadFolderAsync(
        CheckTreeNode folder,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(folder);

        await Task.Delay(600, cancellationToken).ConfigureAwait(true);
        var depth = folder.Key.Count(x => x == Path.DirectorySeparatorChar);
        var children = new List<CheckTreeNode>();
        if (depth < 3)
        {
            foreach (var name in new[] { "Documents", "Projects", "Pictures" })
            {
                children.Add(Folder(folder.Key + Path.DirectorySeparatorChar + name, name));
            }
        }

        for (var i = 1; i <= 3; i++)
        {
            var name = string.Create(CultureInfo.InvariantCulture, $"file{i}.txt");
            children.Add(new CheckTreeNode(folder.Key + Path.DirectorySeparatorChar + name, name) { Icon = "📄" });
        }

        return children;
    }

    private static CheckTreeNode Folder(
        string key,
        string text)
        => new(key, text)
        {
            Icon = "📁",
            HasUnloadedChildren = true,
        };

    private static string Key(
        string prefix,
        int index)
        => string.Create(CultureInfo.InvariantCulture, $"{prefix}{index}");

    private static CheckTreeNode Node(
        string key,
        string text,
        params CheckTreeNode[] children)
    {
        var node = new CheckTreeNode(key, text);
        foreach (var child in children)
        {
            node.Children.Add(child);
        }

        return node;
    }

    private static CheckTreeNode Disabled(CheckTreeNode node)
    {
        node.IsEnabled = false;
        return node;
    }
}