// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Forms.PropertyEditing.Editors;

/// <summary>
/// Property editor for DateTime and DateOnly values using a DatePicker control.
/// </summary>
public sealed class DateTimePropertyEditor : PropertyGridEditorBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Type> SupportedTypes { get; } =
    [
        typeof(DateTime),
        typeof(DateOnly),
        typeof(DateTimeOffset),
    ];

    /// <inheritdoc />
    public override int Priority => 100;

    /// <inheritdoc />
    public override FrameworkElement CreateEditor(PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var datePicker = new DatePicker
        {
            IsEnabled = !item.IsReadOnly,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        return datePicker;
    }

    /// <inheritdoc />
    public override void BindToProperty(
        FrameworkElement editor,
        PropertyGridItem item)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(item);

        if (editor is not DatePicker datePicker)
        {
            return;
        }

        // Set initial value
        datePicker.SelectedDate = ConvertToDateTime(item.Value);

        // Bind for two-way updates
        datePicker.SelectedDateChanged += (_, _) =>
        {
            var underlyingType = GetNonNullableType(item.PropertyType);
            item.Value = ConvertToTargetType(datePicker.SelectedDate, underlyingType);
        };

        // Subscribe to property changes
        item.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PropertyGridItem.Value))
            {
                datePicker.SelectedDate = ConvertToDateTime(item.Value);
            }
        };
    }

    private static DateTime? ConvertToDateTime(object? value)
        => value switch
        {
            DateTime dt => dt,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            DateTimeOffset dto => dto.DateTime,
            _ => null,
        };

    private static object? ConvertToTargetType(
        DateTime? dateTime,
        Type targetType)
    {
        if (dateTime is null)
        {
            return null;
        }

        if (targetType == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(dateTime.Value);
        }

        if (targetType == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(dateTime.Value);
        }

        return dateTime.Value;
    }
}