// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class FrameworkElementToGenerate(
    string namespaceName,
    string className,
    string accessModifier,
    bool isStatic)
    : GenerateBase(
        namespaceName,
        className,
        accessModifier,
        isStatic)
{
    public IList<DependencyPropertyToGenerate>? DependencyPropertiesToGenerate { get; set; }

    public IList<AttachedPropertyToGenerate>? AttachedPropertiesToGenerate { get; set; }

    public override string ToString()
        => $"{nameof(DependencyPropertiesToGenerate)}.Count: {DependencyPropertiesToGenerate?.Count}, {nameof(AttachedPropertiesToGenerate)}.Count: {AttachedPropertiesToGenerate?.Count}";
}