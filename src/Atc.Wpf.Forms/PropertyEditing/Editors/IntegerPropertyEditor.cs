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

        // Bound by the property type, narrowed by a Range attribute if present
        var (typeMinimum, typeMaximum) = GetTypeBounds(GetNonNullableType(item.PropertyType));
        var minimum = (double)typeMinimum;
        var maximum = (double)typeMaximum;
        var rangeAttr = item.GetAttribute<RangeAttribute>();
        if (rangeAttr is not null)
        {
            minimum = System.Math.Max(minimum, Convert.ToDouble(rangeAttr.Minimum, CultureInfo.InvariantCulture));
            maximum = System.Math.Min(maximum, Convert.ToDouble(rangeAttr.Maximum, CultureInfo.InvariantCulture));
        }

        integerBox.Minimum = minimum;
        integerBox.Maximum = maximum;

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
            item.Value = ConvertToTargetType(integerBox.Value ?? 0, underlyingType);
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

    private static (decimal Minimum, decimal Maximum) GetTypeBounds(Type type)
    {
        if (type == typeof(byte))
        {
            return (byte.MinValue, byte.MaxValue);
        }

        if (type == typeof(sbyte))
        {
            return (sbyte.MinValue, sbyte.MaxValue);
        }

        if (type == typeof(short))
        {
            return (short.MinValue, short.MaxValue);
        }

        if (type == typeof(ushort))
        {
            return (ushort.MinValue, ushort.MaxValue);
        }

        if (type == typeof(uint))
        {
            return (uint.MinValue, uint.MaxValue);
        }

        if (type == typeof(long))
        {
            return (long.MinValue, long.MaxValue);
        }

        if (type == typeof(ulong))
        {
            return (ulong.MinValue, ulong.MaxValue);
        }

        return (int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Rounds the editor value to a whole number and converts it to the property type, clamping it to the type's
    /// range. The bounds are compared as <see cref="double"/> first, because <see cref="long.MaxValue"/> and
    /// <see cref="ulong.MaxValue"/> round up beyond the type as a double, and then set exactly from <see cref="decimal"/>.
    /// </summary>
    private static object ConvertToTargetType(
        double value,
        Type targetType)
    {
        var (minimum, maximum) = GetTypeBounds(targetType);
        decimal wholeNumber;
        if (double.IsNaN(value))
        {
            wholeNumber = 0;
        }
        else if (value <= (double)minimum)
        {
            wholeNumber = minimum;
        }
        else if (value >= (double)maximum)
        {
            wholeNumber = maximum;
        }
        else
        {
            wholeNumber = System.Math.Clamp(
                System.Math.Round((decimal)value, MidpointRounding.AwayFromZero),
                minimum,
                maximum);
        }

        return Convert.ChangeType(wholeNumber, targetType, CultureInfo.InvariantCulture);
    }
}