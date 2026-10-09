namespace Atc.Wpf.Controls.Progressing.Internal;

/// <summary>Extension methods for finding visual state groups and visual states.</summary>
public static class VisualStateExtensions
{
    /// <summary>Gets the element's visual state groups named for the active states of a loading indicator.</summary>
    public static IEnumerable<VisualStateGroup> GetActiveVisualStateGroups(
        this FrameworkElement element)
        => element
            .GetVisualStateGroupsByName(IndicatorVisualStateGroupNames.ActiveStates.Name);

    /// <summary>Gets the element's visual state groups with the specified name, or all groups when the name is empty.</summary>
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
            castedVisualStateGroups = groups
                .Cast<VisualStateGroup>()
                .ToArray();
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

    /// <summary>Gets the active visual states of the element's active state groups.</summary>
    public static IEnumerable<VisualState> GetActiveVisualStates(
        this FrameworkElement element)
        => element
            .GetActiveVisualStateGroups()
            .GetAllVisualStatesByName(IndicatorVisualStateNames.ActiveState.Name);

    /// <summary>Gets the visual states with the specified name from all the given groups.</summary>
    public static IEnumerable<VisualState> GetAllVisualStatesByName(
        this IEnumerable<VisualStateGroup> visualStateGroups,
        string name)
        => visualStateGroups.SelectMany(vsg => vsg.GetVisualStatesByName(name)!);

    /// <summary>Gets the visual states with the specified name in the group, or all its states when the name is empty.</summary>
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

    /// <summary>Gets all visual states in the group.</summary>
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