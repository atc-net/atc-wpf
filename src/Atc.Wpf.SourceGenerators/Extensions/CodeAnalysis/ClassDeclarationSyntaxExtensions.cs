namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class ClassDeclarationSyntaxExtensions
{
    public static string GetNamespace(
        this ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration is null)
        {
            throw new ArgumentNullException(nameof(classDeclaration));
        }

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

    public static bool HasBaseClassWithName(
        this ClassDeclarationSyntax classDeclaration,
        params string[] baseClassNames)
    {
        if (classDeclaration is null)
        {
            throw new ArgumentNullException(nameof(classDeclaration));
        }

        if (classDeclaration.BaseList is null)
        {
            return false;
        }

        return classDeclaration.BaseList.Types
            .Select(baseType => baseType.Type)
            .OfType<IdentifierNameSyntax>()
            .Any(identifier => baseClassNames.Contains(identifier.Identifier.Text, StringComparer.Ordinal));
    }
}