namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for ThemeSelector.xaml
/// </summary>
public partial class ThemeSelector : INotifyPropertyChanged
{
    private string selectedKey = "Light";

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(ThemeSelector),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

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

        DataContext = this;

        Items = new List<ThemeItem>();
        foreach (var item in ThemeManager.Current
                     .Themes
                     .GroupBy(x => x.BaseColorScheme, StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(x => x.BaseColorScheme))
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<ThemeItem> Items { get; }

    public string SelectedKey
    {
        get => selectedKey;
        set
        {
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
}