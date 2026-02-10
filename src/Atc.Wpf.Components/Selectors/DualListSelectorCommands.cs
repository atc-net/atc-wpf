namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Provides routed commands for the <see cref="DualListSelector"/> control.
/// </summary>
public static class DualListSelectorCommands
{
    public static readonly RoutedCommand MoveToSelected = new(nameof(MoveToSelected), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveToAvailable = new(nameof(MoveToAvailable), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveAllToSelected = new(nameof(MoveAllToSelected), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveAllToAvailable = new(nameof(MoveAllToAvailable), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveToTop = new(nameof(MoveToTop), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveUp = new(nameof(MoveUp), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveDown = new(nameof(MoveDown), typeof(DualListSelectorCommands));

    public static readonly RoutedCommand MoveToBottom = new(nameof(MoveToBottom), typeof(DualListSelectorCommands));
}