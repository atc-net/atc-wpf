namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class CompilationExtensions
{
    public static IList<ClassDeclarationSyntax> GetAllPartialClassDeclarations(
        this Compilation compilation,
        INamedTypeSymbol classSymbol)
    {
        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        if (classSymbol is null)
        {
            throw new ArgumentNullException(nameof(classSymbol));
        }

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