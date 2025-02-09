namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class GeneratorSyntaxContextExtensions
{
    internal static INamedTypeSymbol GetNamedTypeSymbolFromClassDeclarationSyntax(
        this GeneratorSyntaxContext context)
        => context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node)!;
}