namespace Atc.Wpf.SourceGenerators.Models;

internal sealed class ViewModelInspectorResult(
    List<PropertyToGenerate> propertiesToGenerate)
{
    public List<PropertyToGenerate> PropertiesToGenerate { get; } = propertiesToGenerate;

    public bool FoundAnythingToGenerate
        => PropertiesToGenerate.Count > 0;
}