namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class ImmutableArrayExtensions
{
    public static bool HasPropertyName(
        this ImmutableArray<ISymbol> memberSymbols,
        string name)
        => memberSymbols.FirstOrDefault(x => x is IPropertySymbol ps && ps.Name == name) is not null;
}