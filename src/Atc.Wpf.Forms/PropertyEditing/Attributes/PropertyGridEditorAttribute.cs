namespace Atc.Wpf.Forms.PropertyEditing.Attributes;

/// <summary>
/// Specifies a custom property grid editor to use for a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class PropertyGridEditorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridEditorAttribute"/> class.
    /// </summary>
    /// <param name="editorType">The type of the custom editor, which must implement <see cref="Abstractions.IPropertyGridEditor"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="editorType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="editorType"/> does not implement <see cref="Abstractions.IPropertyGridEditor"/>.</exception>
    public PropertyGridEditorAttribute(Type editorType)
    {
        ArgumentNullException.ThrowIfNull(editorType);

        if (!typeof(Abstractions.IPropertyGridEditor).IsAssignableFrom(editorType))
        {
            throw new ArgumentException(
                $"The editor type must implement {nameof(Abstractions.IPropertyGridEditor)}.",
                nameof(editorType));
        }

        EditorType = editorType;
    }

    /// <summary>
    /// Gets the type of the custom editor.
    /// </summary>
    public Type EditorType { get; }
}