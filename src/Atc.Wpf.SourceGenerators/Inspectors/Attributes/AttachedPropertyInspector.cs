namespace Atc.Wpf.SourceGenerators.Inspectors.Attributes;

internal static class AttachedPropertyInspector
{
    public static List<AttachedPropertyToGenerate> Inspect(
        INamedTypeSymbol classSymbol)
    {
        var attributes = classSymbol.GetAttributes();

        var propertyAttributes = attributes
            .Where(x => x.AttributeClass?.Name
                is NameConstants.AttachedPropertyAttribute
                or NameConstants.AttachedProperty);

        return FrameworkElementInspectorHelper.InspectPropertyAttributes<AttachedPropertyToGenerate>(
            classSymbol,
            propertyAttributes);
    }
}