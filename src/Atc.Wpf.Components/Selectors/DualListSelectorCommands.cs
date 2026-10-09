namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Provides routed commands for the <see cref="DualListSelector"/> control.
/// </summary>
public static class DualListSelectorCommands
{
    /// <summary>
    /// Moves the highlighted available items to the selected list.
    /// </summary>
    public static readonly RoutedCommand MoveToSelected = new(nameof(MoveToSelected), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves the highlighted selected items back to the available list.
    /// </summary>
    public static readonly RoutedCommand MoveToAvailable = new(nameof(MoveToAvailable), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves all available items to the selected list.
    /// </summary>
    public static readonly RoutedCommand MoveAllToSelected = new(nameof(MoveAllToSelected), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves all selected items back to the available list.
    /// </summary>
    public static readonly RoutedCommand MoveAllToAvailable = new(nameof(MoveAllToAvailable), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves the highlighted item in the selected list to the top.
    /// </summary>
    public static readonly RoutedCommand MoveToTop = new(nameof(MoveToTop), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves the highlighted item in the selected list up one position.
    /// </summary>
    public static readonly RoutedCommand MoveUp = new(nameof(MoveUp), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves the highlighted item in the selected list down one position.
    /// </summary>
    public static readonly RoutedCommand MoveDown = new(nameof(MoveDown), typeof(DualListSelectorCommands));

    /// <summary>
    /// Moves the highlighted item in the selected list to the bottom.
    /// </summary>
    public static readonly RoutedCommand MoveToBottom = new(nameof(MoveToBottom), typeof(DualListSelectorCommands));
}