// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.ViewModel;

internal sealed class ViewModelToGenerate(
    string namespaceName,
    string className,
    string accessModifier)
    : GenerateBase(
        namespaceName,
        className,
        accessModifier,
        isStatic: false)
{
    public IList<ObservablePropertyToGenerate>? PropertiesToGenerate { get; set; }

    public IList<RelayCommandToGenerate>? RelayCommandsToGenerate { get; set; }

    public bool ContainsRelayCommandNameDuplicates
    {
        get
        {
            if (RelayCommandsToGenerate is null)
            {
                return false;
            }

            var names = RelayCommandsToGenerate.Select(x => x.CommandName).ToArray();
            return names.Length != names.Distinct(StringComparer.Ordinal).Count();
        }
    }

    public override string ToString()
        => $"{base.ToString()}, {nameof(PropertiesToGenerate)}.Count: {PropertiesToGenerate?.Count}, {nameof(RelayCommandsToGenerate)}.Count: {RelayCommandsToGenerate?.Count}";
}