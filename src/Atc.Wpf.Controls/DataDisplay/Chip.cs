namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A Chip control for displaying tags, filters, or selections with optional icon and remove button.
/// </summary>
[ContentProperty(nameof(Content))]
[TemplatePart(Name = "PART_RemoveButton", Type = typeof(Button))]
public sealed partial class Chip : ContentControl
{
    /// <summary>
    /// The variant/type of the chip.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipVariant.Default)]
    private ChipVariant variant;

    /// <summary>
    /// The size of the chip.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipSize.Medium)]
    private ChipSize size;

    /// <summary>
    /// Icon content displayed before the main content.
    /// </summary>
    [DependencyProperty]
    private object? icon;

    /// <summary>
    /// Template for the icon content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? iconTemplate;

    /// <summary>
    /// Whether the chip can be removed (shows close button).
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool canRemove;

    /// <summary>
    /// Whether the chip can be selected/toggled.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isSelectable;

    /// <summary>
    /// Whether the chip is currently selected (when IsSelectable is true).
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsSelectedChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private bool isSelected;

    /// <summary>
    /// Whether the chip is clickable (shows hover/click effects).
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isClickable;

    /// <summary>
    /// Corner radius for the chip.
    /// </summary>
    [DependencyProperty(DefaultValue = "new CornerRadius(16)")]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Background brush when the chip is selected.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedBackground;

    /// <summary>
    /// Foreground brush when the chip is selected.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedForeground;

    /// <summary>
    /// Background brush when the mouse is over the chip.
    /// </summary>
    [DependencyProperty]
    private Brush? hoverBackground;

    /// <summary>
    /// Background brush when the chip is pressed.
    /// </summary>
    [DependencyProperty]
    private Brush? pressedBackground;

    /// <summary>
    /// Occurs when the chip is clicked.
    /// </summary>
    public event RoutedEventHandler? Click;

    /// <summary>
    /// Occurs when the remove button is clicked.
    /// </summary>
    public event RoutedEventHandler? RemoveClick;

    /// <summary>
    /// Occurs when the IsSelected property changes.
    /// </summary>
    public event RoutedEventHandler? SelectionChanged;

    static Chip()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Chip),
            new FrameworkPropertyMetadata(typeof(Chip)));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_RemoveButton") is Button removeButton)
        {
            removeButton.Click += OnRemoveButtonClick;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (!IsClickable)
        {
            return;
        }

        if (IsSelectable)
        {
            IsSelected = !IsSelected;
        }

        OnClick();
    }

    private void OnClick()
        => Click?.Invoke(this, new RoutedEventArgs());

    private void OnRemoveButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        e.Handled = true;
        RemoveClick?.Invoke(this, new RoutedEventArgs());
    }

    private void OnSelectionChanged()
        => SelectionChanged?.Invoke(this, new RoutedEventArgs());

    private static void OnIsSelectedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Chip chip)
        {
            chip.OnSelectionChanged();
        }
    }
}