namespace Atc.Wpf.Controls.Internal;

/// <summary>
/// Reports a control that loads without its style. The styles live in Controls.xaml, which the application must
/// merge; without it the controls render nothing, so the first load of each such control type is traced with the
/// dictionary to merge.
/// </summary>
internal static class StyleCheck
{
    private const string ControlsDictionary = "pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml";

    private static readonly ConcurrentDictionary<Type, byte> RegisteredTypes = new();

    /// <summary>
    /// Call from the static constructor that sets the control's default style key.
    /// </summary>
    public static void Register(Type controlType)
    {
        if (RegisteredTypes.TryAdd(controlType, 0))
        {
            EventManager.RegisterClassHandler(
                controlType,
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler(OnLoaded));
        }
    }

    public static bool IsRegistered(Type controlType)
        => RegisteredTypes.ContainsKey(controlType);

    public static string? FindMissingStyle(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        return control.Template is null && control.Style is null
            ? $"{control.GetType().Name} has no style: merge {ControlsDictionary} (and the Atc.Wpf.Theming Default.xaml it builds on) into the application resources."
            : null;
    }

    private static void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is Control control &&
            !DesignerProperties.GetIsInDesignMode(control) &&
            FindMissingStyle(control) is { } message)
        {
            TraceOnce.Warn($"style:{control.GetType().FullName}", message);
        }
    }
}