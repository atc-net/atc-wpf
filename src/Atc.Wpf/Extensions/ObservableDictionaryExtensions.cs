// ReSharper disable CheckNamespace
namespace Atc.Wpf.Collections;

public static class ObservableDictionaryExtensions
{
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