// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.Selectors;

public partial class LanguageSelector
{
    private readonly ObservableCollectionEx<LanguageItem> items = new();
    private static DateTime lastKeyChanged = DateTime.MinValue;
    private int? lastLcid;
    private bool processingUiCultureChanged;

    [DependencyProperty(DefaultValue = DropDownFirstItemType.None)]
    private DropDownFirstItemType dropDownFirstItemType;

    [DependencyProperty(DefaultValue = RenderFlagIndicatorType.Flat16)]
    private RenderFlagIndicatorType renderFlagIndicatorType;

    [DependencyProperty(DefaultValue = true)]
    private bool useOnlySupportedLanguages;

    [DependencyProperty(DefaultValue = "")]
    private string defaultCultureIdentifier;

    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnSelectedKeyChanged))]
    private string selectedKey;

    [DependencyProperty(DefaultValue = true)]
    private bool updateUiCultureOnChangeEvent;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public LanguageSelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public ObservableCollectionEx<LanguageItem> Items => items;

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
                Items.Add(CreateBlankLanguageItem());
                break;
            case DropDownFirstItemType.PleaseSelect:
                Items.Add(CreateLanguageItem(DropDownFirstItemType));
                break;
            case DropDownFirstItemType.IncludeAll:
                Items.Add(CreateLanguageItem(DropDownFirstItemType));
                break;
            default:
                throw new SwitchCaseDefaultException(DropDownFirstItemType);
        }

        foreach (var culture in cultures)
        {
            var (_, bitmapImage) = flags.FirstOrDefault(x => x.Key.Contains(culture.CountryCodeA2 + ".png", StringComparison.Ordinal));
            Items.Add(new LanguageItem(culture.Clone(), bitmapImage));
        }

        if (string.IsNullOrEmpty(SelectedKey))
        {
            if (DropDownFirstItemType != DropDownFirstItemType.None)
            {
                SelectedKey = ((int)DropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
            else
            {
                SelectedKey = GetDefaultLanguageItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
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
                var culture = cultures.Single(x => x.Lcid == item.Culture.Lcid);
                item.Culture.CountryEnglishName = culture.CountryEnglishName;
                item.Culture.CountryDisplayName = culture.CountryDisplayName;
                item.Culture.LanguageEnglishName = culture.LanguageEnglishName;
                item.Culture.LanguageDisplayName = culture.LanguageDisplayName;
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
                        item.Culture.LanguageEnglishName = ddfItemType.GetDescription(useLocalizedIfPossible: false);
                        item.Culture.LanguageDisplayName = ddfItemType.GetDescription();
                        break;
                    default:
                        throw new SwitchCaseDefaultException(ddfItemType);
                }
            }
        }

        SortItems();

        if (string.IsNullOrEmpty(SelectedKey))
        {
            var key = GetDefaultLanguageItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
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
            .OrderBy(x => x.Culture.LanguageDisplayName, StringComparer.Ordinal)
            .ToList();

        Items.Clear();
        if (firstItem is not null)
        {
            Items.Add(firstItem);
        }

        Items.AddRange(sortedList);
    }

    private void SetSelectedIndexBySelectedKey()
    {
        UpdateTranslationForSelectedItem();

        for (var i = 0; i < CbLanguages.Items.Count; i++)
        {
            var item = (LanguageItem)CbLanguages.Items[i]!;
            if (SelectedKey == item.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                CbLanguages.SelectedIndex = i;
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
        if (CbLanguages.SelectedIndex != -1)
        {
            var backupIndex = CbLanguages.SelectedIndex;
            if (CbLanguages.Items.Count > backupIndex + 1)
            {
                CbLanguages.SelectedIndex = backupIndex + 1;
            }
            else
            {
                CbLanguages.SelectedIndex = backupIndex - 1;
            }

            CbLanguages.SelectedIndex = backupIndex;
        }
    }

    private IList<Culture> GetCultures()
        => UseOnlySupportedLanguages
            ? CultureHelper.GetSupportedCultures(Thread.CurrentThread.CurrentUICulture.LCID)
            : CultureHelper.GetCulturesForLanguages();

    private static LanguageItem CreateBlankLanguageItem()
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

    private static LanguageItem CreateLanguageItem(
        DropDownFirstItemType dropDownFirstItemType)
        => new(
            new Culture(
                (int)dropDownFirstItemType,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                dropDownFirstItemType.GetDescription(useLocalizedIfPossible: false),
                dropDownFirstItemType.GetDescription(),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty),
            Image: null);

    private LanguageItem? GetDefaultLanguageItem()
    {
        LanguageItem? defaultLanguageItem = null;
        if (!string.IsNullOrEmpty(DefaultCultureIdentifier))
        {
            var languageItem = NumberHelper.IsInt(DefaultCultureIdentifier)
                ? Items.FirstOrDefault(x => x.Culture.Lcid == NumberHelper.ParseToInt(DefaultCultureIdentifier))
                : Items.FirstOrDefault(x => x.Culture.Name == DefaultCultureIdentifier);

            if (languageItem is not null)
            {
                defaultLanguageItem = languageItem;
            }
        }

        return defaultLanguageItem ?? Items.FirstOrDefault(x => x.Culture.Lcid == Thread.CurrentThread.CurrentUICulture.LCID);
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => UpdateDataOnUiCultureChanged();

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var languageSelector = (LanguageSelector)d;

        languageSelector.SetSelectedIndexBySelectedKey();

        if (languageSelector is { processingUiCultureChanged: false, UpdateUiCultureOnChangeEvent: true } &&
            !string.IsNullOrEmpty(languageSelector.SelectedKey) &&
            !languageSelector.SelectedKey.StartsWith('-') &&
            lastKeyChanged.DateTimeDiff(DateTime.Now, DateTimeDiffCompareType.Seconds) > 1)
        {
            lastKeyChanged = DateTime.Now;
            if (CultureManager.UiCulture.LCID.ToString(GlobalizationConstants.EnglishCultureInfo) != languageSelector.SelectedKey)
            {
                CultureManager.UiCulture = new CultureInfo(NumberHelper.ParseToInt(languageSelector.SelectedKey));
            }
        }
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            OnSelectionChanged(sender, (LanguageItem)e.AddedItems[0]!);
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        LanguageItem languageItem)
    {
        if (processingUiCultureChanged ||
            Items.SuppressOnChangedNotification)
        {
            return;
        }

        if (lastLcid is not null &&
            lastLcid == languageItem.Culture.Lcid)
        {
            return;
        }

        lastLcid = languageItem.Culture.Lcid;

        Debug.WriteLine($"LanguageSelector - Change to: {languageItem.Culture.Lcid} ({languageItem.Culture.Name})");

        SelectorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<string?>(
            identifier: Guid.Empty.ToString(),
            oldValue: null,
            newValue: languageItem.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo)));
    }
}