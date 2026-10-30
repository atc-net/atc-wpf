namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled multi-column multi-select drop-down (<see cref="MultiColumnComboBox"/>): a drop-down whose pop-up is a
/// sortable grid with a check-box column, a mandatory marker and validation text. When <c>IsMandatory</c> is set, at
/// least one key must be ticked; with <c>MaxSelectionCount</c>, no more keys than that.
/// </summary>
/// <remarks>
/// <see cref="Columns"/> is the inner drop-down's own collection: its columns are moved into the pop-up grid, so a
/// column instance belongs to this control only.
/// </remarks>
public partial class LabelMultiColumnComboBox : ILabelMultiColumnComboBox
{
    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = MultiColumnComboBox.ItemsSourceProperty.AddOwner(
        typeof(LabelMultiColumnComboBox));

    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = MultiColumnComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(LabelMultiColumnComboBox));

    /// <summary>
    /// The (dotted) property path of an item's key; empty uses the item's <see cref="object.ToString"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string keyMemberPath;

    /// <summary>
    /// The (dotted) property path of an item's display value; empty uses the item's <see cref="object.ToString"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string displayMemberPath;

    /// <summary>
    /// Whether, when <see cref="Columns"/> is empty, the grid shows one text column per public property of the items.
    /// </summary>
    [DependencyProperty]
    private bool autoGenerateColumns;

    /// <summary>
    /// The keys of the ticked items. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// The maximum number of keys; <see langword="null"/> for no limit. Keys set from code beyond the limit are kept,
    /// but make <see cref="IsValid"/> return <see langword="false"/>.
    /// </summary>
    [DependencyProperty]
    private int? maxSelectionCount;

    /// <summary>
    /// The (dotted) path of a <see cref="bool"/> property telling whether an item can be toggled.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string isEnabledMemberPath;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it above 10 items.
    /// </summary>
    [DependencyProperty]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the pop-up's filter box.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private string filterText;

    /// <summary>
    /// Comma-separated property paths the filter searches instead of the visible columns.
    /// </summary>
    [DependencyProperty]
    private string? filterMemberPaths;

    /// <summary>
    /// Whether the ticked rows are shown first when the pop-up opens.
    /// </summary>
    [DependencyProperty]
    private bool keepSelectedOnTop;

    /// <summary>
    /// The width of the pop-up; <see cref="double.NaN"/> (the default) fits the columns, at least the box width.
    /// </summary>
    [DependencyProperty(DefaultValue = double.NaN)]
    private double dropDownWidth;

    /// <summary>
    /// When ticks in the pop-up reach <see cref="SelectedKeys"/>: right away, or on OK.
    /// </summary>
    [DependencyProperty(DefaultValue = MultiSelectCommitMode.Immediate)]
    private MultiSelectCommitMode commitMode;

    /// <summary>
    /// What the closed box shows for the ticked items: the names, a removable chip per item, or only the count.
    /// </summary>
    [DependencyProperty(DefaultValue = MultiSelectDisplayMode.Text)]
    private MultiSelectDisplayMode displayMode;

    /// <summary>
    /// In chips mode, the most chips shown before the "+N" chip; <see langword="null"/> (the default) shows as many as fit.
    /// </summary>
    [DependencyProperty]
    private int? maxVisibleChips;

    /// <summary>
    /// In chips mode, whether the chips wrap over several lines instead of ending in "+N".
    /// </summary>
    [DependencyProperty]
    private bool chipsWrap;

    /// <summary>
    /// In chips mode, the template of a chip (data context: <see cref="MultiSelectChipItem"/>); <see langword="null"/>
    /// for the default small input chip.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? chipTemplate;

    /// <summary>
    /// The most names the summary lists before it switches to <see cref="CountFormat"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = 2)]
    private int maxNamesInSummary;

    /// <summary>
    /// The summary when every item is ticked; <see langword="null"/> (the default) for the translated "All".
    /// </summary>
    [DependencyProperty]
    private string? allText;

    /// <summary>
    /// The summary when nothing is ticked; empty (the default) shows <see cref="WatermarkText"/> instead.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string noneText;

    /// <summary>
    /// The summary format beyond <see cref="MaxNamesInSummary"/> items (<c>{0}</c> ticked, <c>{1}</c> items);
    /// <see langword="null"/> (the default) for the translated "{0} of {1} selected".
    /// </summary>
    [DependencyProperty]
    private string? countFormat;

    /// <summary>
    /// The watermark shown in the closed box when nothing is ticked.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up (a row, select all, a chip removal, OK, or an items change)
    /// and pass validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up and fail validation (a mandatory field with nothing ticked,
    /// or more keys than <see cref="MaxSelectionCount"/>).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelMultiColumnComboBox"/> class.
    /// </summary>
    public LabelMultiColumnComboBox()
    {
        InitializeComponent();

        PartComboBox = new MultiColumnComboBox();
        ((LabelContent)Content).Content = PartComboBox;

        SetCurrentValue(SelectedKeysProperty, new Atc.Wpf.Collections.ObservableCollectionEx<string>());

        // Options first, then SelectedKeys and ItemsSource last.
        Forward(MinHeightProperty, ContentMinHeightProperty);
        Forward(TextBoxHelper.WatermarkProperty, WatermarkTextProperty);
        Forward(MultiColumnComboBox.MaxDropDownHeightProperty, MaxDropDownHeightProperty);
        Forward(MultiColumnComboBox.KeyMemberPathProperty, KeyMemberPathProperty);
        Forward(MultiColumnComboBox.DisplayMemberPathProperty, DisplayMemberPathProperty);
        Forward(MultiColumnComboBox.AutoGenerateColumnsProperty, AutoGenerateColumnsProperty);
        Forward(MultiColumnComboBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(MultiColumnComboBox.IsEnabledMemberPathProperty, IsEnabledMemberPathProperty);
        Forward(MultiColumnComboBox.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(MultiColumnComboBox.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(MultiColumnComboBox.FilterMemberPathsProperty, FilterMemberPathsProperty);
        Forward(MultiColumnComboBox.KeepSelectedOnTopProperty, KeepSelectedOnTopProperty);
        Forward(MultiColumnComboBox.DropDownWidthProperty, DropDownWidthProperty);
        Forward(MultiColumnComboBox.CommitModeProperty, CommitModeProperty);
        Forward(MultiColumnComboBox.DisplayModeProperty, DisplayModeProperty);
        Forward(MultiColumnComboBox.MaxVisibleChipsProperty, MaxVisibleChipsProperty);
        Forward(MultiColumnComboBox.ChipsWrapProperty, ChipsWrapProperty);
        Forward(MultiColumnComboBox.ChipTemplateProperty, ChipTemplateProperty);
        Forward(MultiColumnComboBox.MaxNamesInSummaryProperty, MaxNamesInSummaryProperty);
        Forward(MultiColumnComboBox.AllTextProperty, AllTextProperty);
        Forward(MultiColumnComboBox.NoneTextProperty, NoneTextProperty);
        Forward(MultiColumnComboBox.CountFormatProperty, CountFormatProperty);
        Forward(MultiColumnComboBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(MultiColumnComboBox.ItemsSourceProperty, ItemsSourceProperty);

        PartComboBox.SelectedKeysChanged += OnComboBoxSelectedKeysChanged;
    }

    /// <summary>
    /// Gets or sets the objects shown as rows.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets the grid columns after the check-box column (the inner drop-down's own collection), as on
    /// <see cref="DataGrid"/>.
    /// </summary>
    public ObservableCollection<DataGridColumn> Columns => PartComboBox.Columns;

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; the grid scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets the inner multi-column drop-down.
    /// </summary>
    internal MultiColumnComboBox ComboBox => PartComboBox;

    private MultiColumnComboBox PartComboBox { get; }

    /// <inheritdoc />
    public override bool IsValid()
    {
        Validate();
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelMultiColumnComboBox)d;
        if (!string.IsNullOrEmpty(control.ValidationText))
        {
            control.Validate();
        }
    }

    private void Forward(
        DependencyProperty innerProperty,
        DependencyProperty outerProperty,
        BindingMode mode = BindingMode.OneWay)
        => PartComboBox.SetBinding(
            innerProperty,
            new Binding
            {
                Path = new PropertyPath(outerProperty),
                Source = this,
                Mode = mode,
            });

    private bool Validate()
    {
        var keyCount = SelectedKeys?.Distinct(StringComparer.Ordinal).Count() ?? 0;
        if (IsMandatory && keyCount == 0)
        {
            ValidationText = Validations.FieldIsRequired;
            return false;
        }

        if (MaxSelectionCount is { } max && keyCount > max)
        {
            ValidationText = string.Format(CultureInfo.CurrentCulture, Validations.SelectAtMost1, max);
            return false;
        }

        ValidationText = string.Empty;
        return true;
    }

    private void OnComboBoxSelectedKeysChanged(
        object? sender,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        var args = new ValueChangedEventArgs<IReadOnlyList<string>>(
            Identifier,
            e.OldValue,
            e.NewValue);

        if (Validate())
        {
            SelectedKeysChanged?.Invoke(this, args);
        }
        else
        {
            SelectedKeysChangedInvalid?.Invoke(this, args);
        }
    }
}