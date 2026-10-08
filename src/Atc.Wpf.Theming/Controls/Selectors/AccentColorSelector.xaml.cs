// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Theming.Controls.Selectors;

public partial class AccentColorSelector : INotifyPropertyChanged
{
    private string selectedKey = string.Empty;

    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public AccentColorSelector()
    {
        InitializeComponent();

        var detectTheme = ThemeManager.Current.DetectTheme(this);
        if (detectTheme is not null)
        {
            SelectedKey = detectTheme.ColorScheme;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
        Loaded += OnLoadedSubscribeToThemeChanges;
        Unloaded += OnUnloadedUnsubscribeFromThemeChanges;

        PopulateData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<ColorItem> Items { get; set; } = new List<ColorItem>();

    public string SelectedKey
    {
        get => selectedKey;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            selectedKey = value;
            OnPropertyChanged();

            var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);
            if (string.Equals(currentTheme?.ColorScheme, value, StringComparison.Ordinal))
            {
                return;
            }

            // An accent picked by hand wins over following the Windows accent color.
            WindowsThemeSync.Stop(WindowsThemeSyncMode.Accent);
            ThemeManager.Current.ChangeThemeColorScheme(
                Application.Current,
                value);
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private void PopulateData()
    {
        if (string.IsNullOrEmpty(SelectedKey))
        {
            selectedKey = "Blue";
        }

        Items.Clear();

        var list = new List<ColorItem>();
        foreach (var item in ThemeManager.Current
                     .Themes
                     .Where(x => !x.ColorScheme.Contains(
                         '.',
                         StringComparison.Ordinal))
                     .GroupBy(
                         x => x.ColorScheme,
                         StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(
                         x => x.ColorScheme,
                         StringComparer.Ordinal))
        {
            var translatedName = ColorNames.ResourceManager.GetString(
                item.ColorScheme,
                CultureInfo.CurrentUICulture);

            list.Add(
                new ColorItem(
                    item.ColorScheme,
                    translatedName ?? "#" + item.ColorScheme,
                    DisplayHexCode: string.Empty,
                    item.ShowcaseBrush,
                    item.ShowcaseBrush));
        }

        Items = list
            .OrderBy(
                x => x.DisplayName,
                StringComparer.Ordinal)
            .ToList();
    }

    private void OnLoadedSubscribeToThemeChanges(
        object sender,
        RoutedEventArgs e)
    {
        // ThemeManager is process-wide: only listen while in the visual tree, otherwise it keeps this control alive.
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    private void OnUnloadedUnsubscribeFromThemeChanges(
        object sender,
        RoutedEventArgs e)
        => ThemeManager.Current.ThemeChanged -= OnThemeChanged;

    /// <summary>
    /// Shows an accent applied from elsewhere, such as Windows theme sync, without applying it again.
    /// </summary>
    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        if (e.Target is not Application ||
            string.Equals(selectedKey, e.NewTheme.ColorScheme, StringComparison.Ordinal))
        {
            return;
        }

        selectedKey = e.NewTheme.ColorScheme;
        OnPropertyChanged(nameof(SelectedKey));
    }

    private void OnUiCultureChanged(
        object? sender,
        EventArgs e)
    {
        var oldSelectedKey = SelectedKey;

        PopulateData();

        OnPropertyChanged(nameof(Items));

        SelectedKey = oldSelectedKey;
    }
}