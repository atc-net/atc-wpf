namespace Atc.Wpf.DragDrop;

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

    /// <summary>
    /// Gets the value of the IsDragSource attached property.
    /// </summary>
    public static bool GetIsDragSource(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsDragSourceProperty);
    }

    /// <summary>
    /// Sets the value of the IsDragSource attached property.
    /// </summary>
    public static void SetIsDragSource(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(IsDragSourceProperty, value);
    }

    /// <summary>
    /// Gets the value of the IsDropTarget attached property.
    /// </summary>
    public static bool GetIsDropTarget(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsDropTargetProperty);
    }

    /// <summary>
    /// Sets the value of the IsDropTarget attached property.
    /// </summary>
    public static void SetIsDropTarget(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(IsDropTargetProperty, value);
    }

    /// <summary>
    /// Gets the value of the DropHandler attached property.
    /// </summary>
    public static IDropHandler? GetDropHandler(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (IDropHandler?)element.GetValue(DropHandlerProperty);
    }

    /// <summary>
    /// Sets the value of the DropHandler attached property.
    /// </summary>
    public static void SetDropHandler(
        UIElement element,
        IDropHandler? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(DropHandlerProperty, value);
    }

    /// <summary>
    /// Gets the value of the DragHandler attached property.
    /// </summary>
    public static IDragHandler? GetDragHandler(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (IDragHandler?)element.GetValue(DragHandlerProperty);
    }

    /// <summary>
    /// Sets the value of the DragHandler attached property.
    /// </summary>
    public static void SetDragHandler(
        UIElement element,
        IDragHandler? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(DragHandlerProperty, value);
    }

    /// <summary>
    /// Gets the value of the AllowedEffects attached property.
    /// </summary>
    public static DragDropEffects GetAllowedEffects(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (DragDropEffects)element.GetValue(AllowedEffectsProperty);
    }

    /// <summary>
    /// Sets the value of the AllowedEffects attached property.
    /// </summary>
    public static void SetAllowedEffects(
        UIElement element,
        DragDropEffects value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(AllowedEffectsProperty, value);
    }

    /// <summary>
    /// Gets the value of the ShowDragAdorner attached property.
    /// </summary>
    public static bool GetShowDragAdorner(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(ShowDragAdornerProperty);
    }

    /// <summary>
    /// Sets the value of the ShowDragAdorner attached property.
    /// </summary>
    public static void SetShowDragAdorner(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(ShowDragAdornerProperty, value);
    }

    /// <summary>
    /// Gets the value of the ShowDropIndicator attached property.
    /// </summary>
    public static bool GetShowDropIndicator(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(ShowDropIndicatorProperty);
    }

    /// <summary>
    /// Sets the value of the ShowDropIndicator attached property.
    /// </summary>
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