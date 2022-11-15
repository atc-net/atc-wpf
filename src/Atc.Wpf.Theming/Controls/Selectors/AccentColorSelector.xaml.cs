// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for AccentColorSelector.
/// </summary>
public partial class AccentColorSelector : INotifyPropertyChanged
{
    private string selectedKey = string.Empty;

    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(AccentColorSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

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
            selectedKey = "Blue";
        }

        Items.Clear();

        var list = new List<ColorItem>();
        foreach (var item in ThemeManager.Current
                     .Themes
                     .GroupBy(x => x.ColorScheme, StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(x => x.ColorScheme))
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
            .OrderBy(x => x.DisplayName)
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