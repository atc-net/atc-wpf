namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// Reads the value of a <see cref="DataGrid"/> column from a row item through the column's binding path (or its
/// <see cref="DataGridColumn.SortMemberPath"/>), and formats it the way the cell shows it: through the binding's
/// converter and <c>StringFormat</c>, in the grid's language.
/// </summary>
internal sealed class DataGridColumnValueAccessor
{
    private static readonly ConcurrentPropertyCache PropertyCache = new();

    private readonly string[] segments;
    private readonly DescriptorCache?[] descriptorCaches;
    private readonly IValueConverter? converter;
    private readonly object? converterParameter;
    private readonly string? stringFormat;
    private readonly CultureInfo culture;

    private DataGridColumnValueAccessor(
        string path,
        IValueConverter? converter,
        object? converterParameter,
        string? stringFormat,
        CultureInfo culture)
    {
        segments = string.IsNullOrEmpty(path) || path == "."
            ? []
            : path.Split('.');
        descriptorCaches = new DescriptorCache?[segments.Length];
        this.converter = converter;
        this.converterParameter = converterParameter;
        this.stringFormat = stringFormat;
        this.culture = culture;
    }

    /// <summary>
    /// Creates the accessor of a column, or returns <see langword="null"/> when the column has no usable path
    /// (e.g. a template column without a <see cref="DataGridColumn.SortMemberPath"/>, or an indexer path).
    /// </summary>
    public static DataGridColumnValueAccessor? Create(
        DataGridColumn column,
        CultureInfo gridCulture)
    {
        ArgumentNullException.ThrowIfNull(column);

        var binding = GetBinding(column);
        var path = binding?.Path?.Path;
        if (string.IsNullOrEmpty(path))
        {
            binding = null;
            path = column.SortMemberPath;
        }

        if (string.IsNullOrEmpty(path) ||
            path.Contains('[', StringComparison.Ordinal) ||
            path.Contains('(', StringComparison.Ordinal))
        {
            return null;
        }

        var format = binding?.StringFormat;
        return new DataGridColumnValueAccessor(
            path,
            binding?.Converter,
            binding?.ConverterParameter,
            string.IsNullOrEmpty(format) ? null : format,
            binding?.ConverterCulture ?? gridCulture);
    }

    /// <summary>
    /// Gets the property name of a one-step path (the column name of a <see cref="System.Data.DataView"/> row),
    /// or <see langword="null"/> for a longer or empty path.
    /// </summary>
    public string? SinglePropertyName
        => segments.Length == 1 ? segments[0] : null;

    /// <summary>
    /// Reads the raw value of the column from a row item; <see langword="null"/> when a step of the path is missing.
    /// </summary>
    /// <remarks>
    /// A step reads a <see cref="System.Data.DataRowView"/> column, a dictionary entry (e.g. an
    /// <see cref="System.Dynamic.ExpandoObject"/>), a public property, or a property the item describes through
    /// <see cref="TypeDescriptor"/> (e.g. a <see cref="ICustomTypeDescriptor"/>), in that order.
    /// </remarks>
    public object? GetValue(object? item)
    {
        var current = item;
        for (var i = 0; i < segments.Length; i++)
        {
            if (current is null || current == DBNull.Value)
            {
                return null;
            }

            current = GetStep(current, i);
        }

        return current == DBNull.Value ? null : current;
    }

    /// <summary>
    /// Formats a raw value as the cell shows it; blank values give an empty string.
    /// </summary>
    public string Format(object? value)
    {
        if (converter is not null)
        {
            value = converter.Convert(value, typeof(object), converterParameter, culture);
            if (value == DependencyProperty.UnsetValue || value == Binding.DoNothing)
            {
                value = null;
            }
        }

        if (value is null)
        {
            return string.Empty;
        }

        if (stringFormat is not null)
        {
            try
            {
                return stringFormat.Contains('{', StringComparison.Ordinal)
                    ? string.Format(culture, stringFormat, value)
                    : value is IFormattable formattableValue
                        ? formattableValue.ToString(stringFormat, culture)
                        : value.ToString() ?? string.Empty;
            }
            catch (FormatException)
            {
                // Fall through to the plain text, as the binding would show the unformatted value.
            }
        }

        return value is IFormattable formattable
            ? formattable.ToString(format: null, culture)
            : value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Reads and formats the cell text of a row item.
    /// </summary>
    public string GetText(object? item)
        => Format(GetValue(item));

    private object? GetStep(
        object current,
        int index)
    {
        var segment = segments[index];
        switch (current)
        {
            case System.Data.DataRowView rowView:
            {
                var columns = rowView.Row.Table.Columns;
                return columns.Contains(segment)
                    ? rowView.Row[segment]
                    : GetDescribedValue(current, index);
            }

            case IDictionary<string, object?> dictionary:
                return dictionary.TryGetValue(segment, out var value) ? value : null;
        }

        var property = PropertyCache.Get(current.GetType(), segment);
        return property is not null
            ? property.GetValue(current)
            : GetDescribedValue(current, index);
    }

    private object? GetDescribedValue(
        object current,
        int index)
    {
        // Items of one type usually share one descriptor collection: look the property up once per collection.
        var properties = TypeDescriptor.GetProperties(current);
        var cache = descriptorCaches[index];
        if (!ReferenceEquals(cache?.Properties, properties))
        {
            cache = new DescriptorCache(properties, properties.Find(segments[index], ignoreCase: false));
            descriptorCaches[index] = cache;
        }

        return cache.Descriptor?.GetValue(current);
    }

    private static Binding? GetBinding(DataGridColumn column)
        => column switch
        {
            DataGridBoundColumn bound => bound.Binding as Binding,
            DataGridComboBoxColumn combo => (combo.TextBinding ?? combo.SelectedValueBinding ?? combo.SelectedItemBinding) as Binding,
            _ => null,
        };

    private sealed record DescriptorCache(
        PropertyDescriptorCollection Properties,
        PropertyDescriptor? Descriptor);

    private sealed class ConcurrentPropertyCache
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<(Type Type, string Name), PropertyInfo?> cache = new();

        public PropertyInfo? Get(
            Type type,
            string name)
            => cache.GetOrAdd(
                (type, name),
                static key => key.Type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetIndexParameters().Length == 0 &&
                                         string.Equals(x.Name, key.Name, StringComparison.Ordinal)));
    }
}