namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Attached properties that enable drag-and-drop on any UIElement.
/// </summary>
/// <example>
/// <code>
/// &lt;ListBox atc:DragDropAttach.IsDragSource="True"
///          atc:DragDropAttach.IsDropTarget="True"
///          atc:DragDropAttach.DropHandler="{Binding}" /&gt;
/// </code>
/// </example>
public static class DragDropAttach
{
    /// <summary>
    /// Identifies the IsDragSource attached property.
    /// </summary>
    public static readonly DependencyProperty IsDragSourceProperty =
        DependencyProperty.RegisterAttached(
            "IsDragSource",
            typeof(bool),
            typeof(DragDropAttach),
            new PropertyMetadata(false, OnIsDragSourceChanged));

    /// <summary>
    /// Identifies the IsDropTarget attached property.
    /// </summary>
    public static readonly DependencyProperty IsDropTargetProperty =
        DependencyProperty.RegisterAttached(
            "IsDropTarget",
            typeof(bool),
            typeof(DragDropAttach),
            new PropertyMetadata(false, OnIsDropTargetChanged));

    /// <summary>
    /// Identifies the DropHandler attached property.
    /// </summary>
    public static readonly DependencyProperty DropHandlerProperty =
        DependencyProperty.RegisterAttached(
            "DropHandler",
            typeof(IDropHandler),
            typeof(DragDropAttach),
            new PropertyMetadata(default(IDropHandler)));

    /// <summary>
    /// Identifies the DragHandler attached property.
    /// </summary>
    public static readonly DependencyProperty DragHandlerProperty =
        DependencyProperty.RegisterAttached(
            "DragHandler",
            typeof(IDragHandler),
            typeof(DragDropAttach),
            new PropertyMetadata(default(IDragHandler)));

    /// <summary>
    /// Identifies the AllowedEffects attached property.
    /// </summary>
    public static readonly DependencyProperty AllowedEffectsProperty =
        DependencyProperty.RegisterAttached(
            "AllowedEffects",
            typeof(DragDropEffects),
            typeof(DragDropAttach),
            new PropertyMetadata(DragDropEffects.Move));

    /// <summary>
    /// Identifies the ShowDragAdorner attached property.
    /// </summary>
    public static readonly DependencyProperty ShowDragAdornerProperty =
        DependencyProperty.RegisterAttached(
            "ShowDragAdorner",
            typeof(bool),
            typeof(DragDropAttach),
            new PropertyMetadata(true));

    /// <summary>
    /// Identifies the ShowDropIndicator attached property.
    /// </summary>
    public static readonly DependencyProperty ShowDropIndicatorProperty =
        DependencyProperty.RegisterAttached(
            "ShowDropIndicator",
            typeof(bool),
            typeof(DragDropAttach),
            new PropertyMetadata(true));

    public static bool GetIsDragSource(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsDragSourceProperty);
    }

    public static void SetIsDragSource(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(IsDragSourceProperty, value);
    }

    public static bool GetIsDropTarget(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsDropTargetProperty);
    }

    public static void SetIsDropTarget(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(IsDropTargetProperty, value);
    }

    public static IDropHandler? GetDropHandler(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (IDropHandler?)element.GetValue(DropHandlerProperty);
    }

    public static void SetDropHandler(
        UIElement element,
        IDropHandler? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(DropHandlerProperty, value);
    }

    public static IDragHandler? GetDragHandler(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (IDragHandler?)element.GetValue(DragHandlerProperty);
    }

    public static void SetDragHandler(
        UIElement element,
        IDragHandler? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(DragHandlerProperty, value);
    }

    public static DragDropEffects GetAllowedEffects(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (DragDropEffects)element.GetValue(AllowedEffectsProperty);
    }

    public static void SetAllowedEffects(
        UIElement element,
        DragDropEffects value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(AllowedEffectsProperty, value);
    }

    public static bool GetShowDragAdorner(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(ShowDragAdornerProperty);
    }

    public static void SetShowDragAdorner(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(ShowDragAdornerProperty, value);
    }

    public static bool GetShowDropIndicator(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(ShowDropIndicatorProperty);
    }

    public static void SetShowDropIndicator(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(ShowDropIndicatorProperty, value);
    }

    private static void OnIsDragSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            DragDropManager.RegisterDragSource(element);
        }
        else
        {
            DragDropManager.UnregisterDragSource(element);
        }
    }

    private static void OnIsDropTargetChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            DragDropManager.RegisterDropTarget(element);
        }
        else
        {
            DragDropManager.UnregisterDropTarget(element);
        }
    }
}