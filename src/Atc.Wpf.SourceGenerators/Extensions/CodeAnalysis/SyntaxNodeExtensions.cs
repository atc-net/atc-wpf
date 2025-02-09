// ReSharper disable InvertIf
namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class SyntaxNodeExtensions
{
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
}