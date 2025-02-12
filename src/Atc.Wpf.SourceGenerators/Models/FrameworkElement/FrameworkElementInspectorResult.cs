namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class FrameworkElementInspectorResult(
    List<DependencyPropertyToGenerate> dependencyPropertiesToGenerate,
    List<AttachedPropertyToGenerate> attachedPropertiesToGenerate)
{
    public List<DependencyPropertyToGenerate> DependencyPropertiesToGenerate { get; } = dependencyPropertiesToGenerate;
    public List<AttachedPropertyToGenerate> AttachedPropertiesToGenerate { get; } = attachedPropertiesToGenerate;

    public bool FoundAnythingToGenerate
        => DependencyPropertiesToGenerate.Count > 0 ||
           AttachedPropertiesToGenerate.Count > 0;

    public bool IsStatic => AttachedPropertiesToGenerate.Count > 0;

    public override string ToString()
        => $"{nameof(IsStatic)}: {IsStatic}, {nameof(DependencyPropertiesToGenerate)}.Count: {DependencyPropertiesToGenerate.Count}, {nameof(AttachedPropertiesToGenerate)}.Count: {AttachedPropertiesToGenerate.Count}";
}