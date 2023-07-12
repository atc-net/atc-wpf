// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Interaction logic for CountrySelector.
/// </summary>
public partial class CountrySelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(CountrySelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(CountrySelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedCountriesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedCountries),
        typeof(bool),
        typeof(CountrySelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedCountries
    {
        get => (bool)GetValue(UseOnlySupportedCountriesProperty);
        set => SetValue(UseOnlySupportedCountriesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureLcidProperty = DependencyProperty.Register(
        nameof(DefaultCultureLcid),
        typeof(int?),
        typeof(CountrySelector),
        new PropertyMetadata(default(int?)));

    public int? DefaultCultureLcid
    {
        get => (int?)GetValue(DefaultCultureLcidProperty);
        set => SetValue(DefaultCultureLcidProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(CountrySelector),
        new PropertyMetadata(
            string.Empty,
            OnSelectedKeyChanged));

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var countrySelector = (CountrySelector)d;

        countrySelector.SetSelectedIndexBySelectedKey();
    }

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public static readonly DependencyProperty UpdateUiCultureOnChangeEventProperty = DependencyProperty.Register(
        nameof(UpdateUiCultureOnChangeEvent),
        typeof(bool),
        typeof(CountrySelector),
        new PropertyMetadata(defaultValue: true));

    public bool UpdateUiCultureOnChangeEvent
    {
        get => (bool)GetValue(UpdateUiCultureOnChangeEventProperty);
        set => SetValue(UpdateUiCultureOnChangeEventProperty, value);
    }

    public CountrySelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<CountryItem> Items { get; } = new List<CountryItem>();

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

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var cultures = UseOnlySupportedCountries
            ? ResourceHelper.GetSupportedCultures()
            : CultureHelper
                .GetCulturesForCountries()
                .OrderBy(x => x.CountryDisplayName, StringComparer.Ordinal)
                .ToList();

        var flags = ResourceHelper.GetFlags(RenderFlagIndicatorType);

        Items.Clear();

        switch (DropDownFirstItemType)
        {
            case DropDownFirstItemType.None:
                break;
            case DropDownFirstItemType.Blank:
                Items.Add(CreateBlankCountryItem());
                break;
            case DropDownFirstItemType.PleaseSelect:
                Items.Add(CreateCountryItem(DropDownFirstItemType));
                break;
            case DropDownFirstItemType.IncludeAll:
                Items.Add(CreateCountryItem(DropDownFirstItemType));
                break;
            default:
                throw new SwitchCaseDefaultException(DropDownFirstItemType);
        }

        foreach (var culture in cultures)
        {
            var (_, bitmapImage) = flags.FirstOrDefault(x => x.Key.Contains(culture.CountryCodeA2 + ".png", StringComparison.Ordinal));
            Items.Add(new CountryItem(culture, bitmapImage));
        }

        if (string.IsNullOrEmpty(SelectedKey))
        {
            SelectedKey = GetDefaultCountryItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
                          Items[0].Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
        }
        else
        {
            SetSelectedIndexBySelectedKey();
        }
    }

    private void SetSelectedIndexBySelectedKey()
    {
        for (var i = 0; i < CbCountries.Items.Count; i++)
        {
            var item = (CountryItem)CbCountries.Items[i];
            if (SelectedKey == item.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                CbCountries.SelectedIndex = i;
                break;
            }
        }
    }

    private static CountryItem CreateBlankCountryItem()
        => new(
            new Culture(
                (int)DropDownFirstItemType.Blank,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty),
            Image: null);

    private static CountryItem CreateCountryItem(
        DropDownFirstItemType dropDownFirstItemType)
        => new(
            new Culture(
                (int)dropDownFirstItemType,
                string.Empty,
                dropDownFirstItemType.GetDescription(useLocalizedIfPossible: false),
                dropDownFirstItemType.GetDescription(),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty),
            Image: null);

    private CountryItem? GetDefaultCountryItem()
    {
        CountryItem? defaultCountryItem = null;
        if (DefaultCultureLcid.HasValue)
        {
            var countryItem = Items.FirstOrDefault(x => x.Culture.Lcid == DefaultCultureLcid.Value);
            if (countryItem is not null)
            {
                defaultCountryItem = countryItem;
            }
        }

        return defaultCountryItem;
    }
}