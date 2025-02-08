namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class GeneratorSyntaxContextExtensions
{
    internal static INamedTypeSymbol GetNamedTypeSymbolFromClassDeclarationSyntax(
        this GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        return (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;
    }
}