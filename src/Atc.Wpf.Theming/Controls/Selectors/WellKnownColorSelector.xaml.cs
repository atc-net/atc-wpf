// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for WellKnownColorSelector.
/// </summary>
public partial class WellKnownColorSelector
{
    private string? lastName;
    private bool processingUiCultureChanged;

    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

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
        new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool ShowHexCode
    {
        get => (bool)GetValue(ShowHexCodeProperty);
        set => SetValue(ShowHexCodeProperty, value);
    }

    public static readonly DependencyProperty UseOnlyBasicColorsProperty = DependencyProperty.Register(
        nameof(UseOnlyBasicColors),
        typeof(bool),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool UseOnlyBasicColors
    {
        get => (bool)GetValue(UseOnlyBasicColorsProperty);
        set => SetValue(UseOnlyBasicColorsProperty, value);
    }

    public static readonly DependencyProperty DefaultColorNameProperty = DependencyProperty.Register(
        nameof(DefaultColorName),
        typeof(string),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(default));

    public string? DefaultColorName
    {
        get => (string?)GetValue(DefaultColorNameProperty);
        set => SetValue(DefaultColorNameProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(WellKnownColorSelector),
        new PropertyMetadata(
            string.Empty,
            OnSelectedKeyChanged));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public WellKnownColorSelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public ObservableCollectionEx<ColorItem> Items { get; } = new();

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => PopulateDataOnLoaded();

    private void PopulateDataOnLoaded()
    {
        Items.SuppressOnChangedNotification = true;

        PopulateData(setSelected: true);

        Items.SuppressOnChangedNotification = false;
    }

    private void UpdateDataOnUiCultureChanged()
    {
        processingUiCultureChanged = true;
        Items.SuppressOnChangedNotification = true;

        var backupSelectedKey = SelectedKey;
        Items.Clear();
        PopulateData(setSelected: false);
        SelectedKey = backupSelectedKey;

        Items.SuppressOnChangedNotification = false;
        processingUiCultureChanged = false;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void PopulateData(
        bool setSelected)
    {
        switch (DropDownFirstItemType)
        {
            case DropDownFirstItemType.None:
                break;
            case DropDownFirstItemType.Blank:
                Items.Add(CreateBlankColorItem());
                break;
            case DropDownFirstItemType.PleaseSelect:
                Items.Add(CreateColorItem(DropDownFirstItemType));
                break;
            case DropDownFirstItemType.IncludeAll:
                Items.Add(CreateColorItem(DropDownFirstItemType));
                break;
            default:
                throw new SwitchCaseDefaultException(DropDownFirstItemType);
        }

        var coloKeys = GetColorKeys();

        var list = new List<ColorItem>();
        foreach (var itemKey in coloKeys)
        {
            var translatedName = ColorHelper.GetColorNameFromKey(itemKey, CultureInfo.CurrentUICulture);

            var color = ColorHelper.GetColorFromName(itemKey, GlobalizationConstants.EnglishCultureInfo);
            if (color is null)
            {
                continue;
            }

            var showcaseBrush = SolidColorBrushHelper.GetBrushFromName(itemKey, GlobalizationConstants.EnglishCultureInfo);
            if (showcaseBrush is null)
            {
                continue;
            }

            list.Add(
                new ColorItem(
                    Key: itemKey,
                    DisplayName: translatedName ?? "#" + itemKey,
                    DisplayHexCode: color.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
                    BorderColorBrush: showcaseBrush,
                    ColorBrush: showcaseBrush));
        }

        Items.AddRange(list.OrderBy(x => x.DisplayName, StringComparer.Ordinal));

        UpdateTranslationForSelectedItem();

        if (setSelected)
        {
            if (string.IsNullOrEmpty(SelectedKey))
            {
                SelectedKey = GetDefaultColorItem()?.Key ??
                              Items[0].Key;
            }
            else
            {
                SetSelectedIndexBySelectedKey();
            }
        }
    }

    private IEnumerable<string> GetColorKeys()
        => UseOnlyBasicColors
            ? ColorHelper.GetBasicColorKeys()
            : ColorHelper.GetColorKeys();

    private void SetSelectedIndexBySelectedKey()
    {
        if (CbColors.SelectedValue is null)
        {
            return;
        }

        var selectedValue = CbColors.SelectedValue.ToString();
        for (var i = 0; i < CbColors.Items.Count; i++)
        {
            if (CbColors.Items[i] is ColorItem item &&
                item.Key == selectedValue)
            {
                if (CbColors.SelectedIndex != i)
                {
                    CbColors.SelectedIndex = i;
                    if (!processingUiCultureChanged)
                    {
                        OnSelectionChanged(this, item);
                    }
                }

                break;
            }
        }
    }

    private void UpdateTranslationForSelectedItem()
    {
        if (CbColors.SelectedIndex != -1)
        {
            processingUiCultureChanged = true;

            var backupIndex = CbColors.SelectedIndex;
            if (CbColors.Items.Count > backupIndex + 1)
            {
                CbColors.SelectedIndex = backupIndex + 1;
            }
            else
            {
                CbColors.SelectedIndex = backupIndex - 1;
            }

            CbColors.SelectedIndex = backupIndex;

            processingUiCultureChanged = false;
        }
    }

    private static ColorItem CreateBlankColorItem()
        => new(
            ((int)DropDownFirstItemType.Blank).ToString(GlobalizationConstants.EnglishCultureInfo),
            string.Empty,
            string.Empty,
            Brushes.Transparent,
            Brushes.Transparent);

    private static ColorItem CreateColorItem(
        DropDownFirstItemType dropDownFirstItemType)
        => new(
            ((int)dropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo),
            dropDownFirstItemType.GetDescription(),
            string.Empty,
            Brushes.Transparent,
            Brushes.Transparent);

    private ColorItem? GetDefaultColorItem()
    {
        ColorItem? defaultColorItem = null;
        if (!string.IsNullOrEmpty(DefaultColorName))
        {
            var countryItem = Items.FirstOrDefault(x => x.Key == DefaultColorName);
            if (countryItem is not null)
            {
                defaultColorItem = countryItem;
            }
        }

        return defaultColorItem;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => UpdateDataOnUiCultureChanged();

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var colorSelector = (WellKnownColorSelector)d;

        if (!colorSelector.processingUiCultureChanged)
        {
            colorSelector.SetSelectedIndexBySelectedKey();
        }
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (!processingUiCultureChanged &&
            e.AddedItems.Count == 1)
        {
            OnSelectionChanged(sender, (ColorItem)e.AddedItems[0]!);
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        ColorItem colorItem)
    {
        if (processingUiCultureChanged ||
            Items.SuppressOnChangedNotification)
        {
            return;
        }

        if (lastName is null)
        {
            if (colorItem.Key.StartsWith('-'))
            {
                return;
            }

            if (DefaultColorName is not null &&
                DefaultColorName == colorItem.Key)
            {
                return;
            }
        }
        else if (lastName == colorItem.Key)
        {
            return;
        }

        lastName = colorItem.Key;

        Debug.WriteLine($"WellKnownColorSelector - Change to: {colorItem.Key} ({colorItem.DisplayHexCode})");

        SelectorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<string?>(
                identifier: Guid.Empty.ToString(),
                oldValue: null,
                newValue: colorItem.Key));
    }
}