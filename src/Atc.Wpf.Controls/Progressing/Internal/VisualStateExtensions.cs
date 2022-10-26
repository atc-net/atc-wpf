namespace Atc.Wpf.Controls.Progressing.Internal;

public static class VisualStateExtensions
{
    public static IEnumerable<VisualStateGroup> GetActiveVisualStateGroups(
        this FrameworkElement element)
        => element
            .GetVisualStateGroupsByName(IndicatorVisualStateGroupNames.ActiveStates.Name);

    public static IEnumerable<VisualStateGroup> GetVisualStateGroupsByName(
        this FrameworkElement element,
        string name)
    {
        var groups = VisualStateManager.GetVisualStateGroups(element);

        if (groups is null)
        {
            return Array.Empty<VisualStateGroup>();
        }

        IEnumerable<VisualStateGroup> castedVisualStateGroups;

        try
        {
            castedVisualStateGroups = groups.Cast<VisualStateGroup>().ToArray();
            if (!castedVisualStateGroups.Any())
            {
                return Array.Empty<VisualStateGroup>();
            }
        }
        catch (InvalidCastException)
        {
            return Array.Empty<VisualStateGroup>();
        }

        return string.IsNullOrWhiteSpace(name)
            ? castedVisualStateGroups
            : castedVisualStateGroups.Where(vsg => vsg.Name == name);
    }

    public static IEnumerable<VisualState> GetActiveVisualStates(
        this FrameworkElement element)
        => element
            .GetActiveVisualStateGroups()
            .GetAllVisualStatesByName(IndicatorVisualStateNames.ActiveState.Name);

    public static IEnumerable<VisualState> GetAllVisualStatesByName(
        this IEnumerable<VisualStateGroup> visualStateGroups,
        string name)
        => visualStateGroups.SelectMany(vsg => vsg.GetVisualStatesByName(name)!);

    public static IEnumerable<VisualState>? GetVisualStatesByName(
        this VisualStateGroup? visualStateGroup,
        string name)
    {
        if (visualStateGroup is null)
        {
            return Array.Empty<VisualState>();
        }

        var visualStates = visualStateGroup.GetVisualStates();

        return string.IsNullOrWhiteSpace(name)
            ? visualStates
            : visualStates.Where(vs => vs.Name == name);
    }

    public static IEnumerable<VisualState> GetVisualStates(
        this VisualStateGroup? visualStateGroup)
    {
        if (visualStateGroup is null)
        {
            return Array.Empty<VisualState>();
        }

        return visualStateGroup.States.Count == 0
            ? Array.Empty<VisualState>()
            : visualStateGroup.States.Cast<VisualState>();
    }
}