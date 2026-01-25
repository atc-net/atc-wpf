// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.Forms.PropertyEditing.Factories;

/// <summary>
/// Factory for creating property grid editors based on property types.
/// </summary>
public sealed class PropertyGridEditorFactory
{
    private readonly List<Abstractions.IPropertyGridEditor> editors = [];
    private readonly Dictionary<Type, Abstractions.IPropertyGridEditor> editorCache = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridEditorFactory"/> class.
    /// </summary>
    public PropertyGridEditorFactory()
        => RegisterDefaultEditors();

    /// <summary>
    /// Registers a custom editor.
    /// </summary>
    /// <param name="editor">The editor to register.</param>
    public void RegisterEditor(Abstractions.IPropertyGridEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        editors.Add(editor);
        editorCache.Clear(); // Clear cache when new editor is added
    }

    /// <summary>
    /// Gets the appropriate editor for the specified property item.
    /// </summary>
    /// <param name="item">The property grid item.</param>
    /// <returns>The editor for the property, or null if no suitable editor is found.</returns>
    public Abstractions.IPropertyGridEditor? GetEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Check for custom editor attribute first
        var customEditorAttr = item.GetAttribute<Attributes.PropertyGridEditorAttribute>();
        if (customEditorAttr is not null)
        {
            return CreateEditorInstance(customEditorAttr.EditorType);
        }

        // Try cache first
        var propertyType = item.PropertyType;
        if (editorCache.TryGetValue(propertyType, out var cachedEditor))
        {
            return cachedEditor;
        }

        // Find the best matching editor
        var editor = FindBestEditor(propertyType, item.Attributes);
        if (editor is not null)
        {
            editorCache[propertyType] = editor;
        }

        return editor;
    }

    /// <summary>
    /// Creates an editor control for the specified property item.
    /// </summary>
    /// <param name="item">The property grid item.</param>
    /// <returns>The editor framework element, or null if no suitable editor is found.</returns>
    public FrameworkElement? CreateEditorControl(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var editor = GetEditor(item);
        if (editor is null)
        {
            return null;
        }

        var control = editor.CreateEditor(item);
        editor.BindToProperty(control, item);
        return control;
    }

    private void RegisterDefaultEditors()
    {
        editors.Add(new Editors.BooleanPropertyEditor());
        editors.Add(new Editors.StringPropertyEditor());
        editors.Add(new Editors.IntegerPropertyEditor());
        editors.Add(new Editors.DecimalPropertyEditor());
        editors.Add(new Editors.EnumPropertyEditor());
        editors.Add(new Editors.DateTimePropertyEditor());
        editors.Add(new Editors.TimeOnlyPropertyEditor());
        editors.Add(new Editors.ColorPropertyEditor());
        editors.Add(new Editors.BrushPropertyEditor());
        editors.Add(new Editors.ThicknessPropertyEditor());
        editors.Add(new Editors.FileInfoPropertyEditor());
        editors.Add(new Editors.DirectoryInfoPropertyEditor());
        editors.Add(new Editors.NestedObjectPropertyEditor());
    }

    private Abstractions.IPropertyGridEditor? FindBestEditor(
        Type propertyType,
        IReadOnlyList<Attribute> attributes)
    {
        Abstractions.IPropertyGridEditor? bestEditor = null;
        var bestPriority = int.MinValue;

        foreach (var editor in editors)
        {
            if (editor.CanEdit(propertyType, attributes) && editor.Priority > bestPriority)
            {
                bestEditor = editor;
                bestPriority = editor.Priority;
            }
        }

        return bestEditor;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    private static Abstractions.IPropertyGridEditor? CreateEditorInstance(
        Type editorType)
    {
        try
        {
            return Activator.CreateInstance(editorType) as Abstractions.IPropertyGridEditor;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Failed to create editor instance of type {editorType}: {ex.Message}");
            return null;
        }
    }
}