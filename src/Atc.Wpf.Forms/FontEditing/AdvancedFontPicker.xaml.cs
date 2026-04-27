namespace Atc.Wpf.Forms.FontEditing;

public partial class AdvancedFontPicker
{
    private static readonly double[] DefaultSizes =
    [
        8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72,
    ];

    private static readonly IReadOnlyList<FontFamily> SortedSystemFontFamilies =
        Fonts.SystemFontFamilies
            .OrderBy(f => f.Source, StringComparer.OrdinalIgnoreCase)
            .ToList();

    [DependencyProperty(DefaultValue = true)]
    private bool showFontFamily;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontSize;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontWeight;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStyle;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStretch;

    [DependencyProperty(DefaultValue = true)]
    private bool showForegroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showBackgroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showTextDecorations;

    [DependencyProperty(DefaultValue = true)]
    private bool showQuickToggles;

    [DependencyProperty(DefaultValue = true)]
    private bool showPreview;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontFamilyEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontSizeEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontWeightEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStyleEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStretchEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isForegroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isBackgroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isTextDecorationsEnabled;

    [DependencyProperty(DefaultValue = FontColorEditorMode.ColorPicker)]
    private FontColorEditorMode colorEditorMode;

    [DependencyProperty(
        DefaultValue = "The quick brown fox jumps over the lazy dog 0123456789")]
    private string previewText;

    [DependencyProperty(
        DefaultValue = "Segoe UI",
        PropertyChangedCallback = nameof(OnSelectedFontFamilyChanged))]
    private FontFamily? selectedFontFamily;

