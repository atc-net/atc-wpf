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
public static partial class DragDropAttach
{
    /// <summary>
    /// Gets or sets the value of the IsDragSource attached property.
    /// </summary>
    [AttachedProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsDragSourceChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static bool isDragSource;

    /// <summary>
    /// Gets or sets the value of the IsDropTarget attached property.
    /// </summary>
    [AttachedProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsDropTargetChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static bool isDropTarget;

    /// <summary>
    /// Gets or sets the value of the DropHandler attached property.
    /// </summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static IDropHandler? dropHandler;

    /// <summary>
    /// Gets or sets the value of the DragHandler attached property.
    /// </summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static IDragHandler? dragHandler;

    /// <summary>
    /// Gets or sets the value of the AllowedEffects attached property.
    /// </summary>
    [AttachedProperty(DefaultValue = DragDropEffects.Move, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static DragDropEffects allowedEffects;

    /// <summary>
    /// Gets or sets the value of the ShowDragAdorner attached property.
    /// </summary>
    [AttachedProperty(DefaultValue = true, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static bool showDragAdorner;

    /// <summary>
    /// Gets or sets the value of the ShowDropIndicator attached property.
    /// </summary>
    [AttachedProperty(DefaultValue = true, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.ThrowArgumentNullException)]
    private static bool showDropIndicator;

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