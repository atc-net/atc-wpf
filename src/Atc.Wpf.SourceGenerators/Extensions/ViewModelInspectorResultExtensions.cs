namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class ViewModelInspectorResultExtensions
{
    public static void ApplyCommandsAndProperties(
        this ViewModelInspectorResult result,
        List<RelayCommandToGenerate> allCommands,
        List<ObservablePropertyToGenerate> allProperties)
    {
        if (allCommands.Count == 0)
        {
            allCommands.AddRange(result.RelayCommandsToGenerate);
        }
        else
        {
            foreach (var relayCommandToGenerate in result.RelayCommandsToGenerate.Where(rc => allCommands.Find(x => x.CommandName == rc.CommandName) is null))
            {
                allCommands.Add(relayCommandToGenerate);
            }
        }

        if (allProperties.Count == 0)
        {
            allProperties.AddRange(result.PropertiesToGenerate);
        }
        else
        {
            foreach (var propertyToGenerate in result.PropertiesToGenerate.Where(p => allProperties.Find(x => x.BackingFieldName == p.BackingFieldName) is null))
            {
                allProperties.Add(propertyToGenerate);
            }
        }
    }
}