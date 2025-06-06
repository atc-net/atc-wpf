namespace Atc.Wpf.Controls.Selectors;

public partial class FontFamilySelector
{
    private readonly ObservableCollectionEx<FontFamilyItem> items = new();
    private string? lastKey;

    [DependencyProperty(DefaultValue = DropDownFirstItemType.None)]
    private DropDownFirstItemType dropDownFirstItemType;

    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnSelectedKeyChanged))]
    private string selectedKey;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public FontFamilySelector()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
    }

    public ObservableCollectionEx<FontFamilyItem> Items => items;

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

        switch (DropDownFirstItemType)
        {
            case DropDownFirstItemType.None:
                break;
            case DropDownFirstItemType.Blank:
            case DropDownFirstItemType.PleaseSelect:
            case DropDownFirstItemType.IncludeAll:
                Items.Add(
                    new FontFamilyItem(
                        ((int)DropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo),
                        "Arial",
                        DropDownFirstItemType.GetDescription()));

                SelectedKey = ((int)DropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo);
                break;
            default:
                throw new SwitchCaseDefaultException(DropDownFirstItemType);
        }

        foreach (var fontFamily in Fonts.SystemFontFamilies)
        {
            Items.Add(
                new FontFamilyItem(
                    Key: fontFamily.Source,
                    FontFamily: fontFamily.Source,
                    DisplayName: fontFamily.Source));
        }

        SortItems();

        if (!string.IsNullOrEmpty(SelectedKey))
        {
            SetSelectedIndexBySelectedKey();
        }

        Items.SuppressOnChangedNotification = false;
    }

    private void SortItems()
    {
        var sortedList = Items
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .ToList();

        items.Clear();

        items.AddRange(sortedList);
    }

    private void SetSelectedIndexBySelectedKey()
    {
        for (var i = 0; i < CbFonts.Items.Count; i++)
        {
            var item = (FontFamilyItem)CbFonts.Items[i]!;
            if (SelectedKey == item.Key)
            {
                CbFonts.SelectedIndex = i;
                OnSelectionChanged(this, item);

                break;
            }
        }
    }

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var fontFamilySelector = (FontFamilySelector)d;

        fontFamilySelector.SetSelectedIndexBySelectedKey();
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            OnSelectionChanged(sender, (FontFamilyItem)e.AddedItems[0]!);
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK.")]
    private void OnSelectionChanged(
        object sender,
        FontFamilyItem fontFamilyItem)
    {
        if (Items.SuppressOnChangedNotification)
        {
            return;
        }

        if (lastKey is not null &&
            lastKey == fontFamilyItem.Key)
        {
            return;
        }

        lastKey = fontFamilyItem.Key;

        Debug.WriteLine($"FontFamilySelector - Change to: {fontFamilyItem.Key} ({fontFamilyItem.DisplayName})");

        SelectorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<string?>(
                identifier: Guid.Empty.ToString(),
                oldValue: null,
                newValue: fontFamilyItem.DisplayName));
    }
}