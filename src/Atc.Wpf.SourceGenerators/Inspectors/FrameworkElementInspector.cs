namespace Atc.Wpf.SourceGenerators.Inspectors;

internal static class FrameworkElementInspector
{
    public static FrameworkElementInspectorResult Inspect(
        INamedTypeSymbol classSymbol)
    {
        var attachedPropertiesToGenerate = AttachedPropertyInspector.Inspect(classSymbol);

        var dependencyPropertiesToGenerate = DependencyPropertyInspector.Inspect(classSymbol);

        var relayCommandsToGenerate = RelayCommandInspector.Inspect(classSymbol);

        return new FrameworkElementInspectorResult(
            classSymbol.IsStatic,
            attachedPropertiesToGenerate,
            dependencyPropertiesToGenerate,
            relayCommandsToGenerate);
    }
}