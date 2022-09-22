namespace Atc.Wpf.Theming.Helper;

public static class AtcAppsCommands
{
    static AtcAppsCommands()
    {
    }

    public static ICommand ClearControlCommand { get; } = new RoutedUICommand("Clear", nameof(ClearControlCommand), typeof(AtcAppsCommands));

    public static ICommand SearchCommand { get; } = new RoutedUICommand("Search", nameof(SearchCommand), typeof(AtcAppsCommands));
}