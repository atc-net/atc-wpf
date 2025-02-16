namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class ImmutableArrayExtensions
{
    public static bool HasPropertyName(
        this ImmutableArray<ISymbol> memberSymbols,
        string name)
        => memberSymbols.FirstOrDefault(x => x is IPropertySymbol ps && ps.Name == name) is not null;

    public static bool HasObservableFieldName(
        this ImmutableArray<ISymbol> memberSymbols,
        string name)
    {
        foreach (var memberSymbol in memberSymbols)
        {
            if (memberSymbol is IFieldSymbol fieldSymbol &&
                (fieldSymbol.HasObservableFieldName(name) ||
                 fieldSymbol.HasObservablePropertyName(name)))
            {
                return true;
            }
        }

        return false;
    }
}