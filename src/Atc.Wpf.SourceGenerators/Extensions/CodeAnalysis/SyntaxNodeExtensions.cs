// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class SyntaxNodeExtensions
{
    public static bool HasPartialClassDeclaration(
        this SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDeclaration &&
           classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
}