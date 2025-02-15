namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class EnumerableExtensions
{
    public static string? ExtractParameterValueFromList(
        this IEnumerable<string>? parameters,
        string name)
        => parameters?
            .FirstOrDefault(x => x.StartsWith(name, StringComparison.Ordinal))?
            .Replace(name, string.Empty)
            .Replace("=", string.Empty)
            .Trim();
}