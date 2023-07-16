// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable ConvertToAutoPropertyWhenPossible
namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for WellKnownColorSelector.
/// </summary>
public partial class WellKnownColorSelector : INotifyPropertyChanged
{
    private readonly ObservableCollectionEx<ColorItem> items = new();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<ChangedStringEventArgs>? SelectorChanged;

    public WellKnownColorSelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public ObservableCollectionEx<ColorItem> Items => items;

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

        var colorsInfo = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var itemName in colorsInfo.Select(x => x.Name))
        {
            var translatedName = ColorNames.ResourceManager.GetString(
                itemName,
                CultureInfo.CurrentUICulture);

            var color = (Color)ColorConverter.ConvertFromString(itemName);
            var showcaseBrush = new SolidColorBrush(color);

            Items.Add(
                new ColorItem(
                    itemName,
                    translatedName ?? "#" + itemName,
                    color.ToString(GlobalizationConstants.EnglishCultureInfo),
                    showcaseBrush,
                    showcaseBrush));
        }

        if (string.IsNullOrEmpty(SelectedKey))
        {
            SelectedKey = GetDefaultColorItem()?.Name ??
                          Items[0].Name;
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

        var colorsInfo = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var item in Items)
        {
            if (!NumberHelper.IsInt(item.Name))
            {
                var colorInfo = colorsInfo.SingleOrDefault(x => x.Name == item.Name);
                if (colorInfo is not null)
                {
                    var translatedName = ColorNames.ResourceManager.GetString(
                        colorInfo.Name,
                        CultureInfo.CurrentUICulture)!;

                    item.DisplayName = translatedName;
                }
            }
            else
            {
                var dropDownFirstItemType = (DropDownFirstItemType)NumberHelper.ParseToInt(item.Name);
                switch (dropDownFirstItemType)
                {
                    case DropDownFirstItemType.None:
                    case DropDownFirstItemType.Blank:
                        break;
                    case DropDownFirstItemType.PleaseSelect:
                    case DropDownFirstItemType.IncludeAll:
                        item.DisplayName = dropDownFirstItemType.GetDescription();
                        break;
                    default:
                        throw new SwitchCaseDefaultException(dropDownFirstItemType);
                }
            }
        }

        SortItems();

        if (string.IsNullOrEmpty(SelectedKey))
        {
            SelectedKey = GetDefaultColorItem()?.Name ??
                          Items[0].Name;
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
        var firstItem = Items.FirstOrDefault(x => x.Name.StartsWith('-'));
        var sortedList = Items
            .Where(x => !x.Name.StartsWith('#'))
            .OrderBy(x => x.DisplayName, StringComparer.Ordinal)
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

        for (var i = 0; i < CbColors.Items.Count; i++)
        {
            var item = (ColorItem)CbColors.Items[i];
            if (SelectedKey == item.Name.ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                CbColors.SelectedIndex = i;
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
        if (CbColors.SelectedIndex != -1)
        {
            var backupIndex = CbColors.SelectedIndex;
            if (CbColors.Items.Count > backupIndex)
            {
                CbColors.SelectedIndex = backupIndex + 1;
            }
            else
            {
                CbColors.SelectedIndex = backupIndex - 1;
            }

            CbColors.SelectedIndex = backupIndex;
        }
    }

    private static ColorItem CreateBlankColorItem()
        => new(
            ((int)DropDownFirstItemType.Blank).ToString(GlobalizationConstants.EnglishCultureInfo),
            string.Empty,
            "#",
            Brushes.Pink,
            Brushes.Pink);

    private static ColorItem CreateColorItem(
        DropDownFirstItemType dropDownFirstItemType)
        => new(
            ((int)dropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo),
            dropDownFirstItemType.GetDescription(),
            "#",
            Brushes.Pink,
            Brushes.Pink);

    private ColorItem? GetDefaultColorItem()
    {
        ColorItem? defaultColorItem = null;
        //if (!string.IsNullOrEmpty(DefaultCultureIdentifier))
        //{
        //    var countryItem = NumberHelper.IsInt(DefaultCultureIdentifier)
        //        ? Items.FirstOrDefault(x => x.Culture.Lcid == NumberHelper.ParseToInt(DefaultCultureIdentifier))
        //        : Items.FirstOrDefault(x => x.Culture.Name == DefaultCultureIdentifier);

        //    if (countryItem is not null)
        //    {
        //        defaultColorItem = countryItem;
        //    }
        //}

        return defaultColorItem;
    }

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var colorSelector = (WellKnownColorSelector)d;

        colorSelector.SetSelectedIndexBySelectedKey();
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        => OnSelectionChanged(sender, (ColorItem)e.AddedItems[0]!);

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        ColorItem colorItem)
    {
        if (processingUiCultureChanged ||
            Items.SuppressOnChangedNotification)
        {
            return;
        }

        Debug.WriteLine($"ColorSelector - Change to: {colorItem.Name} ({colorItem.DisplayHexCode})");

        SelectorChanged?.Invoke(
            this,
            new ChangedStringEventArgs(
                identifier: Guid.Empty.ToString(),
                oldValue: null,
                newValue: colorItem.Name));
    }
}