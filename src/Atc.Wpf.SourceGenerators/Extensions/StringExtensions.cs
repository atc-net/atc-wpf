namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class StringExtensions
{
    public static string EnsureFirstCharacterToUpper(
        this string value)
        => value.Length switch
        {
            0 => value,
            1 => value.ToUpperInvariant(),
            _ => char.ToUpperInvariant(value[0]) + value.Substring(1),
        };
}