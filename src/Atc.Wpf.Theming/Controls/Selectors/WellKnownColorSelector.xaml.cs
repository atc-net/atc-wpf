// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for WellKnownColorSelector.
/// </summary>
public partial class WellKnownColorSelector : INotifyPropertyChanged
{
    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public static readonly DependencyProperty ShowHexCodeProperty = DependencyProperty.Register(
        nameof(ShowHexCode),
        typeof(bool),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(default(bool)));

    public bool ShowHexCode
    {
        get => (bool)GetValue(ShowHexCodeProperty);
        set => SetValue(ShowHexCodeProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(default(string)));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public WellKnownColorSelector()
    {
        InitializeComponent();

        DataContext = this;

        PopulateData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<ColorItem> Items { get; set; } = new List<ColorItem>();

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
        Items.Clear();

        var list = new List<ColorItem>();
        var colorsInfo = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var itemName in colorsInfo.Select(x => x.Name))
        {
            var translatedName = ColorNames.ResourceManager.GetString(
                itemName,
                CultureInfo.CurrentUICulture);

            var color = (Color)ColorConverter.ConvertFromString(itemName);
            var showcaseBrush = new SolidColorBrush(color);

            list.Add(
                new ColorItem(
                    itemName,
                    translatedName ?? "#" + itemName,
                    color.ToString(GlobalizationConstants.EnglishCultureInfo),
                    showcaseBrush,
                    showcaseBrush));
        }

        Items = list
            .OrderBy(x => x.DisplayName)
            .ToList();
    }
}