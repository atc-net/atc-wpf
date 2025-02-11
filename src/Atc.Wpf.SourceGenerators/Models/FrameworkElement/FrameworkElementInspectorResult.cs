namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class FrameworkElementInspectorResult(
    List<DependencyPropertyToGenerate> dependencyPropertiesToGenerate)
{
    public List<DependencyPropertyToGenerate> DependencyPropertiesToGenerate { get; } = dependencyPropertiesToGenerate;

    public bool FoundAnythingToGenerate
        => DependencyPropertiesToGenerate.Count > 0;

    public override string ToString()
        => $"{nameof(DependencyPropertiesToGenerate)}.Count: {DependencyPropertiesToGenerate.Count}";
}