namespace Atc.Wpf.Theming.Controls.Windows;

[TemplatePart(Name = "PART_Min", Type = typeof(Button))]
[TemplatePart(Name = "PART_Max", Type = typeof(Button))]
[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
[StyleTypedProperty(Property = nameof(LightMinButtonStyle), StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = nameof(LightMaxButtonStyle), StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = nameof(LightCloseButtonStyle), StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = nameof(DarkMinButtonStyle), StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = nameof(DarkMaxButtonStyle), StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = nameof(DarkCloseButtonStyle), StyleTargetType = typeof(Button))]
public sealed class WindowButtonCommands : ContentControl
{
    public event ClosingWindowEventHandler? ClosingWindow;

    [SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "OK.")]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
    public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventArgs e);

    public static readonly DependencyProperty LightMinButtonStyleProperty = DependencyProperty.Register(
        nameof(LightMinButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current light style for the minimize button.
    /// </summary>
    public Style? LightMinButtonStyle
    {
        get => (Style?)GetValue(LightMinButtonStyleProperty);
        set => SetValue(LightMinButtonStyleProperty, value);
    }

    public static readonly DependencyProperty LightMaxButtonStyleProperty = DependencyProperty.Register(
        nameof(LightMaxButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current light style for the maximize button.
    /// </summary>
    public Style? LightMaxButtonStyle
    {
        get => (Style?)GetValue(LightMaxButtonStyleProperty);
        set => SetValue(LightMaxButtonStyleProperty, value);
    }

    public static readonly DependencyProperty LightCloseButtonStyleProperty = DependencyProperty.Register(
        nameof(LightCloseButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current light style for the close button.
    /// </summary>
    public Style? LightCloseButtonStyle
    {
        get => (Style?)GetValue(LightCloseButtonStyleProperty);
        set => SetValue(LightCloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty DarkMinButtonStyleProperty = DependencyProperty.Register(
        nameof(DarkMinButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current dark style for the minimize button.
    /// </summary>
    public Style? DarkMinButtonStyle
    {
        get => (Style?)GetValue(DarkMinButtonStyleProperty);
        set => SetValue(DarkMinButtonStyleProperty, value);
    }

    public static readonly DependencyProperty DarkMaxButtonStyleProperty = DependencyProperty.Register(
        nameof(DarkMaxButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current dark style for the maximize button.
    /// </summary>
    public Style? DarkMaxButtonStyle
    {
        get => (Style?)GetValue(DarkMaxButtonStyleProperty);
        set => SetValue(DarkMaxButtonStyleProperty, value);
    }

    public static readonly DependencyProperty DarkCloseButtonStyleProperty = DependencyProperty.Register(
        nameof(DarkCloseButtonStyle),
        typeof(Style),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating current dark style for the close button.
    /// </summary>
    public Style? DarkCloseButtonStyle
    {
        get => (Style?)GetValue(DarkCloseButtonStyleProperty);
        set => SetValue(DarkCloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
        nameof(Theme),
        typeof(string),
        typeof(WindowButtonCommands),
        new PropertyMetadata(ThemeManager.BaseColorLight));

    /// <summary>
    /// Gets or sets the value indicating current theme.
    /// </summary>
    public string Theme
    {
        get => (string)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly DependencyProperty MinimizeProperty = DependencyProperty.Register(
        nameof(Minimize),
        typeof(string),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the minimize button tooltip.
    /// </summary>
    public string? Minimize
    {
        get => (string?)GetValue(MinimizeProperty);
        set => SetValue(MinimizeProperty, value);
    }

    public static readonly DependencyProperty MaximizeProperty = DependencyProperty.Register(
        nameof(Maximize),
        typeof(string),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the maximize button tooltip.
    /// </summary>
    public string? Maximize
    {
        get => (string?)GetValue(MaximizeProperty);
        set => SetValue(MaximizeProperty, value);
    }

    public static readonly DependencyProperty CloseProperty = DependencyProperty.Register(
        nameof(Close),
        typeof(string),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the close button tooltip.
    /// </summary>
    public string? Close
    {
        get => (string?)GetValue(CloseProperty);
        set => SetValue(CloseProperty, value);
    }

    public static readonly DependencyProperty RestoreProperty = DependencyProperty.Register(
        nameof(Restore),
        typeof(string),
        typeof(WindowButtonCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the restore button tooltip.
    /// </summary>
    public string? Restore
    {
        get => (string?)GetValue(RestoreProperty);
        set => SetValue(RestoreProperty, value);
    }

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

    static WindowButtonCommands()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
    }

    public WindowButtonCommands()
    {
        CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.MinimizeWindowCommand, MinimizeWindow));
        CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.MaximizeWindowCommand, MaximizeWindow));
        CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.RestoreWindowCommand, RestoreWindow));
        CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.CloseWindowCommand, CloseWindow));

        this.Invoke(
            () =>
            {
                if (ParentWindow is null)
                {
                    var window = this.TryFindParent<Window>();
                    SetValue(ParentWindowPropertyKey, window);
                }

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

    private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.MinimizeWindow(ParentWindow);
    }

    private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.MaximizeWindow(ParentWindow);
    }

    private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (ParentWindow is null)
        {
            return;
        }

        System.Windows.SystemCommands.RestoreWindow(ParentWindow);
    }

    private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
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