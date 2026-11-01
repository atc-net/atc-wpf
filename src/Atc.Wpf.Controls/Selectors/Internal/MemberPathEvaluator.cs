#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Reads a (dotted) public property path, such as <c>Id</c> or <c>Info.Id</c>, from an object by reflection.
/// </summary>
internal static class MemberPathEvaluator
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type Type, string Name), PropertyInfo?> Cache = new();

    /// <summary>
    /// Returns the value at <paramref name="path"/> on <paramref name="source"/>, or <see langword="null"/> when the
    /// path is empty, a step is missing, or a step returns <see langword="null"/>.
    /// </summary>
    public static object? GetValue(
        object? source,
        string? path)
    {
        if (source is null || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var current = source;
        foreach (var name in path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var property = GetProperty(current.GetType(), name);
            if (property is null)
            {
                return null;
            }

            current = property.GetValue(current);
            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    /// <summary>
    /// Returns the value at <paramref name="path"/> as an invariant string, or <see langword="null"/>.
    /// </summary>
    public static string? GetString(
        object? source,
        string? path)
        => GetValue(source, path) is { } value
            ? Convert.ToString(value, CultureInfo.InvariantCulture)
            : null;

    private static PropertyInfo? GetProperty(
        Type type,
        string name)
        => Cache.GetOrAdd((type, name), static key => key.Type.GetProperty(key.Name, BindingFlags.Public | BindingFlags.Instance));
}