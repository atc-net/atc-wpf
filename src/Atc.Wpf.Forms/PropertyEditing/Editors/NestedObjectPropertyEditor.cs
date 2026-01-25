// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for complex/nested objects using an Expander control.
/// This editor has the lowest priority and acts as a fallback for unsupported types.
/// </summary>
public sealed class NestedObjectPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [];

    /// <inheritdoc />
    public override int Priority => -100; // Lowest priority - fallback

    /// <inheritdoc />
    public override bool CanEdit(
        Type propertyType,
        IReadOnlyList<Attribute> attributes)
    {
        // Can handle any reference type that is not a string or collection
        if (propertyType == typeof(string))
        {
            return false;
        }

        if (propertyType.IsValueType && !IsComplexValueType(propertyType))
        {
            return false;
        }

        // Skip arrays and most collections
        if (propertyType.IsArray)
        {
            return false;
        }

        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) &&
            propertyType != typeof(string))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };

        var headerText = item.Value?.GetType().Name ?? "(null)";
        var header = new TextBlock
        {
            Text = headerText,
            FontStyle = FontStyles.Italic,
            Foreground = SystemColors.GrayTextBrush,
        };

        if (item.Value is not null && item.HasChildren)
        {
            var expander = new Expander
            {
                Header = header,
                IsExpanded = item.IsExpanded,
                Padding = new Thickness(10, 0, 0, 0),
            };

            var childPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            expander.Content = childPanel;
            panel.Children.Add(expander);

            // Bind IsExpanded
            expander.Expanded += (_, _) => item.IsExpanded = true;
            expander.Collapsed += (_, _) => item.IsExpanded = false;
        }
        else
        {
            panel.Children.Add(header);
        }

        return panel;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        // For nested objects, child property editors are bound separately
        // through the PropertyGrid's recursive property extraction
    }

    private static bool IsComplexValueType(Type type)
    {
        // Consider structs with multiple properties as complex
        var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;
        if (!nonNullableType.IsValueType)
        {
            return false;
        }

        // Primitives and common simple types are not complex
        if (nonNullableType.IsPrimitive ||
            nonNullableType == typeof(decimal) ||
            nonNullableType == typeof(DateTime) ||
            nonNullableType == typeof(DateOnly) ||
            nonNullableType == typeof(TimeOnly) ||
            nonNullableType == typeof(TimeSpan) ||
            nonNullableType == typeof(Guid) ||
            nonNullableType.IsEnum)
        {
            return false;
        }

        // Other value types (structs with properties) are complex
        return true;
    }
}