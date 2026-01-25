namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for SolidColorBrush values using a ColorPicker control.
/// </summary>
public sealed class BrushPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(SolidColorBrush)];

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
        if (item.Value is SolidColorBrush brush)
        {
            colorPicker.BrushValue = brush;
        }

        // Bind for two-way updates
        colorPicker.ColorChanged += (_, e) =>
        {
            item.Value = new SolidColorBrush(e.NewValue);
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value) && item.Value is SolidColorBrush newBrush)
            {
                colorPicker.BrushValue = newBrush;
            }
        };
    }
}