namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled multi-select drop-down (<see cref="MultiSelectComboBox"/>) with a mandatory marker and validation text.
/// When <c>IsMandatory</c> is set, at least one key must be ticked.
/// </summary>
public partial class LabelMultiSelectComboBox : ILabelMultiSelectComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = MultiSelectComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(LabelMultiSelectComboBox));

    /// <summary>
    /// The items, keyed by their selection key, with their display value.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the ticked items. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Whether keys that appear in <see cref="Items"/> are ticked automatically.
    /// </summary>
    [DependencyProperty]
    private bool selectNewItems;

    /// <summary>
    /// Whether the pop-up shows the top row with the tri-state "Select all" check box and the count.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectAllRow;

    /// <summary>
    /// Whether the pop-up shows a "Clear" button in the "Select all" row.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showClearButton;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it when there are more than 10 items.
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
    /// The group of each key (key to group name); <see langword="null"/> for no groups.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string>? itemGroups;

    /// <summary>
    /// Whether a group header in the pop-up has a tri-state check box that ticks or unticks its shown items.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isGroupCheckable;

    /// <summary>
    /// The maximum number of ticked keys; <see langword="null"/> for no limit.
    /// </summary>
    [DependencyProperty]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys whose items are shown but cannot be toggled.
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<string> disabledKeys;

    /// <summary>
    /// The template for the text part of a pop-up row; its data context is the <see cref="CheckListItem"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// The most display values the summary lists before it switches to <see cref="CountFormat"/>.
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
    /// The summary format when more than <see cref="MaxNamesInSummary"/> items (but not all) are ticked;
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
    /// Occurs when the ticked keys change through the pop-up (a tick, Select all, Clear, a group, or an
    /// <see cref="Items"/> change that drops or adds keys) and pass validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up and fail validation (a mandatory field with nothing ticked).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelMultiSelectComboBox"/> class.
    /// </summary>
    public LabelMultiSelectComboBox()
    {
        InitializeComponent();

        PartComboBox = new MultiSelectComboBox();
        ((LabelContent)Content).Content = PartComboBox;

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        // Options first, then SelectNewItems, SelectedKeys and Items last.
        Forward(MinHeightProperty, ContentMinHeightProperty);
        Forward(TextBoxHelper.WatermarkProperty, WatermarkTextProperty);
        Forward(MultiSelectComboBox.MaxDropDownHeightProperty, MaxDropDownHeightProperty);
        Forward(MultiSelectComboBox.MaxNamesInSummaryProperty, MaxNamesInSummaryProperty);
        Forward(MultiSelectComboBox.AllTextProperty, AllTextProperty);
        Forward(MultiSelectComboBox.NoneTextProperty, NoneTextProperty);
        Forward(MultiSelectComboBox.CountFormatProperty, CountFormatProperty);
        Forward(MultiSelectComboBox.ShowSelectAllRowProperty, ShowSelectAllRowProperty);
        Forward(MultiSelectComboBox.ShowClearButtonProperty, ShowClearButtonProperty);
        Forward(MultiSelectComboBox.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(MultiSelectComboBox.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(MultiSelectComboBox.ItemGroupsProperty, ItemGroupsProperty);
        Forward(MultiSelectComboBox.IsGroupCheckableProperty, IsGroupCheckableProperty);
        Forward(MultiSelectComboBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(MultiSelectComboBox.DisabledKeysProperty, DisabledKeysProperty);
        Forward(MultiSelectComboBox.ItemTemplateProperty, ItemTemplateProperty);
        Forward(MultiSelectComboBox.SelectNewItemsProperty, SelectNewItemsProperty);
        Forward(MultiSelectComboBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(MultiSelectComboBox.ItemsProperty, ItemsProperty);

        PartComboBox.SelectedKeysChanged += OnComboBoxSelectedKeysChanged;
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; the list scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets the inner multi-select drop-down.
    /// </summary>
    internal MultiSelectComboBox ComboBox => PartComboBox;

    private MultiSelectComboBox PartComboBox { get; }

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
        var control = (LabelMultiSelectComboBox)d;
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
        if (IsMandatory && (SelectedKeys is null || SelectedKeys.Count == 0))
        {
            ValidationText = Validations.FieldIsRequired;
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