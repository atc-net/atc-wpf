namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class CompilationExtensions
{
    public static IList<ClassDeclarationSyntax> GetAllPartialClassDeclarations(
        this Compilation compilation,
        INamedTypeSymbol classSymbol)
    {
        var targetNamespace = classSymbol.ContainingNamespace.ToDisplayString();

        return compilation.SyntaxTrees
            .SelectMany(tree => tree.GetRoot().DescendantNodes())
            .OfType<ClassDeclarationSyntax>()
            .Where(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)) &&
                        c.Identifier.Text == classSymbol.Name &&
                        c.GetNamespace() == targetNamespace)
            .ToList();
    }
}