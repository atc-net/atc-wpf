namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled, always-visible check list (<see cref="CheckListBox"/>) with a mandatory marker and validation text.
/// When <c>IsMandatory</c> is set, at least one key must be ticked.
/// </summary>
public partial class LabelCheckListBox : ILabelCheckListBox
{
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
    /// Whether the top row with the tri-state "Select all" check box and the count is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectAllRow;

    /// <summary>
    /// Whether a "Clear" button is shown in the "Select all" row.
    /// </summary>
    [DependencyProperty]
    private bool showClearButton;

    /// <summary>
    /// Whether the filter box is shown; <see langword="null"/> (the default) shows it when there are more than 10 items.
    /// </summary>
    [DependencyProperty]
    private bool? isFilterVisible;

    /// <summary>
    /// The group of each key (key to group name); <see langword="null"/> for no groups.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string>? itemGroups;

    /// <summary>
    /// Whether a group header has a tri-state check box that ticks or unticks its shown items.
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
    /// How a row shows whether it is ticked: a check box, a highlight or a switch.
    /// </summary>
    [DependencyProperty(DefaultValue = CheckListItemStyle.CheckBox)]
    private CheckListItemStyle itemStyle;

    /// <summary>
    /// The number of columns the items are laid out in.
    /// </summary>
    [DependencyProperty(DefaultValue = 1)]
    private int columns;

    /// <summary>
    /// The template for the text part of a row; its data context is the <see cref="CheckListItem"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// The maximum height of the list; the list scrolls (virtualized) beyond it.
    /// </summary>
    [DependencyProperty(DefaultValue = 250d)]
    private double maxListHeight;

    /// <summary>
    /// Occurs when the ticked keys change through the list (a tick, Select all, Clear, a group or a range).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the ticked keys change through the list and fail validation (a mandatory field with nothing ticked).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCheckListBox"/> class.
    /// </summary>
    public LabelCheckListBox()
    {
        InitializeComponent();

        PartCheckListBox = new CheckListBox();
        var border = new Border
        {
            BorderThickness = new Thickness(1),
            Child = PartCheckListBox,
        };
        border.SetResourceReference(Border.BorderBrushProperty, "AtcApps.Brushes.Control.Border");
        ((LabelContent)Content).Content = border;

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        Forward(CheckListBox.SelectNewItemsProperty, SelectNewItemsProperty);
        Forward(CheckListBox.ShowSelectAllRowProperty, ShowSelectAllRowProperty);
        Forward(CheckListBox.ShowClearButtonProperty, ShowClearButtonProperty);
        Forward(CheckListBox.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(CheckListBox.ItemGroupsProperty, ItemGroupsProperty);
        Forward(CheckListBox.IsGroupCheckableProperty, IsGroupCheckableProperty);
        Forward(CheckListBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(CheckListBox.DisabledKeysProperty, DisabledKeysProperty);
        Forward(CheckListBox.ItemStyleProperty, ItemStyleProperty);
        Forward(CheckListBox.ColumnsProperty, ColumnsProperty);
        Forward(CheckListBox.ItemTemplateProperty, ItemTemplateProperty);
        Forward(MaxHeightProperty, MaxListHeightProperty);
        Forward(CheckListBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(CheckListBox.ItemsProperty, ItemsProperty);

        PartCheckListBox.SelectedKeysChanged += OnListSelectedKeysChanged;
    }

    /// <summary>
    /// Gets the inner check list.
    /// </summary>
    internal CheckListBox CheckList => PartCheckListBox;

    private CheckListBox PartCheckListBox { get; }

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
        var control = (LabelCheckListBox)d;
        if (!string.IsNullOrEmpty(control.ValidationText))
        {
            control.Validate();
        }
    }

    private void Forward(
        DependencyProperty innerProperty,
        DependencyProperty outerProperty,
        BindingMode mode = BindingMode.OneWay)
        => PartCheckListBox.SetBinding(
            innerProperty,
            new Binding(outerProperty.Name)
            {
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

    private void OnListSelectedKeysChanged(
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