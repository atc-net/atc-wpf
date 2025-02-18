namespace Atc.Wpf.SourceGenerators.Inspectors;

internal static class ViewModelInspector
{
    internal static ViewModelInspectorResult Inspect(
        INamedTypeSymbol viewModelClassSymbol)
    {
        var propertiesToGenerate = ObservablePropertyInspector.Inspect(viewModelClassSymbol);
        var relayCommandsToGenerate = RelayCommandInspector.Inspect(viewModelClassSymbol);

        return new ViewModelInspectorResult(
            propertiesToGenerate,
            relayCommandsToGenerate);
    }
}