namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class CollectionExtensions
{
    public static ICollection<string>? RemoveIsExist(
        this ICollection<string>? list,
        string value)
    {
        if (list is null)
        {
            return null;
        }

        list.Remove(value);

        return list;
    }
}