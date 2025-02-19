namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class FrameworkElementInspectorResult(
    bool isStatic,
    List<AttachedPropertyToGenerate> attachedPropertiesToGenerate,
    List<DependencyPropertyToGenerate> dependencyPropertiesToGenerate,
    List<RelayCommandToGenerate> relayCommandsToGenerate)
{
    public List<AttachedPropertyToGenerate> AttachedPropertiesToGenerate { get; } = attachedPropertiesToGenerate;

    public List<DependencyPropertyToGenerate> DependencyPropertiesToGenerate { get; } = dependencyPropertiesToGenerate;

    public List<RelayCommandToGenerate> RelayCommandsToGenerate { get; } = relayCommandsToGenerate;

    public bool FoundAnythingToGenerate
        => AttachedPropertiesToGenerate.Count > 0 ||
           DependencyPropertiesToGenerate.Count > 0 ||
           RelayCommandsToGenerate.Count > 0;

    public bool IsStatic { get; } = isStatic;

    public override string ToString()
        => $"{nameof(IsStatic)}: {IsStatic}, {nameof(AttachedPropertiesToGenerate)}.Count: {AttachedPropertiesToGenerate.Count}, {nameof(DependencyPropertiesToGenerate)}.Count: {DependencyPropertiesToGenerate.Count}, {nameof(DependencyPropertiesToGenerate)}.Count: {DependencyPropertiesToGenerate.Count}";
}