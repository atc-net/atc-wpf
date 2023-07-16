// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Interaction logic for LanguageSelector.
/// </summary>
public partial class LanguageSelector
{
    private readonly ObservableCollectionEx<LanguageItem> items = new();
    private static DateTime lastKeyChanged = DateTime.MinValue;
    private DateTime lastItemChanged = DateTime.MinValue;
    private LanguageItem? lastChangedToItem;
    private bool processingUiCultureChanged;

    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LanguageSelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LanguageSelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedLanguagesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedLanguages),
        typeof(bool),
        typeof(LanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedLanguages
    {
        get => (bool)GetValue(UseOnlySupportedLanguagesProperty);
        set => SetValue(UseOnlySupportedLanguagesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureIdentifierProperty = DependencyProperty.Register(
        nameof(DefaultCultureIdentifier),
        typeof(string),
        typeof(LanguageSelector),
        new PropertyMetadata(default));

    public string? DefaultCultureIdentifier
    {
        get => (string?)GetValue(DefaultCultureIdentifierProperty);
        set => SetValue(DefaultCultureIdentifierProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LanguageSelector),
        new PropertyMetadata(
            string.Empty,
            OnSelectedKeyChanged));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public static readonly DependencyProperty UpdateUiCultureOnChangeEventProperty = DependencyProperty.Register(
        nameof(UpdateUiCultureOnChangeEvent),
        typeof(bool),
        typeof(LanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UpdateUiCultureOnChangeEvent
    {
        get => (bool)GetValue(UpdateUiCultureOnChangeEventProperty);
        set => SetValue(UpdateUiCultureOnChangeEventProperty, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<ChangedStringEventArgs>? SelectorChanged;

    public LanguageSelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public ObservableCollectionEx<LanguageItem> Items => items;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => PopulateDataOnLoaded();

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => UpdateDataOnUiCultureChanged();

    private void PopulateDataOnLoaded()
    {
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
            SelectedKey = GetDefaultLanguageItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
                          Items[0].Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
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
                var dropDownFirstItemType = (DropDownFirstItemType)item.Culture.Lcid;
                switch (dropDownFirstItemType)
                {
                    case DropDownFirstItemType.None:
                    case DropDownFirstItemType.Blank:
                        break;
                    case DropDownFirstItemType.PleaseSelect:
                    case DropDownFirstItemType.IncludeAll:
                        item.Culture.LanguageEnglishName = dropDownFirstItemType.GetDescription(useLocalizedIfPossible: false);
                        item.Culture.LanguageDisplayName = dropDownFirstItemType.GetDescription();
                        break;
                    default:
                        throw new SwitchCaseDefaultException(dropDownFirstItemType);
                }
            }
        }

        SortItems();

        if (string.IsNullOrEmpty(SelectedKey))
        {
            SelectedKey = GetDefaultLanguageItem()?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
                          Items[0].Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
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
            var item = (LanguageItem)CbLanguages.Items[i];
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
            if (CbLanguages.Items.Count > backupIndex)
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
            CultureManager.UiCulture = new CultureInfo(NumberHelper.ParseToInt(languageSelector.SelectedKey));
        }
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        => OnSelectionChanged(sender, (LanguageItem)e.AddedItems[0]!);

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        LanguageItem languageItem)
    {
        if (processingUiCultureChanged ||
            Items.SuppressOnChangedNotification)
        {
            return;
        }

        if ((lastChangedToItem is null ||
            languageItem.Culture.Lcid != lastChangedToItem.Culture.Lcid) &&
            lastItemChanged.DateTimeDiff(DateTime.Now, DateTimeDiffCompareType.Seconds) > 1)
        {
            lastItemChanged = DateTime.Now;
            lastChangedToItem = languageItem;

            Debug.WriteLine($"LanguageSelector - Change to: {languageItem.Culture.Lcid} ({languageItem.Culture.Name})");

            SelectorChanged?.Invoke(
                this,
                new ChangedStringEventArgs(
                identifier: Guid.Empty.ToString(),
                oldValue: null,
                newValue: languageItem.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo)));
        }
    }
}