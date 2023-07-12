namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for ThemeSelector.
/// </summary>
public partial class ThemeSelector : INotifyPropertyChanged
{
    private string selectedKey = string.Empty;

    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(ThemeSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public ThemeSelector()
    {
        InitializeComponent();

        var detectTheme = ThemeManager.Current.DetectTheme(this);
        if (detectTheme is not null)
        {
            SelectedKey = detectTheme.Name.Split('.')[0];
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;

        PopulateData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<ThemeItem> Items { get; set; } = new List<ThemeItem>();

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

            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, SelectedKey);
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
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