// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class FrameworkElementToGenerate(
    string namespaceName,
    string className,
    string accessModifier)
    : GenerateBase(
        namespaceName,
        className,
        accessModifier)
{
    public IList<DependencyPropertyToGenerate>? DependencyPropertiesToGenerate { get; set; }
}