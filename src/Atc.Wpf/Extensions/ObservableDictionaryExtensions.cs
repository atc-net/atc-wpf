// ReSharper disable CheckNamespace
namespace Atc.Wpf.Collections;

/// <summary>
/// Extension methods for <see cref="ObservableDictionary{TKey, TValue}"/>.
/// </summary>
public static class ObservableDictionaryExtensions
{
    /// <summary>
    /// Copies the entries into a new dictionary of strings.
    /// </summary>
    public static Dictionary<string, string> ToDictionaryOfStrings(
        this ObservableDictionary<string, string> keyValues)
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var data = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var keyValue in keyValues)
        {
            data.Add(keyValue.Key, keyValue.Value);
        }

        return data;
    }

    /// <summary>
    /// Copies the entries into a new dictionary of strings, converting the integer keys with the invariant English culture.
    /// </summary>
    public static Dictionary<string, string> ToDictionaryOfStrings(
        this ObservableDictionary<int, string> keyValues)
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var data = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var keyValue in keyValues)
        {
            data.Add(keyValue.Key.ToString(GlobalizationConstants.EnglishCultureInfo), keyValue.Value);
        }

        return data;
    }

    /// <summary>
    /// Copies the entries into a new dictionary of strings, converting the GUID keys to strings.
    /// </summary>
    public static Dictionary<string, string> ToDictionaryOfStrings(
        this ObservableDictionary<Guid, string> keyValues)
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var data = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var keyValue in keyValues)
        {
            data.Add(keyValue.Key.ToString(), keyValue.Value);
        }

        return data;
    }
}