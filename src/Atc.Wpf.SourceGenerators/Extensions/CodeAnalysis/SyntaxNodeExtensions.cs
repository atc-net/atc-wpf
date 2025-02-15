// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class SyntaxNodeExtensions
{
    public static bool HasPublicPartialClassDeclaration(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration)
        {
            return false;
        }

        return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)) &&
               classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    public static bool HasPublicPartialClassDeclarationWithIdentifierContainsViewModel(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration)
        {
            return false;
        }

        return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)) &&
               classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)) &&
               classDeclaration.Identifier.Text.Contains(NameConstants.ViewModel);
    }

    public static bool HasClassDeclarationWithValidObservableFields(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        var foundValidCandidate = false;

        foreach (var memberDeclaratorSyntax in classDeclarationSyntax.Members)
        {
            if (memberDeclaratorSyntax is FieldDeclarationSyntax fieldDeclaration)
            {
                if (!fieldDeclaration.HasObservablePropertyAttribute())
                {
                    continue;
                }

                if (!fieldDeclaration.IsValidObservableBackingField())
                {
                    return false;
                }

                foundValidCandidate = true;
            }
        }

        return foundValidCandidate;
    }

    public static bool HasClassDeclarationWithValidRelayCommandMethods(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        var foundValidCandidate = false;
        foreach (var memberDeclaratorSyntax in classDeclarationSyntax.Members)
        {
            if (memberDeclaratorSyntax is MethodDeclarationSyntax methodDeclaration)
            {
                if (!methodDeclaration.HasRelayCommandAttribute())
                {
                    continue;
                }

                if (!methodDeclaration.IsValidRelayCommandMethod())
                {
                    return false;
                }

                foundValidCandidate = true;
            }
        }

        return foundValidCandidate;
    }

    public static bool HasClassDeclarationWithValidDependencyProperties(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        var foundValidCandidate = false;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            var dependencyPropertyAttributes = attributeListSyntax.Attributes
                .Where(x => x.Name.ToString().StartsWith(NameConstants.DependencyProperty, StringComparison.Ordinal));

            if (dependencyPropertyAttributes.Any())
            {
                foundValidCandidate = true;
            }
        }

        return foundValidCandidate;
    }

    public static bool HasClassDeclarationWithValidAttachedProperties(
        this SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        var foundValidCandidate = false;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            var dependencyPropertyAttributes = attributeListSyntax.Attributes
                .Where(x => x.Name.ToString().StartsWith(NameConstants.AttachedProperty, StringComparison.Ordinal));

            if (dependencyPropertyAttributes.Any())
            {
                foundValidCandidate = true;
            }
        }

        return foundValidCandidate;
    }
}