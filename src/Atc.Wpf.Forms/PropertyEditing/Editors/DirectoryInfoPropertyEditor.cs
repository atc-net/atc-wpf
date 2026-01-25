namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for DirectoryInfo values using a DirectoryPicker control.
/// </summary>
public sealed class DirectoryInfoPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(DirectoryInfo)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var directoryPicker = new DirectoryPicker
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
        };

        return directoryPicker;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not DirectoryPicker directoryPicker)
        {
            return;
        }

        // Set initial value
        if (item.Value is DirectoryInfo directoryInfo)
        {
            directoryPicker.Value = directoryInfo;
        }

        // Bind for two-way updates
        directoryPicker.LostFocus += (_, _) =>
        {
            item.Value = directoryPicker.Value;
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value) && item.Value is DirectoryInfo newDirectoryInfo)
            {
                directoryPicker.Value = newDirectoryInfo;
            }
        };
    }
}