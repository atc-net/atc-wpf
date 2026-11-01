namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// Deep, uniform hierarchies for the cascader samples: country › region › city, category › sub-category › product
/// and a lazily loaded site › building › room tree.
/// </summary>
public static class CascaderSampleData
{
    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateRegions()
        =>
        [
            Node(
                "dk",
                "Denmark",
                Node("dk-jut", "Jutland", Node("dk-jut-aar", "Aarhus"), Node("dk-jut-aal", "Aalborg"), Node("dk-jut-esb", "Esbjerg"), Node("dk-jut-vej", "Vejle")),
                Node("dk-zea", "Zealand", Node("dk-zea-cph", "Copenhagen"), Node("dk-zea-ros", "Roskilde"), Node("dk-zea-hel", "Helsingør")),
                Node("dk-fun", "Funen", Node("dk-fun-ode", "Odense"), Node("dk-fun-sve", "Svendborg"))),
            Node(
                "se",
                "Sweden",
                Node("se-sve", "Svealand", Node("se-sve-sto", "Stockholm"), Node("se-sve-upp", "Uppsala"), Node("se-sve-vas", "Västerås")),
                Node("se-got", "Götaland", Node("se-got-got", "Gothenburg"), Node("se-got-mal", "Malmö"), Node("se-got-lin", "Linköping")),
                Node("se-nor", "Norrland", Node("se-nor-ume", "Umeå"), Node("se-nor-lul", "Luleå"))),
            Node(
                "no",
                "Norway",
                Node("no-ost", "Østlandet", Node("no-ost-osl", "Oslo"), Node("no-ost-dra", "Drammen")),
                Node("no-ves", "Vestlandet", Node("no-ves-ber", "Bergen"), Node("no-ves-sta", "Stavanger")),
                Node("no-tro", "Trøndelag", Node("no-tro-tro", "Trondheim"))),
            Node(
                "de",
                "Germany",
                Node("de-by", "Bavaria", Node("de-by-mun", "Munich"), Node("de-by-nur", "Nuremberg")),
                Node("de-hh", "Hamburg", Node("de-hh-ham", "Hamburg")),
                Node("de-nw", "North Rhine-Westphalia", Node("de-nw-col", "Cologne"), Node("de-nw-dus", "Düsseldorf"), Node("de-nw-dor", "Dortmund"))),
        ];

    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateProducts()
        =>
        [
            Node(
                "electronics",
                "Electronics",
                Node("electronics.computers", "Computers", Node("electronics.computers.laptops", "Laptops"), Node("electronics.computers.desktops", "Desktops"), Node("electronics.computers.tablets", "Tablets")),
                Node("electronics.audio", "Audio", Node("electronics.audio.headphones", "Headphones"), Node("electronics.audio.speakers", "Speakers")),
                Node("electronics.cameras", "Cameras", Node("electronics.cameras.compact", "Compact cameras"), Node("electronics.cameras.mirrorless", "Mirrorless cameras"))),
            Node(
                "home",
                "Home",
                Node("home.kitchen", "Kitchen", Node("home.kitchen.blenders", "Blenders"), Node("home.kitchen.kettles", "Kettles"), Node("home.kitchen.toasters", "Toasters")),
                Node("home.garden", "Garden", Node("home.garden.mowers", "Lawn mowers"), Node("home.garden.hoses", "Hoses"))),
            Node(
                "sports",
                "Sports",
                Node("sports.outdoor", "Outdoor", Node("sports.outdoor.tents", "Tents"), Node("sports.outdoor.backpacks", "Backpacks")),
                Node("sports.fitness", "Fitness", Node("sports.fitness.dumbbells", "Dumbbells"), Node("sports.fitness.mats", "Yoga mats"))),
        ];

    public static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateSites()
        =>
        [
            new CheckTreeNode("hq", "Headquarters") { HasUnloadedChildren = true },
            new CheckTreeNode("plant", "Plant") { HasUnloadedChildren = true },
            new CheckTreeNode("warehouse", "Warehouse") { HasUnloadedChildren = true },
        ];

    /// <summary>
    /// Loads the buildings of a site or the rooms of a building: simulates a slow service (700 ms).
    /// </summary>
    /// <param name="node">The site or building node.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The child nodes.</returns>
    public static async Task<IReadOnlyList<CheckTreeNode>> LoadSiteChildrenAsync(
        CheckTreeNode node,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(node);

        await Task.Delay(700, cancellationToken).ConfigureAwait(true);
        var isSite = !node.Key.Contains('/', StringComparison.Ordinal);
        var children = new List<CheckTreeNode>();
        for (var i = 1; i <= 3; i++)
        {
            children.Add(
                isSite
                    ? new CheckTreeNode(Key(node.Key + "/b", i), Text("Building ", i)) { HasUnloadedChildren = true }
                    : new CheckTreeNode(Key(node.Key + "/r", i), Text("Room ", i)));
        }

        return children;
    }

    private static string Key(
        string prefix,
        int index)
        => string.Create(CultureInfo.InvariantCulture, $"{prefix}{index}");

    private static string Text(
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
}