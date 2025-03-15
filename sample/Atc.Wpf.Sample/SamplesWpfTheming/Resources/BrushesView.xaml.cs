namespace Atc.Wpf.Sample.SamplesWpfTheming.Resources;

[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "OK.")]
[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "OK.")]
public partial class BrushesView : INotifyPropertyChanged
{
    private List<BrushInfo> brushes = new();
    private string filterText = string.Empty;

    public BrushesView()
    {
        InitializeComponent();
        DataContext = this;

        PopulateBrushes();

        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<BrushInfo> Brushes
    {
        get => brushes;
        set
        {
            brushes = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        PopulateBrushes();
    }

    private void OnFilterTextChanged(
        object sender,
        RoutedPropertyChangedEventArgs<string> e)
    {
        filterText = e.NewValue.Replace("AtcApps.Brushes.", string.Empty, StringComparison.Ordinal);
        PopulateBrushes();
    }

    private void OnCopyKeyToClipboardClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not MenuItem item ||
            item.CommandParameter is null)
        {
            return;
        }

        var key = item.CommandParameter.ToString()!;
        Clipboard.SetText(key);
    }

    private void OnCopyColorCodeToClipboardClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not MenuItem item ||
            item.CommandParameter is null)
        {
            return;
        }

        var brush = item.CommandParameter;
        Clipboard.SetText(brush.ToString()!);
    }

    private void PopulateBrushes()
    {
        PopulateBrushesForLightMode();
        PopulateBrushesForDarkMode();
    }

    private void PopulateBrushesForLightMode()
    {
        var filteredBrushes = new List<BrushInfo>();

        var detectTheme = ThemeManager.Current.DetectTheme()!;
        var lightTheme = ThemeManager.Current.GetTheme($"Light.{detectTheme.ColorScheme}")!;

        foreach (var level1MergedDictionaries in lightTheme.Resources.MergedDictionaries)
        {
            CollectResources(level1MergedDictionaries, filteredBrushes);
        }

        BrushItemsControlForLightMode.ItemsSource = filteredBrushes.OrderBy(x => x.DisplayName, StringComparer.Ordinal);
    }

    private void PopulateBrushesForDarkMode()
    {
        var filteredBrushes = new List<BrushInfo>();

        var detectTheme = ThemeManager.Current.DetectTheme()!;
        var darkTheme = ThemeManager.Current.GetTheme($"Dark.{detectTheme.ColorScheme}")!;

        foreach (var level1MergedDictionaries in darkTheme.Resources.MergedDictionaries)
        {
            CollectResources(level1MergedDictionaries, filteredBrushes);
        }

        BrushItemsControlForDarkMode.ItemsSource = filteredBrushes.OrderBy(x => x.DisplayName, StringComparer.Ordinal);
    }

    private void CollectResources(
        ResourceDictionary resourceDictionary,
        ICollection<BrushInfo> filteredBrushes)
    {
        foreach (var resourceDict in resourceDictionary.MergedDictionaries)
        {
            foreach (var dictKey in resourceDict.Keys)
            {
                var key = dictKey.ToString()!;
                if (resourceDict[dictKey] is not SolidColorBrush brush)
                {
                    continue;
                }

                var displayName = key.Replace("AtcApps.Brushes.", string.Empty, StringComparison.Ordinal);
                if (filterText.Length > 0)
                {
                    if (displayName.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredBrushes.Add(
                            new BrushInfo(
                                key,
                                displayName,
                                brush));
                    }
                }
                else
                {
                    filteredBrushes.Add(
                        new BrushInfo(
                            key,
                            displayName,
                            brush));
                }
            }
        }
    }
}