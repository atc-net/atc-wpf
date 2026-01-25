namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for enum values using a ComboBox control.
/// </summary>
public sealed class EnumPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override bool CanEdit(
        Type propertyType,
        IReadOnlyList<Attribute> attributes)
    {
        var nonNullableType = GetNonNullableType(propertyType);
        return nonNullableType.IsEnum;
    }

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var comboBox = new ComboBox
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        var enumType = GetNonNullableType(item.PropertyType);
        var enumValues = Enum.GetValues(enumType);

        foreach (var enumValue in enumValues)
        {
            var enumMember = (Enum)enumValue;
            var displayText = GetEnumDisplayText(enumMember);

            comboBox.Items.Add(new ComboBoxItem
            {
                Content = displayText,
                Tag = enumValue,
            });
        }

        return comboBox;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not ComboBox comboBox)
        {
            return;
        }

        // Set initial selection
        SetSelectedItem(comboBox, item.Value);

        // Bind for two-way updates
        comboBox.SelectionChanged += (_, _) =>
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                item.Value = selectedItem.Tag;
            }
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                SetSelectedItem(comboBox, item.Value);
            }
        };
    }

    private static void SetSelectedItem(
        ComboBox comboBox,
        object? value)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            if (Equals(item.Tag, value))
            {
                comboBox.SelectedItem = item;
                return;
            }
        }
    }

    private static string GetEnumDisplayText(Enum enumValue)
    {
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
        if (memberInfo.Length > 0)
        {
            // Try to get Description attribute
            var descriptionAttr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr is not null)
            {
                return descriptionAttr.Description;
            }

            // Try to get Display attribute
            var displayAttr = memberInfo[0].GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
            if (displayAttr is not null && !string.IsNullOrEmpty(displayAttr.Name))
            {
                return displayAttr.Name;
            }
        }

        // Fallback: normalize the enum name
        return enumValue.ToString().NormalizePascalCase();
    }
}