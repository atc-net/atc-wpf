namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled multi-select cascader (<see cref="CascaderComboBox"/>): a drop-down with one column per hierarchy level
/// and tri-state check boxes, a mandatory marker and validation text. When <c>IsMandatory</c> is set, at least one key
/// must be ticked; with <c>MaxSelectionCount</c>, no more keys than that.
/// </summary>
public partial class LabelCascaderComboBox : ILabelCascaderComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = CascaderComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(LabelCascaderComboBox));

    /// <summary>
    /// Identifies the <see cref="LoadChildrenAsync"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadChildrenAsyncProperty = CascaderComboBox.LoadChildrenAsyncProperty.AddOwner(
        typeof(LabelCascaderComboBox));

    /// <summary>
    /// The root nodes (the first column).
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<CheckTreeNode> nodes;

    /// <summary>
    /// The keys of the ticked nodes. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Which ticked nodes <see cref="SelectedKeys"/> holds: the top-most fully ticked nodes (the default), only leaves,
    /// or every ticked node.
    /// </summary>
    [DependencyProperty(DefaultValue = CheckedKeysStrategy.Parents)]
    private CheckedKeysStrategy checkedKeysStrategy;

    /// <summary>
    /// Whether parent and child check boxes are linked; <see langword="false"/> makes every node independent.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isCheckLinked;

    /// <summary>
    /// What makes a row active (its children shown in the next column): a click, or also resting the mouse on it.
    /// </summary>
    [DependencyProperty(DefaultValue = CascaderExpandTrigger.Click)]
    private CascaderExpandTrigger expandTrigger;

    /// <summary>
    /// The width of each column in the pop-up.
    /// </summary>
    [DependencyProperty(DefaultValue = 180.0)]
    private double columnWidth;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it above 20 nodes.
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
    /// The maximum number of keys; <see langword="null"/> for no limit. Keys set from code beyond the limit are kept,
    /// but make <see cref="IsValid"/> return <see langword="false"/>.
    /// </summary>
    [DependencyProperty]
    private int? maxSelectionCount;

    /// <summary>
    /// The separator between the node texts of a path.
    /// </summary>
    [DependencyProperty(DefaultValue = " / ")]
    private string pathSeparator;

    /// <summary>
    /// Whether the summary names and the chips show the full path of a node instead of its text.
    /// </summary>
    [DependencyProperty]
    private bool showFullPath;

    /// <summary>
    /// The template for the text part of a pop-up row; its data context is the <see cref="CheckTreeNode"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// What the closed box shows for the ticked nodes: the names, a removable chip per key, or only the count.
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
    /// The summary when every leaf is ticked; <see langword="null"/> (the default) for the translated "All".
    /// </summary>
    [DependencyProperty]
    private string? allText;

    /// <summary>
    /// The summary when nothing is ticked; empty (the default) shows <see cref="WatermarkText"/> instead.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string noneText;

    /// <summary>
    /// The summary format beyond <see cref="MaxNamesInSummary"/> keys (<c>{0}</c> ticked leaves, <c>{1}</c> leaves);
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
    /// Occurs when the ticked keys change through the pop-up (a tick, a chip removal, or loaded children) and pass
    /// validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up and fail validation (a mandatory field with nothing ticked,
    /// or more keys than <see cref="MaxSelectionCount"/>).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCascaderComboBox"/> class.
    /// </summary>
    public LabelCascaderComboBox()
    {
        InitializeComponent();

        PartComboBox = new CascaderComboBox();
        ((LabelContent)Content).Content = PartComboBox;

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(NodesProperty, new ObservableCollection<CheckTreeNode>());

        // Options first, then SelectedKeys and Nodes last.
        Forward(MinHeightProperty, ContentMinHeightProperty);
        Forward(TextBoxHelper.WatermarkProperty, WatermarkTextProperty);
        Forward(CascaderComboBox.MaxDropDownHeightProperty, MaxDropDownHeightProperty);
        Forward(CascaderComboBox.MaxNamesInSummaryProperty, MaxNamesInSummaryProperty);
        Forward(CascaderComboBox.AllTextProperty, AllTextProperty);
        Forward(CascaderComboBox.NoneTextProperty, NoneTextProperty);
        Forward(CascaderComboBox.CountFormatProperty, CountFormatProperty);
        Forward(CascaderComboBox.CheckedKeysStrategyProperty, CheckedKeysStrategyProperty);
        Forward(CascaderComboBox.IsCheckLinkedProperty, IsCheckLinkedProperty);
        Forward(CascaderComboBox.ExpandTriggerProperty, ExpandTriggerProperty);
        Forward(CascaderComboBox.ColumnWidthProperty, ColumnWidthProperty);
        Forward(CascaderComboBox.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(CascaderComboBox.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(CascaderComboBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(CascaderComboBox.PathSeparatorProperty, PathSeparatorProperty);
        Forward(CascaderComboBox.ShowFullPathProperty, ShowFullPathProperty);
        Forward(CascaderComboBox.ItemTemplateProperty, ItemTemplateProperty);
        Forward(CascaderComboBox.DisplayModeProperty, DisplayModeProperty);
        Forward(CascaderComboBox.MaxVisibleChipsProperty, MaxVisibleChipsProperty);
        Forward(CascaderComboBox.ChipsWrapProperty, ChipsWrapProperty);
        Forward(CascaderComboBox.ChipTemplateProperty, ChipTemplateProperty);
        Forward(CascaderComboBox.LoadChildrenAsyncProperty, LoadChildrenAsyncProperty);
        Forward(CascaderComboBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(CascaderComboBox.NodesProperty, NodesProperty);

        PartComboBox.SelectedKeysChanged += OnComboBoxSelectedKeysChanged;
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; each column scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the callback that loads the children of a node with <see cref="CheckTreeNode.HasUnloadedChildren"/>
    /// when the node becomes active; children of a ticked node inherit the tick.
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>? LoadChildrenAsync
    {
        get => (Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?)GetValue(LoadChildrenAsyncProperty);
        set => SetValue(LoadChildrenAsyncProperty, value);
    }

    /// <summary>
    /// Gets the inner cascader drop-down.
    /// </summary>
    internal CascaderComboBox ComboBox => PartComboBox;

    private CascaderComboBox PartComboBox { get; }

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
        var control = (LabelCascaderComboBox)d;
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