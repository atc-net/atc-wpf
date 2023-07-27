namespace Atc.Wpf.Sample.SamplesWpfTheming.Misc;

/// <summary>
/// Interaction logic for BrushesView.
/// </summary>
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
        filterText = e.NewValue;
        PopulateBrushes();
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
            foreach (var level2MergedDictionaries in level1MergedDictionaries.MergedDictionaries)
            {
                foreach (var dictKey in level2MergedDictionaries.Keys)
                {
                    var key = dictKey.ToString()!;
                    if (level2MergedDictionaries[dictKey] is not SolidColorBrush brush)
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

        BrushItemsControlForLightMode.ItemsSource = filteredBrushes.OrderBy(x => x.DisplayName, StringComparer.Ordinal);
    }

    private void PopulateBrushesForDarkMode()
    {
        var filteredBrushes = new List<BrushInfo>();

        var detectTheme = ThemeManager.Current.DetectTheme()!;
        var darkTheme = ThemeManager.Current.GetTheme($"Dark.{detectTheme.ColorScheme}")!;

        foreach (var level1MergedDictionaries in darkTheme.Resources.MergedDictionaries)
        {
            foreach (var level2MergedDictionaries in level1MergedDictionaries.MergedDictionaries)
            {
                foreach (var dictKey in level2MergedDictionaries.Keys)
                {
                    var key = dictKey.ToString()!;
                    if (level2MergedDictionaries[dictKey] is not SolidColorBrush brush)
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

        BrushItemsControlForDarkMode.ItemsSource = filteredBrushes.OrderBy(x => x.DisplayName, StringComparer.Ordinal);
    }
}