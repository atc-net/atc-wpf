namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Base class for property grid editors.
/// </summary>
public abstract class PropertyGridEditorBase : Abstractions.IPropertyGridEditor
{
    /// <inheritdoc />
    public abstract IReadOnlyList<Type> SupportedTypes { get; }

    /// <inheritdoc />
    public virtual int Priority => 0;

    /// <inheritdoc />
    public virtual bool CanEdit(
        Type propertyType,
        IReadOnlyList<Attribute> attributes)
    {
        var nonNullableType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        return SupportedTypes.Any(t => t.IsAssignableFrom(nonNullableType));
    }

    /// <inheritdoc />
    public abstract FrameworkElement CreateEditor(PropertyGridItem item);

    /// <inheritdoc />
    public abstract void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item);

    /// <summary>
    /// Gets the non-nullable type from the property type.
    /// </summary>
    /// <param name="type">The property type.</param>
    /// <returns>The underlying type if nullable; otherwise, the original type.</returns>
    protected static Type GetNonNullableType(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;

    /// <summary>
    /// Determines if the property type is nullable.
    /// </summary>
    /// <param name="type">The property type.</param>
    /// <returns><c>true</c> if the type is nullable; otherwise, <c>false</c>.</returns>
    protected static bool IsNullable(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) is not null ||
               !type.IsValueType;
    }
}