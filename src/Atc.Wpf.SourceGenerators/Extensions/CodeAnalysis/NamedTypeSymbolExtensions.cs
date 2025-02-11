namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class NamedTypeSymbolExtensions
{
    public static string GetAccessModifier(
        this INamedTypeSymbol namedTypeSymbol)
        => namedTypeSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            _ => string.Empty,
        };

    public static bool IsInheritsFromIObservableObject(
        this INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.AllInterfaces.Length == 0)
        {
            // Syntax check
            if (namedTypeSymbol.BaseType?.ToString() == NameConstants.ViewModelBase ||
                namedTypeSymbol.BaseType?.ToString() == NameConstants.MainWindowViewModelBase ||
                namedTypeSymbol.BaseType?.ToString() == NameConstants.ViewModelDialogBase ||
                namedTypeSymbol.BaseType?.ToString() == NameConstants.ObservableObject)
            {
                return true;
            }
        }
        else
        {
            // Runtime check
            return namedTypeSymbol.AllInterfaces.Any(x => x.Name == NameConstants.IObservableObject);
        }

        return false;
    }
}