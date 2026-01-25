namespace Atc.Wpf.Forms.PropertyEditing.Abstractions;

/// <summary>
/// Defines the interface for property grid editors that create and manage
/// UI controls for editing property values.
/// </summary>
public interface IPropertyGridEditor
{
    /// <summary>
    /// Gets the list of .NET types this editor supports.
    /// </summary>
    IReadOnlyList<Type> SupportedTypes { get; }

    /// <summary>
    /// Gets the priority of this editor. Higher values indicate higher priority
    /// when multiple editors can handle the same type.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether this editor can edit the specified property type
    /// based on the type and its attributes.
    /// </summary>
    /// <param name="propertyType">The type of the property.</param>
    /// <param name="attributes">The attributes applied to the property.</param>
    /// <returns><c>true</c> if this editor can edit the property; otherwise, <c>false</c>.</returns>
    bool CanEdit(
        Type propertyType,
        IReadOnlyList<Attribute> attributes);

    /// <summary>
    /// Creates the editor control for the specified property grid item.
    /// </summary>
    /// <param name="item">The property grid item containing property information.</param>
    /// <returns>A <see cref="FrameworkElement"/> that provides the editing UI.</returns>
    FrameworkElement CreateEditor(PropertyGridItem item);

    /// <summary>
    /// Binds the editor control to the property grid item for two-way data synchronization.
    /// </summary>
    /// <param name="editor">The editor control created by <see cref="CreateEditor"/>.</param>
    /// <param name="item">The property grid item to bind to.</param>
    void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item);
}