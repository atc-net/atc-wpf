// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

public static class SystemExtension
{
    public static T Clone<T>(this T source)
    {
        var serialized = System.Text.Json.JsonSerializer.Serialize(source);
        return System.Text.Json.JsonSerializer.Deserialize<T>(serialized)!;
    }
}