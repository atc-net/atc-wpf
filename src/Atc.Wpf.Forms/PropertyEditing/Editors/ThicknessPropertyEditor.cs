namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for Thickness values using a ThicknessBox control.
/// </summary>
public sealed class ThicknessPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(Thickness)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var thicknessBox = new ThicknessBox
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            HideUpDownButtons = true,
        };

        return thicknessBox;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not ThicknessBox thicknessBox)
        {
            return;
        }

        // Set initial value
        if (item.Value is Thickness thickness)
        {
            thicknessBox.Value = thickness;
        }

        // Bind for two-way updates
        thicknessBox.ValueChanged += (_, _) =>
        {
            item.Value = thicknessBox.Value;
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value) &&
                item.Value is Thickness newThickness)
            {
                thicknessBox.Value = newThickness;
            }
        };
    }
}