    [DependencyProperty(
        DefaultValue = FontDescription.DefaultSize,
        PropertyChangedCallback = nameof(OnSelectedFontSizeChanged))]
    private double selectedFontSize;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectedFontWeightChanged))]
    private FontWeight selectedFontWeight;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectedFontStyleChanged))]
    private FontStyle selectedFontStyle;

    [DependencyProperty]
    private FontStretch selectedFontStretch;

    [DependencyProperty(DefaultValue = "Black")]
    private SolidColorBrush? selectedForegroundBrush;

    [DependencyProperty(DefaultValue = "Transparent")]
    private SolidColorBrush? selectedBackgroundBrush;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectedTextDecorationsChanged))]
    private TextDecorationCollection? selectedTextDecorations;

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnFamilyFilterChanged))]
    private string familyFilter;

    private ICollectionView? familyView;

    public AdvancedFontPicker()
    {
        AvailableFontFamilies = new ObservableCollection<FontFamily>(SortedSystemFontFamilies);

        AvailableFontSizes = new ObservableCollection<double>(DefaultSizes);

        AvailableFontWeights = new ObservableCollection<FontWeight>();
        AvailableFontStyles = new ObservableCollection<FontStyle>();
        AvailableFontStretches = new ObservableCollection<FontStretch>();

        RecentFontFamilies = new ObservableCollection<FontFamily>();

        SetCurrentValue(SelectedFontWeightProperty, FontWeights.Normal);
        SetCurrentValue(SelectedFontStyleProperty, FontStyles.Normal);
        SetCurrentValue(SelectedFontStretchProperty, FontStretches.Normal);
        SetCurrentValue(SelectedTextDecorationsProperty, new TextDecorationCollection());

        InitializeComponent();

        familyView = CollectionViewSource.GetDefaultView(AvailableFontFamilies);
        familyView.Filter = FilterFamily;

        Loaded += OnLoaded;
    }

    public ObservableCollection<FontFamily> AvailableFontFamilies { get; }

    public ObservableCollection<double> AvailableFontSizes { get; }

    public ObservableCollection<FontWeight> AvailableFontWeights { get; }

    public ObservableCollection<FontStyle> AvailableFontStyles { get; }

    public ObservableCollection<FontStretch> AvailableFontStretches { get; }

    public ObservableCollection<FontFamily> RecentFontFamilies { get; }

    /// <summary>
    /// Records the currently selected font family in the configured
    /// <see cref="IFontPickerStorage"/>, then refreshes the in-control recents list.
    /// Call this after the user has confirmed their selection (e.g. on dialog OK).
    /// </summary>
    public void RecordCurrentFamilyAsRecent()
    {
        if (SelectedFontFamily is null)
        {
            return;
        }

        FontPickerStorage.Current.RecordRecentFontFamily(SelectedFontFamily.Source);
        RefreshRecentFontFamilies();
    }

    private void RefreshRecentFontFamilies()
    {
        var sources = FontPickerStorage.Current.GetRecentFontFamilies();
        var resolved = sources
            .Select(name => SortedSystemFontFamilies.FirstOrDefault(
                f => string.Equals(f.Source, name, StringComparison.OrdinalIgnoreCase)))
            .Where(f => f is not null)
            .Cast<FontFamily>()
            .ToList();

        if (resolved.Count == RecentFontFamilies.Count
            && resolved.SequenceEqual(RecentFontFamilies))
        {
            return;
        }

        RecentFontFamilies.Clear();
        foreach (var family in resolved)
        {
            RecentFontFamilies.Add(family);
        }
    }

    [SuppressMessage("Performance", "CA1024:Use properties where appropriate", Justification = "Returns a new instance per call.")]
    public FontDescription GetFontDescription()
        => new(
            SelectedFontFamily ?? new FontFamily("Segoe UI"),
            SelectedFontSize,
            SelectedFontWeight,
            SelectedFontStyle,
            SelectedFontStretch,
            SelectedForegroundBrush,
            SelectedBackgroundBrush,
            SelectedTextDecorations);

    public void SetFontDescription(FontDescription fontDescription)
    {
        ArgumentNullException.ThrowIfNull(fontDescription);

        SetCurrentValue(SelectedFontFamilyProperty, fontDescription.Family);
        SetCurrentValue(SelectedFontSizeProperty, fontDescription.Size);
        SetCurrentValue(SelectedFontWeightProperty, fontDescription.Weight);
        SetCurrentValue(SelectedFontStyleProperty, fontDescription.Style);
        SetCurrentValue(SelectedFontStretchProperty, fontDescription.Stretch);

        if (fontDescription.Foreground is not null)
        {
            SetCurrentValue(SelectedForegroundBrushProperty, fontDescription.Foreground);
        }

        if (fontDescription.Background is not null)
        {
            SetCurrentValue(SelectedBackgroundBrushProperty, fontDescription.Background);
        }

        if (fontDescription.TextDecorations is not null)
        {
            SetCurrentValue(SelectedTextDecorationsProperty, fontDescription.TextDecorations);
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        ThemeBrushHelper.ApplyDefaults(
            this,
            SelectedForegroundBrushProperty,
            SelectedBackgroundBrushProperty);
        SyncQuickToggleStates();
        RefreshRecentFontFamilies();
    }

    private void SyncQuickToggleStates()
    {
        if (BoldToggle is not null)
        {
            BoldToggle.IsChecked = SelectedFontWeight == FontWeights.Bold;
        }

        if (ItalicToggle is not null)
        {
            ItalicToggle.IsChecked = SelectedFontStyle == FontStyles.Italic;
        }

        if (UnderlineToggle is not null)
        {
            UnderlineToggle.IsChecked = ContainsDecoration(System.Windows.TextDecorations.Underline);
        }

        if (StrikethroughToggle is not null)
        {
            StrikethroughToggle.IsChecked = ContainsDecoration(System.Windows.TextDecorations.Strikethrough);
        }
    }

    private bool ContainsDecoration(TextDecorationCollection decoration)
    {
        if (SelectedTextDecorations is null || decoration.Count == 0)
        {
            return false;
        }

        var targetLocation = decoration[0].Location;
        for (var i = 0; i < SelectedTextDecorations.Count; i++)
        {
            if (SelectedTextDecorations[i].Location == targetLocation)
            {
                return true;
            }
        }

        return false;
    }

    private void OnBoldToggleClick(
        object sender,
        RoutedEventArgs e)
        => SetCurrentValue(
            SelectedFontWeightProperty,
            BoldToggle.IsChecked == true ? FontWeights.Bold : FontWeights.Normal);

    private void OnItalicToggleClick(
        object sender,
        RoutedEventArgs e)
        => SetCurrentValue(
            SelectedFontStyleProperty,
            ItalicToggle.IsChecked == true ? FontStyles.Italic : FontStyles.Normal);

    private void OnUnderlineToggleClick(
        object sender,
        RoutedEventArgs e)
        => UpdateDecoration(
            System.Windows.TextDecorations.Underline,
            UnderlineToggle.IsChecked == true);

    private void OnStrikethroughToggleClick(
        object sender,
        RoutedEventArgs e)
        => UpdateDecoration(
            System.Windows.TextDecorations.Strikethrough,
            StrikethroughToggle.IsChecked == true);

    private void UpdateDecoration(
        TextDecorationCollection decoration,
        bool add)
    {
        var current = SelectedTextDecorations is null
            ? new TextDecorationCollection()
            : new TextDecorationCollection(SelectedTextDecorations);

        var targetLocation = decoration[0].Location;

        for (var i = current.Count - 1; i >= 0; i--)
        {
            if (current[i].Location == targetLocation)
            {
                current.RemoveAt(i);
            }
        }

        if (add)
        {
            foreach (var d in decoration)
            {
                current.Add(d);
            }
        }

        SetCurrentValue(SelectedTextDecorationsProperty, current);
    }

    private static void OnSelectedFontWeightChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).SyncQuickToggleStates();

    private static void OnSelectedFontStyleChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).SyncQuickToggleStates();

    private static void OnSelectedTextDecorationsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).SyncQuickToggleStates();

    private static void OnSelectedFontFamilyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).RefreshTypefaceLists();

    /// <summary>
    /// Pushes the user-selected family to <see cref="SelectedFontFamily"/>.
    /// Ignores selection-cleared events (e.g. caused by the family filter excluding
    /// the current selection from the visible items) so the selection survives filtering.
    /// </summary>
    private void OnFamilyListSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox)
        {
            return;
        }

        if (listBox.SelectedItem is FontFamily family)
        {
            SetCurrentValue(SelectedFontFamilyProperty, family);
        }
    }

    // The Weight/Style/Stretch ListBox SelectedItem bindings are OneWay (not TwoWay)
    // so that transient null writes during ReplaceCollection (Clear+Add) do not raise
    // ValidationError on the non-nullable struct DPs (which would draw WPF's red error
    // border around the ListBox until the user clicked an item).
    private void OnWeightListSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is FontWeight weight)
        {
            SetCurrentValue(SelectedFontWeightProperty, weight);
        }
    }

    private void OnStyleListSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is FontStyle style)
        {
            SetCurrentValue(SelectedFontStyleProperty, style);
        }
    }

    private void OnStretchListSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is FontStretch stretch)
        {
            SetCurrentValue(SelectedFontStretchProperty, stretch);
        }
    }

    private static void OnSelectedFontSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).EnsureSizeInList();

    private static void OnFamilyFilterChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AdvancedFontPicker)d).familyView?.Refresh();

    private bool FilterFamily(object item)
    {
        if (string.IsNullOrWhiteSpace(FamilyFilter))
        {
            return true;
        }

        return item is FontFamily family
            && family.Source.Contains(FamilyFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureSizeInList()
    {
        if (SelectedFontSize <= 0)
        {
            return;
        }

        if (AvailableFontSizes.Contains(SelectedFontSize))
        {
            return;
        }

        var insertIndex = 0;
        while (insertIndex < AvailableFontSizes.Count
               && AvailableFontSizes[insertIndex] < SelectedFontSize)
        {
            insertIndex++;
        }

        AvailableFontSizes.Insert(insertIndex, SelectedFontSize);
    }

    private void RefreshTypefaceLists()
    {
        if (SelectedFontFamily is null)
        {
            return;
        }

        var typefaces = SelectedFontFamily.FamilyTypefaces;
        if (typefaces.Count == 0)
        {
            ReplaceCollection(AvailableFontWeights, [FontWeights.Normal]);
            ReplaceCollection(AvailableFontStyles, [FontStyles.Normal]);
            ReplaceCollection(AvailableFontStretches, [FontStretches.Normal]);
        }
        else
        {
            ReplaceCollection(
                AvailableFontWeights,
                typefaces.Select(t => t.Weight)
                    .Distinct()
                    .OrderBy(w => w.ToOpenTypeWeight()));

            ReplaceCollection(
                AvailableFontStyles,
                typefaces.Select(t => t.Style)
                    .Distinct()
                    .OrderBy(s => s.ToString(), StringComparer.OrdinalIgnoreCase));

            ReplaceCollection(
                AvailableFontStretches,
                typefaces.Select(t => t.Stretch)
                    .Distinct()
                    .OrderBy(s => s.ToOpenTypeStretch()));
        }

        SnapWeightIfInvalid();
        SnapStyleIfInvalid();
        SnapStretchIfInvalid();
    }

    private void SnapWeightIfInvalid()
    {
        if (AvailableFontWeights.Count == 0 ||
            AvailableFontWeights.Contains(SelectedFontWeight))
        {
            return;
        }

        var current = SelectedFontWeight.ToOpenTypeWeight();
        var closest = AvailableFontWeights
            .OrderBy(w => System.Math.Abs(w.ToOpenTypeWeight() - current))
            .First();

        SetCurrentValue(SelectedFontWeightProperty, closest);
    }

    private void SnapStyleIfInvalid()
    {
        if (AvailableFontStyles.Count == 0 ||
            AvailableFontStyles.Contains(SelectedFontStyle))
        {
            return;
        }

        SetCurrentValue(SelectedFontStyleProperty, AvailableFontStyles[0]);
    }

    private void SnapStretchIfInvalid()
    {
        if (AvailableFontStretches.Count == 0 ||
            AvailableFontStretches.Contains(SelectedFontStretch))
        {
            return;
        }

        var current = SelectedFontStretch.ToOpenTypeStretch();
        var closest = AvailableFontStretches
            .OrderBy(s => System.Math.Abs(s.ToOpenTypeStretch() - current))
            .First();

        SetCurrentValue(SelectedFontStretchProperty, closest);
    }

    /// <summary>
    /// Replaces the contents of an ObservableCollection in place,
    /// but only if the new sequence differs from the current contents.
    /// Avoids unnecessary Clear/Add churn that triggers transient writes
    /// through TwoWay bindings to struct DPs.
    /// </summary>
    private static void ReplaceCollection<T>(
        ObservableCollection<T> target,
        IEnumerable<T> source)
    {
        var newItems = source as IList<T> ?? source.ToList();
        if (newItems.Count == target.Count
            && newItems.SequenceEqual(target))
        {
            return;
        }

        target.Clear();
        foreach (var item in newItems)
        {
            target.Add(item);
        }
    }
}