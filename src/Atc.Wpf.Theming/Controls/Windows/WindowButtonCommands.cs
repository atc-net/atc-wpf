namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// The minimize, maximize/restore and close buttons shown in the title bar of a window.
/// </summary>
[TemplatePart(
    Name = "PART_Min",
    Type = typeof(Button))]
[TemplatePart(
    Name = "PART_Max",
    Type = typeof(Button))]
[TemplatePart(
    Name = "PART_Close",
    Type = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(LightMinButtonStyle),
    StyleTargetType = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(LightMaxButtonStyle),
    StyleTargetType = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(LightCloseButtonStyle),
    StyleTargetType = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(DarkMinButtonStyle),
    StyleTargetType = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(DarkMaxButtonStyle),
    StyleTargetType = typeof(Button))]
[StyleTypedProperty(
    Property = nameof(DarkCloseButtonStyle),
    StyleTargetType = typeof(Button))]
[DefaultStyleKey]
public sealed partial class WindowButtonCommands : ContentControl
{
    /// <summary>
    /// Occurs when the close button is used, before the window is closed; the handler can cancel closing.
    /// </summary>
    public event ClosingWindowEventHandler? ClosingWindow;

    /// <summary>
    /// Represents the method that handles the <see cref="ClosingWindow"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data, which allows cancelling the close.</param>
    [SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "OK.")]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
    public delegate void ClosingWindowEventHandler(
        object sender,
        ClosingWindowEventArgs e);

    /// <summary>
    /// Gets or sets the value indicating current light style for the minimize button.
    /// </summary>
    [DependencyProperty]
    private Style? lightMinButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current light style for the maximize button.
    /// </summary>
    [DependencyProperty]
    private Style? lightMaxButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current light style for the close button.
    /// </summary>
    [DependencyProperty]
    private Style? lightCloseButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current dark style for the minimize button.
    /// </summary>
    [DependencyProperty]
    private Style? darkMinButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current dark style for the maximize button.
    /// </summary>
    [DependencyProperty]
    private Style? darkMaxButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current dark style for the close button.
    /// </summary>
    [DependencyProperty]
    private Style? darkCloseButtonStyle;

    /// <summary>
    /// Gets or sets the value indicating current theme.
    /// </summary>
    [DependencyProperty(DefaultValueExpression = "ThemeManager.BaseColorLight")]
    private string theme;

    /// <summary>
    /// Gets or sets the minimize button tooltip.
    /// </summary>
    [DependencyProperty]
    private string? minimize;

    /// <summary>
    /// Gets or sets the maximize button tooltip.
    /// </summary>
    [DependencyProperty]
    private string? maximize;

    /// <summary>
    /// Gets or sets the close button tooltip.
    /// </summary>
    [DependencyProperty]
    private string? close;

    /// <summary>
    /// Gets or sets the restore button tooltip.
    /// </summary>
    [DependencyProperty]
    private string? restore;

    internal static readonly DependencyPropertyKey ParentWindowPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ParentWindow),
        typeof(Window),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
    public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the window.
    /// </summary>
    public Window? ParentWindow
    {
        get => (Window?)GetValue(ParentWindowProperty);
        set => SetValue(ParentWindowPropertyKey, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowButtonCommands"/> class.
    /// </summary>
    public WindowButtonCommands()
    {
        CommandBindings.Add(new CommandBinding(
            System.Windows.SystemCommands.MinimizeWindowCommand,
            MinimizeWindow));

        CommandBindings.Add(new CommandBinding(
            System.Windows.SystemCommands.MaximizeWindowCommand,
            MaximizeWindow));

        CommandBindings.Add(new CommandBinding(
            System.Windows.SystemCommands.RestoreWindowCommand,
            RestoreWindow));

        CommandBindings.Add(new CommandBinding(
            System.Windows.SystemCommands.CloseWindowCommand,
            CloseWindow));

        // The parent window exists only once the control is in a tree; NiceWindow sets it earlier itself.
        Loaded += WindowButtonCommandsLoaded;

        this.Invoke(
            () =>
            {
                if (string.IsNullOrWhiteSpace(Minimize))
                {
                    SetCurrentValue(MinimizeProperty, WinApiHelper.GetCaption(900));
                }

                if (string.IsNullOrWhiteSpace(Maximize))
                {
                    SetCurrentValue(MaximizeProperty, WinApiHelper.GetCaption(901));
                }

                if (string.IsNullOrWhiteSpace(Close))
                {
                    SetCurrentValue(CloseProperty, WinApiHelper.GetCaption(905));
                }

                if (string.IsNullOrWhiteSpace(Restore))
                {
                    SetCurrentValue(RestoreProperty, WinApiHelper.GetCaption(903));
                }
            },
            DispatcherPriority.Loaded);
    }

    private void WindowButtonCommandsLoaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -= WindowButtonCommandsLoaded;

        if (ParentWindow is null)
        {
            var window = this.TryFindParent<Window>();
            SetValue(ParentWindowPropertyKey, window);
        }
    }

    private void MinimizeWindow(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.MinimizeWindow(ParentWindow);
    }

    private void MaximizeWindow(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.MaximizeWindow(ParentWindow);
    }

    private void RestoreWindow(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.RestoreWindow(ParentWindow);
    }

    private void CloseWindow(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        var args = new ClosingWindowEventArgs();
        ClosingWindow?.Invoke(this, args);

        if (args.Cancelled)
        {
            return;
        }

        System.Windows.SystemCommands.CloseWindow(ParentWindow);
    }
}