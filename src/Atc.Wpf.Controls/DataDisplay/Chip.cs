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
    /// Whether a check mark is shown before the content when the chip is selected;
    /// <see langword="null"/> (the default) shows it only for the <see cref="ChipVariant.Filter"/> variant.
    /// </summary>
    [DependencyProperty]
    private bool? showCheckMark;

    /// <summary>
    /// Border brush when the chip is selected; <see langword="null"/> (the default) uses <see cref="SelectedBackground"/>.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedBorderBrush;

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

    /// <summary>
    /// Gets or sets a hook that decides what a click (or a UI Automation toggle) does instead of the chip flipping
    /// <see cref="IsSelected"/> itself. A chip group sets it so it can apply its selection mode and limits.
    /// </summary>
    internal Action<Chip>? ToggleRequested { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the chip is one option of a single-select group: UI Automation then
    /// exposes it as a radio button with the selection-item pattern instead of a check box.
    /// </summary>
    internal bool IsRadioItem { get; set; }

    /// <summary>
    /// Gets a value indicating whether a click (or a UI Automation toggle) changes the selection.
    /// </summary>
    internal bool IsToggleable
        => ToggleRequested is not null || IsSelectable;

    /// <summary>
    /// Does what a mouse click does: toggles the selection (through <see cref="ToggleRequested"/> when set) and
    /// raises <see cref="Click"/>. Nothing happens when <see cref="IsClickable"/> is <see langword="false"/>.
    /// </summary>
    internal void PerformClick()
    {
        if (!IsClickable)
        {
            return;
        }

        RequestToggle();
        OnClick();
    }

    /// <summary>
    /// Toggles the selection: through <see cref="ToggleRequested"/> when set, otherwise when <see cref="IsSelectable"/>.
    /// </summary>
    internal void RequestToggle()
    {
        if (ToggleRequested is not null)
        {
            ToggleRequested(this);
            return;
        }

        if (IsSelectable)
        {
            SetCurrentValue(IsSelectedProperty, !IsSelected);
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new ChipAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        PerformClick();
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
            if (UIElementAutomationPeer.FromElement(chip) is ChipAutomationPeer peer)
            {
                peer.RaiseSelectionChanged((bool)e.OldValue, (bool)e.NewValue);
            }

            chip.OnSelectionChanged();
        }
    }
}