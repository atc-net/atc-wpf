namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class MethodDeclarationSyntaxExtensions
{
    public static bool HasRelayCommandAttribute(
        this MethodDeclarationSyntax methodDeclarationSyntax)
        => methodDeclarationSyntax.AttributeLists
            .SelectMany(attrList => attrList.Attributes)
            .Any(attribute =>
            {
                var attributeName = attribute.Name.GetSimpleAttributeName();
                return attributeName
                    is NameConstants.RelayCommandAttribute
                    or NameConstants.RelayCommand;
            });

    public static bool IsValidRelayCommandMethod(
        this MethodDeclarationSyntax methodDeclarationSyntax)
        => methodDeclarationSyntax.HasRelayCommandAttribute();
}