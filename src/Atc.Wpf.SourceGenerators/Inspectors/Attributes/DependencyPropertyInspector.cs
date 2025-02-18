namespace Atc.Wpf.SourceGenerators.Inspectors.Attributes;

internal static class DependencyPropertyInspector
{
    public static List<DependencyPropertyToGenerate> Inspect(
        INamedTypeSymbol classSymbol)
    {
        var attributes = classSymbol.GetAttributes();

        var propertyAttributes = attributes
            .Where(x => x.AttributeClass?.Name
                is NameConstants.DependencyPropertyAttribute
                or NameConstants.DependencyProperty);

        return FrameworkElementInspectorHelper.InspectPropertyAttributes<DependencyPropertyToGenerate>(
            classSymbol,
            propertyAttributes);
    }
}