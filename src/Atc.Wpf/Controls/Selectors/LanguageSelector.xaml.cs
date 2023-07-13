// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Interaction logic for LanguageSelector.
/// </summary>
public partial class LanguageSelector
{
    private bool processingOnLoaded;

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

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var languageSelector = (LanguageSelector)d;

        languageSelector.SetSelectedIndexBySelectedKey();

        if (languageSelector is { processingOnLoaded: false, UpdateUiCultureOnChangeEvent: true } &&
            !string.IsNullOrEmpty(languageSelector.SelectedKey) &&
            !languageSelector.SelectedKey.StartsWith('-'))
        {
            CultureManager.UiCulture = new CultureInfo(NumberHelper.ParseToInt(languageSelector.SelectedKey));
        }
    }

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

    public LanguageSelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollectionEx<LanguageItem> Items { get; } = new();

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
        processingOnLoaded = true;

        PopulateDataOnLoaded();

        processingOnLoaded = false;
    }

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

    private void SetSelectedIndexBySelectedKey()
    {
        var selectedKey = SelectedKey;
        CbLanguages.SelectedIndex = CbLanguages.Items.Count - 1;
        SelectedKey = selectedKey;

        for (var i = 0; i < CbLanguages.Items.Count; i++)
        {
            var item = (LanguageItem)CbLanguages.Items[i];
            if (SelectedKey == item.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                CbLanguages.SelectedIndex = i;
                break;
            }
        }
    }

    private IList<Culture> GetCultures()
    {
        if (UseOnlySupportedLanguages)
        {
            return ResourceHelper.GetSupportedCultures();
        }

        var languageNames = CultureHelper.GetLanguageNames(Thread.CurrentThread.CurrentUICulture.LCID);

        var list = new List<Culture>();
        foreach (var item in languageNames)
        {
            var culture = CultureHelper.GetCultureByLcid(item.Key);
            if (culture is not null)
            {
                list.Add(culture);
            }
        }

        return list;
    }

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
            var languageItem = DefaultCultureIdentifier.IsDigitOnly()
                ? Items.FirstOrDefault(x => x.Culture.Lcid == NumberHelper.ParseToInt(DefaultCultureIdentifier))
                : Items.FirstOrDefault(x => x.Culture.Name == DefaultCultureIdentifier);

            if (languageItem is not null)
            {
                defaultLanguageItem = languageItem;
            }
        }

        return defaultLanguageItem ?? Items.FirstOrDefault(x => x.Culture.Lcid == Thread.CurrentThread.CurrentUICulture.LCID);
    }
}