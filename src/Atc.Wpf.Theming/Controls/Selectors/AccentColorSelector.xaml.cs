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
            SelectedKey = detectTheme.Name.Split('.').Last();
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;

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

            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, SelectedKey);
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                     .Where(x => !x.ColorScheme.Contains('.', StringComparison.Ordinal))
                     .GroupBy(x => x.ColorScheme, StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(x => x.ColorScheme, StringComparer.Ordinal))
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