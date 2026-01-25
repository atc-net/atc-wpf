namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for decimal/float/double values using a DecimalBox control.
/// </summary>
public sealed class DecimalPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } =
    [
        typeof(decimal),
        typeof(double),
        typeof(float),
    ];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var decimalBox = new DecimalBox
        {
            IsReadOnly = item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            HideUpDownButtons = true,
        };

        // Apply Range attribute if present
        var rangeAttr = item.GetAttribute<RangeAttribute>();
        if (rangeAttr is null)
        {
            return decimalBox;
        }

        decimalBox.Minimum = rangeAttr.Minimum switch
        {
            double minDouble => minDouble,
            int minInt => minInt,
            _ => decimalBox.Minimum,
        };

        decimalBox.Maximum = rangeAttr.Maximum switch
        {
            double maxDouble => maxDouble,
            int maxInt => maxInt,
            _ => decimalBox.Maximum,
        };

        return decimalBox;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not DecimalBox decimalBox)
        {
            return;
        }

        // Set initial value
        decimalBox.Value = ConvertToDouble(item.Value);

        // Bind for two-way updates
        decimalBox.ValueChanged += (_, _) =>
        {
            var underlyingType = GetNonNullableType(item.PropertyType);
            item.Value = ConvertToTargetType(decimalBox.Value ?? 0d, underlyingType);
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                decimalBox.Value = ConvertToDouble(item.Value);
            }
        };
    }

    private static double ConvertToDouble(object? value)
    {
        if (value is null)
        {
            return 0d;
        }

        return value switch
        {
            double d => d,
            decimal dec => (double)dec,
            float f => f,
            _ => Convert.ToDouble(value, GlobalizationConstants.EnglishCultureInfo),
        };
    }

    private static object ConvertToTargetType(
        double value,
        Type targetType)
    {
        if (targetType == typeof(decimal))
        {
            return (decimal)value;
        }

        if (targetType == typeof(float))
        {
            return (float)value;
        }

        return value;
    }
}