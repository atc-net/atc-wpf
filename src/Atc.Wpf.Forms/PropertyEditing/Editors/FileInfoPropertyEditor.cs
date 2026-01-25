namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for FileInfo values using a FilePicker control.
/// </summary>
public sealed class FileInfoPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(FileInfo)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var filePicker = new FilePicker
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
        };

        return filePicker;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not FilePicker filePicker)
        {
            return;
        }

        // Set initial value
        if (item.Value is FileInfo fileInfo)
        {
            filePicker.Value = fileInfo;
        }

        // Bind for two-way updates
        filePicker.LostFocus += (_, _) =>
        {
            item.Value = filePicker.Value;
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value) && item.Value is FileInfo newFileInfo)
            {
                filePicker.Value = newFileInfo;
            }
        };
    }
}