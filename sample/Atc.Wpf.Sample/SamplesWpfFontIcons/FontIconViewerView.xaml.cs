namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

/// <summary>
/// Interaction logic for FontIconViewerView.
/// </summary>
public partial class FontIconViewerView : INotifyPropertyChanged
{
    private const int FilterDebounceDelayMs = 500;
    private readonly DebounceDispatcher debounceDispatcher = new();
    private readonly FontIconImageSourceValueConverter fontConverter;
    private readonly Dictionary<string, string> fontAwesomeBrandList;
    private readonly Dictionary<string, string> fontAwesomeRegularList;
    private readonly Dictionary<string, string> fontAwesomeSolidList;
    private readonly Dictionary<string, string> fontBootstrapList;
    private readonly Dictionary<string, string> fontIcoFontList;
    private readonly Dictionary<string, string> fontMaterialDesignList;
    private readonly Dictionary<string, string> fontWeatherList;
    private bool isBusy;

    public FontIconViewerView()
    {
        InitializeComponent();

        DataContext = this;

        fontConverter = new FontIconImageSourceValueConverter();
        fontAwesomeBrandList = Enum<FontAwesomeBrandType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontAwesomeRegularList = Enum<FontAwesomeRegularType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontAwesomeSolidList = Enum<FontAwesomeSolidType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontBootstrapList = Enum<FontBootstrapType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontIcoFontList = Enum<IcoFontType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontMaterialDesignList = Enum<FontMaterialDesignType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);
        fontWeatherList = Enum<FontWeatherType>.ToDictionaryWithStringKey(useDescriptionAttribute: true, includeDefault: false);

        FilterFontAwesomeBrand.LabelText += $" ({fontAwesomeBrandList.Count})";
        FilterFontAwesomeRegular.LabelText += $" ({fontAwesomeRegularList.Count})";
        FilterFontAwesomeSolid.LabelText += $" ({fontAwesomeSolidList.Count})";
        FilterBootstrap.LabelText += $" ({fontBootstrapList.Count})";
        FilterIcoFont.LabelText += $" ({fontIcoFontList.Count})";
        FilterMaterialDesign.LabelText += $" ({fontMaterialDesignList.Count})";
        FilterWeather.LabelText += $" ({fontWeatherList.Count})";

