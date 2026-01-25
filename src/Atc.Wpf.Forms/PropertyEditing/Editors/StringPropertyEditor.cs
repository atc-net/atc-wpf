namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for string values using a TextBox control.
/// </summary>
public sealed class StringPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(string), typeof(char)];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var textBox = new TextBox
        {
            IsReadOnly = item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        // Apply MaxLength attribute if present
        var maxLengthAttr = item.GetAttribute<MaxLengthAttribute>();
        if (maxLengthAttr is not null)
        {
            textBox.MaxLength = maxLengthAttr.Length;
        }

        // For char type, limit to 1 character
        if (GetNonNullableType(item.PropertyType) == typeof(char))
        {
            textBox.MaxLength = 1;
        }

        return textBox;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not TextBox textBox)
        {
            return;
        }

        // Set initial value
        textBox.Text = item.Value?.ToString() ?? string.Empty;

        // Bind for two-way updates
        textBox.TextChanged += (_, _) =>
        {
            if (GetNonNullableType(item.PropertyType) == typeof(char))
            {
                item.Value = string.IsNullOrEmpty(textBox.Text) ? '\0' : textBox.Text[0];
            }
            else
            {
                item.Value = textBox.Text;
            }
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                textBox.Text = item.Value?.ToString() ?? string.Empty;
            }
        };
    }
}