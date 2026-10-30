namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The container of one card in a <see cref="CardSelector"/>. A click (or <c>Space</c>) asks the selector to toggle
/// the card, so the selection mode, the limit and the disabled items are always honoured.
/// </summary>
public sealed class CardSelectorItem : ListBoxItem
{
    private static readonly DependencyPropertyKey IsRadioItemPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsRadioItem),
        typeof(bool),
        typeof(CardSelectorItem),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    private static readonly DependencyPropertyKey ShowCheckMarkPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ShowCheckMark),
        typeof(bool),
        typeof(CardSelectorItem),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Identifies the <see cref="IsRadioItem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsRadioItemProperty = IsRadioItemPropertyKey.DependencyProperty;

    /// <summary>
    /// Identifies the <see cref="ShowCheckMark"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCheckMarkProperty = ShowCheckMarkPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value indicating whether the card is an option of a single-select <see cref="CardSelector"/> (it shows
    /// a radio dot instead of a check mark).
    /// </summary>
    public bool IsRadioItem
    {
        get => (bool)GetValue(IsRadioItemProperty);
        internal set => SetValue(IsRadioItemPropertyKey, value);
    }

    /// <summary>
    /// Gets a value indicating whether the card shows its check mark (or radio dot) when selected
    /// (<see cref="CardSelector.ShowCheckMark"/>).
    /// </summary>
    public bool ShowCheckMark
    {
        get => (bool)GetValue(ShowCheckMarkProperty);
        internal set => SetValue(ShowCheckMarkPropertyKey, value);
    }

    /// <summary>
    /// Gets the selector that owns this card, or <see langword="null"/>.
    /// </summary>
    internal CardSelector? Owner
        => ItemsControl.ItemsControlFromItemContainer(this) as CardSelector;

    /// <summary>
    /// Returns the first non-empty <see cref="AutomationProperties.NameProperty"/> inside the card body (the
    /// <see cref="ItemsControl.ItemTemplate"/>), or <see langword="null"/>.
    /// </summary>
    internal string? GetTemplateAutomationName()
    {
        var item = Owner?.ItemContainerGenerator.ItemFromContainer(this) ?? Content;
        var presenter = FindContentPresenter(this, item);
        return presenter is null
            ? null
            : FindName(presenter);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (Owner is not { } owner)
        {
            base.OnMouseLeftButtonDown(e);
            return;
        }

        Focus();
        owner.RequestToggle(owner.ItemContainerGenerator.ItemFromContainer(this));
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        // A right click must not select the card (the ListBoxItem default would).
    }

    private static ContentPresenter? FindContentPresenter(
        DependencyObject parent,
        object? item)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is ContentPresenter presenter && ReferenceEquals(presenter.Content, item))
            {
                return presenter;
            }

            var nested = FindContentPresenter(child, item);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }

    private static string? FindName(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            var name = AutomationProperties.GetName(child);
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = FindName(child);
            if (name is not null)
            {
                return name;
            }
        }

        return null;
    }
}