namespace Atc.Wpf.Behaviors;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public sealed class MouseEventToCommandBehavior : Behavior<UIElement>
{
    private const int DoubleClickTimeMs = 500;

    private static readonly DependencyProperty LastClickTimeProperty =
        DependencyProperty.RegisterAttached(
            "LastClickTime",
            typeof(DateTime),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(DateTime.MinValue));

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(propertyChangedCallback: null));

    public static void SetCommandParameter(
        DependencyObject element,
        object value)
        => element.SetValue(CommandParameterProperty, value);

    public static object GetCommandParameter(DependencyObject element)
        => element.GetValue(CommandParameterProperty);

    public static readonly DependencyProperty LeftClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "LeftClickCommand",
            typeof(ICommand),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(
                defaultValue: null,
                OnLeftClickCommandChanged));

    public static void SetLeftClickCommand(
        DependencyObject element,
        ICommand value)
        => element.SetValue(LeftClickCommandProperty, value);

    public static ICommand GetLeftClickCommand(DependencyObject element)
        => (ICommand)element.GetValue(LeftClickCommandProperty);

    public static readonly DependencyProperty LeftDoubleClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "LeftDoubleClickCommand",
            typeof(ICommand),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(
                defaultValue: null,
                OnLeftDoubleClickCommandChanged));

    public static void SetLeftDoubleClickCommand(
        DependencyObject element,
        ICommand value)
        => element.SetValue(LeftDoubleClickCommandProperty, value);

    public static ICommand GetLeftDoubleClickCommand(DependencyObject element)
        => (ICommand)element.GetValue(LeftDoubleClickCommandProperty);

    public static readonly DependencyProperty RightClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "RightClickCommand",
            typeof(ICommand),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(
                defaultValue: null,
                OnRightClickCommandChanged));

    public static void SetRightClickCommand(
        DependencyObject element,
        ICommand value)
        => element.SetValue(RightClickCommandProperty, value);

    public static ICommand GetRightClickCommand(DependencyObject element)
        => (ICommand)element.GetValue(RightClickCommandProperty);

    public static readonly DependencyProperty RightDoubleClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "RightDoubleClickCommand",
            typeof(ICommand),
            typeof(MouseEventToCommandBehavior),
            new PropertyMetadata(
                defaultValue: null,
                OnRightDoubleClickCommandChanged));

    public static void SetRightDoubleClickCommand(
        DependencyObject element,
        ICommand value)
        => element.SetValue(RightDoubleClickCommandProperty, value);

    public static ICommand GetRightDoubleClickCommand(DependencyObject element)
        => (ICommand)element.GetValue(RightDoubleClickCommandProperty);

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

        var command = GetLeftDoubleClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        var now = DateTime.Now;
        var lastClickTime = (DateTime)element.GetValue(LastClickTimeProperty);
        var span = now - lastClickTime;
        if (span.TotalMilliseconds < DoubleClickTimeMs)
        {
            if (command is not null &&
                command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }

            element.SetValue(LastClickTimeProperty, now);
        }
        else
        {
            element.SetValue(LastClickTimeProperty, now);
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

        var command = GetRightDoubleClickCommand(element);
        var commandParameter = GetCommandParameter(element);

        var now = DateTime.Now;
        var lastClickTime = (DateTime)element.GetValue(LastClickTimeProperty);
        var span = now - lastClickTime;
        if (span.TotalMilliseconds < DoubleClickTimeMs)
        {
            if (command is not null &&
                command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }

            element.SetValue(LastClickTimeProperty, now);
        }
        else
        {
            element.SetValue(LastClickTimeProperty, now);
        }
    }
}