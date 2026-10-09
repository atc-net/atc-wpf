namespace Atc.Wpf.Theming.Controls.Selectors;

public partial class ThemeSelector : INotifyPropertyChanged
{
    private string selectedKey = string.Empty;

    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSelector"/> class.
    /// </summary>
    public ThemeSelector()
    {
        InitializeComponent();

        var detectTheme = ThemeManager.Current.DetectTheme(this);
        if (detectTheme is not null)
        {
            SelectedKey = detectTheme.BaseColorScheme;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
        Loaded += OnLoadedSubscribeToThemeChanges;
        Unloaded += OnUnloadedUnsubscribeFromThemeChanges;

        PopulateData();
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the available themes.
    /// </summary>
    public IList<ThemeItem> Items { get; set; } = new List<ThemeItem>();

    /// <summary>
    /// Gets or sets the base color scheme name of the selected theme. Setting it changes the application theme;
    /// empty values are ignored.
    /// </summary>
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
            if (string.Equals(currentTheme?.BaseColorScheme, value, StringComparison.Ordinal))
            {
                return;
            }

            // A theme picked by hand wins over following the Windows app mode.
            WindowsThemeSync.Stop(WindowsThemeSyncMode.AppMode);
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, value);
        }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void PopulateData()
    {
        if (string.IsNullOrEmpty(SelectedKey))
        {
            selectedKey = "Light";
        }

        Items.Clear();

        foreach (var item in ThemeManager.Current
                     .Themes
                     .GroupBy(x => x.BaseColorScheme, StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(x => x.BaseColorScheme, StringComparer.Ordinal))
        {
            var translatedName = ColorNames.ResourceManager.GetString(
                item.BaseColorScheme,
                CultureInfo.CurrentUICulture);

            var borderColorBrush = item.Resources["AtcApps.Brushes.ThemeForeground"] as Brush;
            var colorBrush = item.Resources["AtcApps.Brushes.ThemeBackground"] as Brush;
            Items.Add(
                new ThemeItem(
                    item.BaseColorScheme,
                    translatedName ?? "#" + item.BaseColorScheme,
                    borderColorBrush!,
                    colorBrush!));
        }

        Items = Items
            .OrderBy(x => x.DisplayName, StringComparer.Ordinal)
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
    /// Shows a theme applied from elsewhere, such as Windows theme sync, without applying it again.
    /// </summary>
    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        if (e.Target is not Application ||
            string.Equals(selectedKey, e.NewTheme.BaseColorScheme, StringComparison.Ordinal))
        {
            return;
        }

        selectedKey = e.NewTheme.BaseColorScheme;
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