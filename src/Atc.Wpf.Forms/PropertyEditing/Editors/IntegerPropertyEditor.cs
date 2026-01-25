namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for integer values using an IntegerBox control.
/// </summary>
public sealed class IntegerPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } =
    [
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(short),
        typeof(ushort),
        typeof(byte),
        typeof(sbyte),
    ];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var integerBox = new IntegerBox
        {
            IsReadOnly = item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            HideUpDownButtons = true,
        };

        // Apply Range attribute if present
        var rangeAttr = item.GetAttribute<RangeAttribute>();
        if (rangeAttr is not null)
        {
            integerBox.Minimum = rangeAttr.Minimum switch
            {
                int minInt => minInt,
                double minDouble => minDouble,
                _ => integerBox.Minimum,
            };

            integerBox.Maximum = rangeAttr.Maximum switch
            {
                int maxInt => maxInt,
                double maxDouble => maxDouble,
                _ => integerBox.Maximum,
            };
        }
        else
        {
            // Set type-specific defaults
            var underlyingType = GetNonNullableType(item.PropertyType);
            SetTypeBounds(integerBox, underlyingType);
        }

        return integerBox;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not IntegerBox integerBox)
        {
            return;
        }

        // Set initial value
        integerBox.Value = Convert.ToDouble(item.Value, GlobalizationConstants.EnglishCultureInfo);

        // Bind for two-way updates
        integerBox.ValueChanged += (_, _) =>
        {
            var underlyingType = GetNonNullableType(item.PropertyType);
            var intValue = (int)(integerBox.Value ?? 0);
            item.Value = ConvertToTargetType(intValue, underlyingType);
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                integerBox.Value = Convert.ToDouble(item.Value, GlobalizationConstants.EnglishCultureInfo);
            }
        };
    }

    private static void SetTypeBounds(
        IntegerBox integerBox,
        Type type)
    {
        if (type == typeof(byte))
        {
            integerBox.Minimum = byte.MinValue;
            integerBox.Maximum = byte.MaxValue;
        }
        else if (type == typeof(sbyte))
        {
            integerBox.Minimum = sbyte.MinValue;
            integerBox.Maximum = sbyte.MaxValue;
        }
        else if (type == typeof(short))
        {
            integerBox.Minimum = short.MinValue;
            integerBox.Maximum = short.MaxValue;
        }
        else if (type == typeof(ushort))
        {
            integerBox.Minimum = ushort.MinValue;
            integerBox.Maximum = ushort.MaxValue;
        }
    }

    private static object ConvertToTargetType(
        int value,
        Type targetType)
    {
        if (targetType == typeof(byte))
        {
            return (byte)value;
        }

        if (targetType == typeof(sbyte))
        {
            return (sbyte)value;
        }

        if (targetType == typeof(short))
        {
            return (short)value;
        }

        if (targetType == typeof(ushort))
        {
            return (ushort)value;
        }

        if (targetType == typeof(uint))
        {
            return (uint)value;
        }

        if (targetType == typeof(long))
        {
            return (long)value;
        }

        if (targetType == typeof(ulong))
        {
            return (ulong)value;
        }

        return value;
    }
}