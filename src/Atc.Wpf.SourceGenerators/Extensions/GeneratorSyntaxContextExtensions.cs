namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class GeneratorSyntaxContextExtensions
{
    internal static INamedTypeSymbol GetNamedTypeSymbolFromClassDeclarationSyntax(
        this GeneratorSyntaxContext context)
        => context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node)!;
}