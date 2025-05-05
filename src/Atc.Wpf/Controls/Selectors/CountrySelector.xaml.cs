// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.Selectors;

public partial class CountrySelector
{
    private readonly ObservableCollectionEx<CountryItem> items = new();
    private int? lastLcid;
    private bool processingUiCultureChanged;

    [DependencyProperty(DefaultValue = DropDownFirstItemType.None)]
    private DropDownFirstItemType dropDownFirstItemType;

    [DependencyProperty(DefaultValue = RenderFlagIndicatorType.Flat16)]
    private RenderFlagIndicatorType renderFlagIndicatorType;

    [DependencyProperty(DefaultValue = true)]
    private bool useOnlySupportedCountries;

    [DependencyProperty]
    private string defaultCultureIdentifier;

    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnSelectedKeyChanged))]
    private string selectedKey;

    [DependencyProperty(DefaultValue = true)]
    private bool updateUiCultureOnChangeEvent;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public CountrySelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public ObservableCollectionEx<CountryItem> Items => items;

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => PopulateDataOnLoaded();

    private void PopulateDataOnLoaded()
    {
        if (Items.Count > 0)
        {
            return;
        }

        Items.SuppressOnChangedNotification = true;

        var cultures = GetCultures();
        var flags = ResourceHelper.GetFlags(RenderFlagIndicatorType);

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
            Items.Add(new CountryItem(culture.Clone(), bitmapImage));
        }

        if (string.IsNullOrEmpty(SelectedKey))
        {
            if (DropDownFirstItemType != DropDownFirstItemType.None)
            {
                SelectedKey = ((int)DropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
            else
            {
                SelectedKey = GetDefaultCountryItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
                              Items[0].Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
            }
        }
        else
        {
            SetSelectedIndexBySelectedKey();
        }

        Items.SuppressOnChangedNotification = false;
    }

    private void UpdateDataOnUiCultureChanged()
    {
        processingUiCultureChanged = true;
        Items.SuppressOnChangedNotification = true;

        var cultures = GetCultures();

        foreach (var item in Items)
        {
            if (item.Culture.Lcid > 0)
            {
                var culture = cultures.SingleOrDefault(x => x.Lcid == item.Culture.Lcid);
                if (culture is not null)
                {
                    item.Culture.CountryEnglishName = culture.CountryEnglishName;
                    item.Culture.CountryDisplayName = culture.CountryDisplayName;
                }
            }
            else
            {
                var ddfItemType = (DropDownFirstItemType)item.Culture.Lcid;
                switch (ddfItemType)
                {
                    case DropDownFirstItemType.None:
                    case DropDownFirstItemType.Blank:
                        break;
                    case DropDownFirstItemType.PleaseSelect:
                    case DropDownFirstItemType.IncludeAll:
                        item.Culture.CountryEnglishName = ddfItemType.GetDescription(useLocalizedIfPossible: false);
                        item.Culture.CountryDisplayName = ddfItemType.GetDescription();
                        break;
                    default:
                        throw new SwitchCaseDefaultException(ddfItemType);
                }
            }
        }

        SortItems();

        if (string.IsNullOrEmpty(SelectedKey))
        {
            var key = GetDefaultCountryItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
            if (key is null &&
                Items.Count > 0)
            {
                key = Items[0].Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
            }

            if (key is not null)
            {
                SelectedKey = key;
            }
        }
        else
        {
            SetSelectedIndexBySelectedKey();
        }

        Items.SuppressOnChangedNotification = false;
        processingUiCultureChanged = false;
    }

    private void SortItems()
    {
        var firstItem = Items.FirstOrDefault(x => x.Culture.Lcid <= 0);
        var sortedList = Items
            .Where(x => x.Culture.Lcid > 0)
            .OrderBy(x => x.Culture.CountryDisplayName, StringComparer.Ordinal)
            .ToList();

        items.Clear();
        if (firstItem is not null)
        {
            items.Add(firstItem);
        }

        items.AddRange(sortedList);
    }

    private void SetSelectedIndexBySelectedKey()
    {
        UpdateTranslationForSelectedItem();

        for (var i = 0; i < CbCountries.Items.Count; i++)
        {
            var item = (CountryItem)CbCountries.Items[i]!;
            if (SelectedKey == item.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                CbCountries.SelectedIndex = i;
                if (!processingUiCultureChanged)
                {
                    OnSelectionChanged(this, item);
                }

                break;
            }
        }
    }

    private void UpdateTranslationForSelectedItem()
    {
        if (CbCountries.SelectedIndex != -1)
        {
            var backupIndex = CbCountries.SelectedIndex;
            if (CbCountries.Items.Count > backupIndex + 1)
            {
                CbCountries.SelectedIndex = backupIndex + 1;
            }
            else
            {
                CbCountries.SelectedIndex = backupIndex - 1;
            }

            CbCountries.SelectedIndex = backupIndex;
        }
    }

    private IList<Culture> GetCultures()
        => UseOnlySupportedCountries
            ? CultureHelper.GetSupportedCultures(Thread.CurrentThread.CurrentUICulture.LCID)
            : CultureHelper.GetCulturesForCountries();

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
        if (!string.IsNullOrEmpty(DefaultCultureIdentifier))
        {
            var countryItem = NumberHelper.IsInt(DefaultCultureIdentifier)
                ? Items.FirstOrDefault(x => x.Culture.Lcid == NumberHelper.ParseToInt(DefaultCultureIdentifier))
                : Items.FirstOrDefault(x => x.Culture.Name == DefaultCultureIdentifier);

            if (countryItem is not null)
            {
                defaultCountryItem = countryItem;
            }
        }

        return defaultCountryItem;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => UpdateDataOnUiCultureChanged();

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var countrySelector = (CountrySelector)d;

        countrySelector.SetSelectedIndexBySelectedKey();
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            OnSelectionChanged(sender, (CountryItem)e.AddedItems[0]!);
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        CountryItem countryItem)
    {
        if (processingUiCultureChanged ||
            Items.SuppressOnChangedNotification)
        {
            return;
        }

        if (lastLcid is not null &&
            lastLcid == countryItem.Culture.Lcid)
        {
            return;
        }

        lastLcid = countryItem.Culture.Lcid;

        Debug.WriteLine($"CountrySelector - Change to: {countryItem.Culture.Lcid} ({countryItem.Culture.Name})");

        SelectorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<string?>(
                identifier: Guid.Empty.ToString(),
                oldValue: null,
                newValue: countryItem.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo)));
    }
}