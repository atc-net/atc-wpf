namespace Atc.Wpf.UndoRedo.Behaviors;

/// <summary>
/// An attached behavior that maps standard keyboard shortcuts to
/// <see cref="IUndoRedoService"/> operations on any <see cref="UIElement"/>.
/// </summary>
/// <remarks>
/// <list type="table">
///   <listheader><term>Shortcut</term><description>Action</description></listheader>
///   <item><term>Ctrl+Z</term><description>Undo</description></item>
///   <item><term>Ctrl+Shift+Z</term><description>Undo All</description></item>
///   <item><term>Ctrl+Y</term><description>Redo</description></item>
///   <item><term>Ctrl+Shift+Y</term><description>Redo All</description></item>
/// </list>
/// </remarks>
public sealed class UndoRedoKeyBindingBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty UndoRedoServiceProperty =
        DependencyProperty.Register(
            nameof(UndoRedoService),
            typeof(IUndoRedoService),
            typeof(UndoRedoKeyBindingBehavior),
            new PropertyMetadata(defaultValue: null));

    public IUndoRedoService? UndoRedoService
    {
        get => (IUndoRedoService?)GetValue(UndoRedoServiceProperty);
        set => SetValue(UndoRedoServiceProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        base.OnDetaching();
    }

    private void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (UndoRedoService is not { } service)
        {
            return;
        }

        var ctrl = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        var shift = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        if (!ctrl)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Z when shift:
                service.UndoAll();
                e.Handled = true;
                break;
            case Key.Z:
                service.Undo();
                e.Handled = true;
                break;
            case Key.Y when shift:
                service.RedoAll();
                e.Handled = true;
                break;
            case Key.Y:
                service.Redo();
                e.Handled = true;
                break;
        }
    }
}