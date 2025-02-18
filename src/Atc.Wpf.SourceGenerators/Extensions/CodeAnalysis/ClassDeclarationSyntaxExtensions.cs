namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class ClassDeclarationSyntaxExtensions
{
    public static string GetNamespace(
        this ClassDeclarationSyntax classDeclaration)
    {
        // Check for file-scoped namespace declaration (C# 10+)
        var fileScopedNamespace = classDeclaration.SyntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<FileScopedNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        if (fileScopedNamespace is not null)
        {
            return fileScopedNamespace.Name.ToString();
        }

        // Check for traditional namespace declaration
        var namespaceDeclaration = classDeclaration.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault();

        return namespaceDeclaration?.Name.ToString() ?? string.Empty;
    }

    public static bool HasBaseClassFromList(
        this IEnumerable<ClassDeclarationSyntax> partialDeclarations,
        GeneratorSyntaxContext context,
        params string[] baseClassNames)
        => partialDeclarations.Any(declaration =>
        {
            var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(declaration.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(declaration);
            return symbol?.InheritsFrom(baseClassNames) == true;
        });

    public static bool HasBaseClassFromFrameworkElementOrEndsOnAttachOrBehavior(
        this IEnumerable<ClassDeclarationSyntax> declarations,
        GeneratorSyntaxContext context)
        => declarations.Any(declaration =>
        {
            var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(declaration.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(declaration);

            return symbol?.InheritsFrom(
                       NameConstants.UserControl,
                       NameConstants.DependencyObject,
                       NameConstants.FrameworkElement) == true ||
                   symbol?.Name.EndsWith(NameConstants.Attach, StringComparison.Ordinal) == true ||
                   symbol?.Name.EndsWith(NameConstants.Behavior, StringComparison.Ordinal) == true;
        });
}