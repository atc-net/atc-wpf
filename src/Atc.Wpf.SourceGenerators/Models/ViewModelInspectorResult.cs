namespace Atc.Wpf.SourceGenerators.Models;

internal sealed class ViewModelInspectorResult(
    List<RelayCommandToGenerate> relayCommandsToGenerate,
    List<PropertyToGenerate> propertiesToGenerate)
{
    public List<RelayCommandToGenerate> RelayCommandsToGenerate { get; } = relayCommandsToGenerate;

    public List<PropertyToGenerate> PropertiesToGenerate { get; } = propertiesToGenerate;

    public bool FoundAnythingToGenerate
        => RelayCommandsToGenerate.Count > 0 ||
           PropertiesToGenerate.Count > 0;

    public override string ToString()
        => $"{nameof(RelayCommandsToGenerate)}.Count: {RelayCommandsToGenerate.Count}, {nameof(PropertiesToGenerate)}.Count: {PropertiesToGenerate.Count}, {nameof(FoundAnythingToGenerate)}: {FoundAnythingToGenerate}";
}