        Loaded += OnLoaded;
    }

    public bool IsBusy
    {
        get => isBusy;
        set
        {
            isBusy = value;
            OnPropertyChanged();
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        PopulateListOfIcons();
    }

    private void IconColorPickerOnSelectorChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        if (ListOfIcons is null ||
            ListOfIcons.Children.Count == 0)
        {
            return;
        }

        IsBusy = true;

        _ = Application.Current.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                ListOfIcons.Children.Clear();

                UiPopulateListOfIcons();

                UiFilterListOfIcons(FilterText.Text);

                IsBusy = false;
            }),
            DispatcherPriority.Background);
    }

    private void FilterCheckBoxOnIsCheckedChanged(
        object sender,
        ChangedBooleanEventArgs e)
    {
        if (ListOfIcons is null ||
            ListOfIcons.Children.Count == 0)
        {
            return;
        }

        debounceDispatcher.Debounce(FilterDebounceDelayMs, _ =>
        {
            FilterListOfIcons(FilterText.Text);
        });
    }

    private void FilterOnTextChanged(
        object sender,
        RoutedPropertyChangedEventArgs<string> e)
    {
        if (ListOfIcons is null ||
            ListOfIcons.Children.Count == 0)
        {
            return;
        }

        debounceDispatcher.Debounce(FilterDebounceDelayMs, _ =>
        {
            FilterListOfIcons(e.NewValue);
        });
    }

    private void PopulateListOfIcons()
    {
        IsBusy = true;

        _ = Application.Current.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                UiPopulateListOfIcons();

                IsBusy = false;
            }),
            DispatcherPriority.Background);
    }

    private void UiPopulateListOfIcons()
    {
        var brushColor = (SolidColorBrush)new BrushConverter().ConvertFromString(IconColorPicker.SelectedKey)!;

        foreach (var pair in fontAwesomeBrandList)
        {
            var key = $"{nameof(FontAwesomeBrandType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontAwesomeBrandType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontAwesomeRegularList)
        {
            var key = $"{nameof(FontAwesomeRegularType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontAwesomeRegularType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontAwesomeSolidList)
        {
            var key = $"{nameof(FontAwesomeSolidType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontAwesomeSolidType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontBootstrapList)
        {
            var key = $"{nameof(FontBootstrapType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontBootstrapType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontIcoFontList)
        {
            var key = $"{nameof(IcoFontType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<IcoFontType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontMaterialDesignList)
        {
            var key = $"{nameof(FontMaterialDesignType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontMaterialDesignType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        foreach (var pair in fontWeatherList)
        {
            var key = $"{nameof(FontWeatherType)}_{pair.Key}";
            var drawingImage = (fontConverter.Convert(Enum<FontWeatherType>.Parse(pair.Key), targetType: null, brushColor, culture: null) as DrawingImage)!;
            AddToListOfIcon(key, new ImageSet(drawingImage, pair.Key.Humanize(), pair.Value));
        }

        UpdateCountListOfIcons();
    }

    private void FilterListOfIcons(
        string filterText)
    {
        if (IsBusy || ListOfIcons.Children.Count == 0)
        {
            return;
        }

        IsBusy = true;

        _ = Application.Current.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                UiFilterListOfIcons(filterText);

                IsBusy = false;
            }),
            DispatcherPriority.Background);
    }

    private void UiFilterListOfIcons(
        string filterText)
    {
        filterText = filterText
            .Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLower(GlobalizationConstants.EnglishCultureInfo);

        FilterIcons(nameof(FontAwesomeBrandType), fontAwesomeBrandList, FilterFontAwesomeBrand.IsChecked, filterText);
        FilterIcons(nameof(FontAwesomeRegularType), fontAwesomeRegularList, FilterFontAwesomeRegular.IsChecked, filterText);
        FilterIcons(nameof(FontAwesomeSolidType), fontAwesomeSolidList, FilterFontAwesomeSolid.IsChecked, filterText);
        FilterIcons(nameof(FontBootstrapType), fontBootstrapList, FilterBootstrap.IsChecked, filterText);
        FilterIcons(nameof(IcoFontType), fontIcoFontList, FilterIcoFont.IsChecked, filterText);
        FilterIcons(nameof(FontMaterialDesignType), fontMaterialDesignList, FilterMaterialDesign.IsChecked, filterText);
        FilterIcons(nameof(FontWeatherType), fontWeatherList, FilterWeather.IsChecked, filterText);

        UpdateCountListOfIcons();
    }

    private void UpdateCountListOfIcons()
    {
        var displayCount = ListOfIcons.Children.Cast<FrameworkElement>().Count(x => x.Visibility == Visibility.Visible);
        CountListOfIcons.Text = $"Show {displayCount} of {ListOfIcons.Children.Count} icons";
    }

    [SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "OK.")]
    private void FilterIcons(
        string keyPrefix,
        Dictionary<string, string> list,
        bool showIt,
        string filterText)
    {
        var frameworkElementsForKeyPrefix = ListOfIcons.Children
            .Cast<FrameworkElement>()
            .Where(x => x.Tag is not null &&
                        x.Tag.ToString()!.StartsWith(keyPrefix, StringComparison.Ordinal))
            .ToList();

        foreach (var pair in list)
        {
            var tagKey = $"{keyPrefix}_{pair.Key}";
            foreach (var item in frameworkElementsForKeyPrefix)
            {
                if (!tagKey.Equals(item.Tag.ToString(), StringComparison.Ordinal))
                {
                    continue;
                }

                if (showIt)
                {
                    if (filterText.Length == 0)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    else if (pair.Key.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    private void AddToListOfIcon(
        string key,
        ImageSet imageSet)
    {
        var sp = new StackPanel
        {
            Tag = key,
            Orientation = Orientation.Vertical,
            Width = 150,
            Margin = new Thickness(20),
            ToolTip = imageSet.ToolTip,
        };

        var image = new Image
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 50,
            Height = 50,
            Source = imageSet.ImageSource,
        };

        var textBlock = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = imageSet.Label,
        };

        sp.Children.Add(image);
        sp.Children.Add(textBlock);

        ListOfIcons.Children.Add(sp);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}