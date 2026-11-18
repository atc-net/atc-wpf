namespace Atc.Wpf.Behaviors;

/// <summary>
/// Provides attached properties that execute commands on left/right mouse clicks and double-clicks of a <see cref="UIElement"/>.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public sealed partial class MouseEventToCommandBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Identifies the CommandParameter attached property.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Sets the value of the CommandParameter attached property.
    /// </summary>
    public static void SetCommandParameter(
        DependencyObject element,
        object value)
        => element.SetValue(CommandParameterProperty, value);

    /// <summary>
    /// Gets the value of the CommandParameter attached property.
    /// </summary>
    public static object GetCommandParameter(DependencyObject element)
        => element.GetValue(CommandParameterProperty);

    /// <summary>
    /// Gets or sets the value of the LeftClickCommand attached property.
    /// </summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnLeftClickCommandChanged), NullElementHandling = NullElementHandling.None)]
    private static ICommand leftClickCommand;

    /// <summary>
    /// Gets or sets the value of the LeftDoubleClickCommand attached property.
    /// </summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnLeftDoubleClickCommandChanged), NullElementHandling = NullElementHandling.None)]
    private static ICommand leftDoubleClickCommand;

    /// <summary>
    /// Gets or sets the value of the RightClickCommand attached property.
    /// </summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnRightClickCommandChanged), NullElementHandling = NullElementHandling.None)]
    private static ICommand rightClickCommand;

    /// <summary>
    /// Gets or sets the value of the RightDoubleClickCommand attached property.
    /// </summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnRightDoubleClickCommandChanged), NullElementHandling = NullElementHandling.None)]
    private static ICommand rightDoubleClickCommand;

    private static void OnLeftClickCommandChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown -= OnLeftClick;
        if (e.NewValue is ICommand)
        {
            element.PreviewMouseLeftButtonDown += OnLeftClick;
        }
    }

    private static void OnLeftDoubleClickCommandChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown -= OnLeftDoubleClick;
        if (e.NewValue is ICommand)
        {
            element.PreviewMouseLeftButtonDown += OnLeftDoubleClick;
        }
    }

    private static void OnRightClickCommandChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        element.PreviewMouseRightButtonDown -= OnRightClick;
        if (e.NewValue is ICommand)
        {
            element.PreviewMouseRightButtonDown += OnRightClick;
        }
    }

    private static void OnRightDoubleClickCommandChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        element.PreviewMouseRightButtonDown -= OnRightDoubleClick;
        if (e.NewValue is ICommand)
        {
            element.PreviewMouseRightButtonDown += OnRightDoubleClick;
        }
    }

    private static void OnLeftClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject element)
        {
            return;
        }

        var command = GetLeftClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        if (command is not null &&
            command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    private static void OnLeftDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject element)
        {
            return;
        }

        if (!IsDoubleClick(e))
        {
            return;
        }

        var command = GetLeftDoubleClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        if (command is not null &&
            command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    private static void OnRightClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject element)
        {
            return;
        }

        var command = GetRightClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        if (command is not null &&
            command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    private static void OnRightDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject element)
        {
            return;
        }

        if (!IsDoubleClick(e))
        {
            return;
        }

        var command = GetRightDoubleClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        if (command is not null &&
            command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    /// <summary>
    /// A double-click is the second press of the same button within the system double-click
    /// time and distance, which the input system reports through <see cref="MouseButtonEventArgs.ClickCount"/>.
    /// Per-button, so a left press followed by a right press is never a double-click.
    /// </summary>
    private static bool IsDoubleClick(MouseButtonEventArgs e)
        => e.ClickCount == 2;
}