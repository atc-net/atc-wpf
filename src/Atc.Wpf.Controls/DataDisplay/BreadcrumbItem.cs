namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Represents a single clickable segment in a <see cref="Breadcrumb"/> control.
/// </summary>
[ContentProperty(nameof(Content))]
[DefaultStyleKey]
public sealed partial class BreadcrumbItem : ContentControl
{
    /// <summary>
    /// Whether this item is the last (current) item in the breadcrumb.
    /// When true, the item is displayed as bold and non-clickable.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isLast;

    /// <summary>
    /// The command to execute when the item is clicked.
    /// </summary>
    [DependencyProperty]
    private ICommand? command;

    /// <summary>
    /// The parameter to pass to the <see cref="Command"/>.
    /// </summary>
    [DependencyProperty]
    private object? commandParameter;

    /// <summary>
    /// The background brush when the mouse is over the item.
    /// </summary>
    [DependencyProperty]
    private Brush? hoverBackground;

    /// <summary>
    /// The foreground brush when the mouse is over the item.
    /// </summary>
    [DependencyProperty]
    private Brush? hoverForeground;

    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    public event RoutedEventHandler? Click;

    static BreadcrumbItem()
    {
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(BreadcrumbItem));
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        Activate();
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        // The item is a tab stop, so the keyboard activates it like a click.
        if (e.Handled || e.Key is not (Key.Enter or Key.Space) || IsLast)
        {
            return;
        }

        Activate();
        e.Handled = true;
    }

    private void Activate()
    {
        if (IsLast)
        {
            return;
        }

        Click?.Invoke(this, new RoutedEventArgs());

        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}