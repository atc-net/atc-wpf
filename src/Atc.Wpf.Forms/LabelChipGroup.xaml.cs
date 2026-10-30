namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled group of toggle chips (<see cref="ChipGroup"/>) with a mandatory marker and validation text.
/// When <c>IsMandatory</c> is set, at least one key must be selected; with <c>MaxSelectionCount</c>, no more than that many.
/// </summary>
public partial class LabelChipGroup : ILabelChipGroup
{
    /// <summary>
    /// The items, keyed by their selection key, with their display value.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the selected chips. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// How many chips can be selected: any number, exactly one (a radio group) or at most one.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipSelectionMode.Multiple)]
    private ChipSelectionMode selectionMode;

    /// <summary>
    /// Whether keys that appear in <see cref="Items"/> are selected automatically (multiple selection only).
    /// </summary>
    [DependencyProperty]
    private bool selectNewItems;

    /// <summary>
    /// How a selected chip looks: filled with the accent colour, or outlined with an accent border.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipGroupVariant.Filled)]
    private ChipGroupVariant chipVariant;

    /// <summary>
    /// The size of the chips.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipSize.Medium)]
    private ChipSize chipSize;

    /// <summary>
    /// Whether a selected chip shows a check mark before its text.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showCheckMark;

    /// <summary>
    /// The direction the chips are laid out in (the label position is set with <c>Orientation</c>).
    /// </summary>
    [DependencyProperty(DefaultValue = Orientation.Horizontal)]
    private Orientation chipOrientation;

    /// <summary>
    /// Whether the chips wrap onto more lines; <see langword="false"/> keeps them on one line that scrolls.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool wrap;

    /// <summary>
    /// The space between two chips (and between two lines).
    /// </summary>
    [DependencyProperty(DefaultValue = 4d)]
    private double itemSpacing;

    /// <summary>
    /// Whether a leading "All" chip is shown (multiple selection only).
    /// </summary>
    [DependencyProperty]
    private bool showAllChip;

    /// <summary>
    /// The maximum number of selected keys; <see langword="null"/> for no limit.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys whose chips are shown but cannot be toggled.
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<string> disabledKeys;

    /// <summary>
    /// An optional icon per key shown before the text.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, object>? itemIcons;

    /// <summary>
    /// An optional count per key shown after the text.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, int>? itemCounts;

    /// <summary>
    /// Occurs when the selected keys change through the chips (a toggle, the "All" chip, an arrow key in a
    /// single-select group, or an items change).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the selected keys change through the chips and fail validation (a mandatory field with nothing
    /// selected, or more keys than <see cref="MaxSelectionCount"/>).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelChipGroup"/> class.
    /// </summary>
    public LabelChipGroup()
    {
        InitializeComponent();

        PartChipGroup = new ChipGroup();
        ((LabelContent)Content).Content = PartChipGroup;

        SetCurrentValue(SelectedKeysProperty, new Atc.Wpf.Collections.ObservableCollectionEx<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        Forward(ChipGroup.SelectionModeProperty, SelectionModeProperty);
        Forward(ChipGroup.SelectNewItemsProperty, SelectNewItemsProperty);
        Forward(ChipGroup.ChipVariantProperty, ChipVariantProperty);
        Forward(ChipGroup.ChipSizeProperty, ChipSizeProperty);
        Forward(ChipGroup.ShowCheckMarkProperty, ShowCheckMarkProperty);
        Forward(ChipGroup.OrientationProperty, ChipOrientationProperty);
        Forward(ChipGroup.WrapProperty, WrapProperty);
        Forward(ChipGroup.ItemSpacingProperty, ItemSpacingProperty);
        Forward(ChipGroup.ShowAllChipProperty, ShowAllChipProperty);
        Forward(ChipGroup.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(ChipGroup.DisabledKeysProperty, DisabledKeysProperty);
        Forward(ChipGroup.ItemIconsProperty, ItemIconsProperty);
        Forward(ChipGroup.ItemCountsProperty, ItemCountsProperty);
        Forward(ChipGroup.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(ChipGroup.ItemsProperty, ItemsProperty);

        PartChipGroup.SelectedKeysChanged += OnChipGroupSelectedKeysChanged;
    }

    /// <summary>
    /// Gets the inner chip group.
    /// </summary>
    internal ChipGroup ChipGroup => PartChipGroup;

    private ChipGroup PartChipGroup { get; }

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
        var control = (LabelChipGroup)d;
        if (!string.IsNullOrEmpty(control.ValidationText))
        {
            control.Validate();
        }
    }

    private void Forward(
        DependencyProperty innerProperty,
        DependencyProperty outerProperty,
        BindingMode mode = BindingMode.OneWay)
        => PartChipGroup.SetBinding(
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

        if (MaxSelectionCount is { } max && CountSelectedItems() > max)
        {
            ValidationText = string.Format(CultureInfo.CurrentCulture, Validations.SelectAtMost1, max);
            return false;
        }

        ValidationText = string.Empty;
        return true;
    }

    private int CountSelectedItems()
    {
        if (SelectedKeys is null || Items is null)
        {
            return 0;
        }

        return SelectedKeys
            .Distinct(StringComparer.Ordinal)
            .Count(Items.ContainsKey);
    }

    private void OnChipGroupSelectedKeysChanged(
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