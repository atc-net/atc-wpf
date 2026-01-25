namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for boolean values using a ToggleSwitch control.
/// </summary>
public sealed class BooleanPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(bool)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var toggle = new ToggleSwitch
        {
            IsEnabled = !item.IsReadOnly,
            OnContent = "Yes",
            OffContent = "No",
            VerticalAlignment = VerticalAlignment.Center,
        };

        return toggle;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not ToggleSwitch toggle)
        {
            return;
        }

        // Set initial value
        toggle.IsOn = item.Value is true;

        // Bind for two-way updates
        toggle.Toggled += (_, _) =>
        {
            item.Value = toggle.IsOn;
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                toggle.IsOn = item.Value is true;
            }
        };
    }
}