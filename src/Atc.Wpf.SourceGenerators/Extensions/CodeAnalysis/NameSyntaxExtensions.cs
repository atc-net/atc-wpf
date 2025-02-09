namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class NameSyntaxExtensions
{
    public static string GetSimpleAttributeName(
        this NameSyntax nameSyntax)
        => nameSyntax switch
        {
            QualifiedNameSyntax qns => qns.Right.Identifier.Text,
            IdentifierNameSyntax ins => ins.Identifier.Text,
            _ => nameSyntax.ToString(),
        };
}