namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for TimeOnly values using a TextBox with time format.
/// </summary>
public sealed class TimeOnlyPropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } = [typeof(TimeOnly), typeof(TimeSpan)];

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
        textBox.Text = FormatTimeValue(item.Value);

        // Bind for two-way updates
        textBox.LostFocus += (_, _) =>
        {
            var underlyingType = GetNonNullableType(item.PropertyType);
            var parsedValue = ParseTimeValue(textBox.Text, underlyingType);
            if (parsedValue is not null)
            {
                item.Value = parsedValue;
            }
            else
            {
                // Revert to original value on parse failure
                textBox.Text = FormatTimeValue(item.Value);
            }
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                textBox.Text = FormatTimeValue(item.Value);
            }
        };
    }

    private static string FormatTimeValue(object? value)
        => value switch
        {
            TimeOnly t => t.ToString("HH:mm:ss", GlobalizationConstants.EnglishCultureInfo),
            TimeSpan ts => ts.ToString(@"hh\:mm\:ss", GlobalizationConstants.EnglishCultureInfo),
            _ => string.Empty,
        };

    private static object? ParseTimeValue(
        string text,
        Type targetType)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (targetType == typeof(TimeOnly))
        {
            if (TimeOnly.TryParse(text, GlobalizationConstants.EnglishCultureInfo, out var timeOnly))
            {
                return timeOnly;
            }
        }
        else if (targetType == typeof(TimeSpan) &&
                 TimeSpan.TryParse(text, GlobalizationConstants.EnglishCultureInfo, out var timeSpan))
        {
            return timeSpan;
        }

        return null;
    }
}