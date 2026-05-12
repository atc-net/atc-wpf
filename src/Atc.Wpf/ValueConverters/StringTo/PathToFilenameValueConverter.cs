// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: file-system path → filename component (e.g. <c>"C:\foo\bar.txt" → "bar.txt"</c>).
/// </summary>
/// <remarks>
/// Uses <see cref="Path.GetFileName(string)"/>, so Windows and Unix-style separators both work.
/// <c>ConverterParameter=WithoutExtension</c> (case-insensitive) drops the extension —
/// <c>"bar.txt" → "bar"</c>. Null/empty input returns <see cref="string.Empty"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding RecentFilePath,
///     Converter={x:Static converters:PathToFilenameValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(string), typeof(string))]
public sealed class PathToFilenameValueConverter : IValueConverter
{
    public static readonly PathToFilenameValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var path = value?.ToString();
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        var fileName = Path.GetFileName(path);

        if (parameter is string p &&
            string.Equals(p, "WithoutExtension", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        return fileName;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}