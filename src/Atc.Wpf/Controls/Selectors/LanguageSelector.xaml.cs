// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
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

    public static readonly DependencyProperty DefaultCultureLcidProperty = DependencyProperty.Register(
        nameof(DefaultCultureLcid),
        typeof(int?),
        typeof(LanguageSelector),
        new PropertyMetadata(default(int?)));

    public int? DefaultCultureLcid
    {
        get => (int?)GetValue(DefaultCultureLcidProperty);
        set => SetValue(DefaultCultureLcidProperty, value);
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
            !languageSelector.SelectedKey.StartsWith('-'))
        {
            CultureManager.UiCulture = new CultureInfo(int.Parse(languageSelector.SelectedKey, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo));
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<LanguageItem> Items { get; } = new List<LanguageItem>();

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

        var cultures = UseOnlySupportedLanguages
            ? ResourceHelper.GetSupportedCultures()
            : CultureHelper
                .GetCulturesForCountries()
                .OrderBy(x => x.LanguageDisplayName, StringComparer.Ordinal)
                .ToList();

        var flags = ResourceHelper.GetFlags(RenderFlagIndicatorType);

        Items.Clear();

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
            Items.Add(new LanguageItem(culture, bitmapImage));
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

        processingOnLoaded = false;
    }

    private void SetSelectedIndexBySelectedKey()
    {
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
        if (DefaultCultureLcid.HasValue)
        {
            var languageItem = Items.FirstOrDefault(x => x.Culture.Lcid == DefaultCultureLcid.Value);
            if (languageItem is not null)
            {
                defaultLanguageItem = languageItem;
            }
        }

        return defaultLanguageItem ?? Items.FirstOrDefault(x => x.Culture.Lcid == Thread.CurrentThread.CurrentUICulture.LCID);
    }
}