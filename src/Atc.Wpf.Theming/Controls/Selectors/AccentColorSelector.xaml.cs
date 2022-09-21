namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for AccentColorSelector.xaml
/// </summary>
public partial class AccentColorSelector
{
    private string selectedKey = "Blue";

    public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.Register(
        nameof(ShowLabel),
        typeof(bool),
        typeof(AccentColorSelector),
        new PropertyMetadata(defaultValue: true));

    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(AccentColorSelector),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

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

        DataContext = this;

        Items = new List<AccentColorItem>();
        foreach (var item in ThemeManager.Current
                     .Themes
                     .GroupBy(x => x.ColorScheme, StringComparer.Ordinal)
                     .Select(x => x.First())
                     .OrderBy(x => x.ColorScheme))
        {
            var translatedName = ColorNames.ResourceManager.GetString(
                item.ColorScheme,
                CultureInfo.CurrentUICulture);

            Items.Add(
                new AccentColorItem(
                    item.ColorScheme,
                    translatedName ?? "#" + item.ColorScheme,
                    item.ShowcaseBrush,
                    item.ShowcaseBrush));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<AccentColorItem> Items { get; }

    public string SelectedKey
    {
        get => selectedKey;
        set
        {
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
}