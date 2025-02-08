namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class FieldDeclarationSyntaxExtensions
{
    public static bool HasObservablePropertyAttribute(
        this FieldDeclarationSyntax fieldDeclarationSyntax)
        => fieldDeclarationSyntax.AttributeLists
            .SelectMany(attrList => attrList.Attributes)
            .Any(attribute => attribute.Name.ToString()
                is NameConstants.ObservablePropertyAttribute
                or NameConstants.ObservableProperty);

    public static bool HasValidBackingFieldAccessor(
        this FieldDeclarationSyntax fieldDeclarationSyntax)
    {
        var modifiers = fieldDeclarationSyntax.Modifiers;

        var isExplicitlyPrivate = modifiers
            .Any(m => m.IsKind(SyntaxKind.PrivateKeyword));

        var hasAccessibilityModifier = modifiers
            .Any(m =>
                m.IsKind(SyntaxKind.PublicKeyword) ||
                m.IsKind(SyntaxKind.ProtectedKeyword) ||
                m.IsKind(SyntaxKind.InternalKeyword));

        return isExplicitlyPrivate || !hasAccessibilityModifier;
    }

    public static bool IsValidObservableBackingField(
        this FieldDeclarationSyntax fieldDeclarationSyntax)
        => fieldDeclarationSyntax.HasObservablePropertyAttribute() &&
           fieldDeclarationSyntax.HasValidBackingFieldAccessor() &&
           !fieldDeclarationSyntax.Modifiers.Any(m =>
               m.IsKind(SyntaxKind.StaticKeyword) ||
               m.IsKind(SyntaxKind.ReadOnlyKeyword) ||
               m.IsKind(SyntaxKind.ConstKeyword)) &&
           fieldDeclarationSyntax.Declaration.Variables.All(x => x.HasValidBackingFieldName());
}