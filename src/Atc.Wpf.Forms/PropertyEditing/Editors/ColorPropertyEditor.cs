namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for Color values using a ColorPicker control.
/// </summary>
public sealed class ColorPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(Color)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var colorPicker = new BaseControls.ColorPicker
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
        };

        return colorPicker;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not BaseControls.ColorPicker colorPicker)
        {
            return;
        }

        // Set initial value
        if (item.Value is Color color)
        {
            colorPicker.ColorValue = color;
        }

        // Bind for two-way updates
        colorPicker.ColorChanged += (_, e) =>
        {
            item.Value = e.NewValue;
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value) && item.Value is Color newColor)
            {
                colorPicker.ColorValue = newColor;
            }
        };
    }
}