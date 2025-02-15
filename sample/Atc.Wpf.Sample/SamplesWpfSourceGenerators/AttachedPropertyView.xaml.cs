namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

/// <summary>
/// Interaction logic for AttachedPropertyView.
/// </summary>
public partial class AttachedPropertyView
{
    public AttachedPropertyView()
    {
        InitializeComponent();

        DataContext = this;
    }

    private void OnToggleDraggingClick(object sender, RoutedEventArgs e)
    {
        var isCurrentlyDraggable = DragBehavior.GetIsDraggable(MyTextBlock);
        DragBehavior.SetIsDraggable(MyTextBlock, !isCurrentlyDraggable);
    }

    public string CSharpCodeForDragBehavior
        => """
           //
           //
           // This is the DragBehavior that is used in the xaml file for AttachedPropertyView.xaml
           // And some of the magic is made by the class attribute <see cref="AttachedProperty" />
           //
           //
           [AttachedProperty<bool>("IsDraggable", PropertyChangedCallback = nameof(OnIsDraggableChanged))]
           public partial class DragBehavior
           {
               private static bool isDragging;
               private static Point startPoint;
           
               private static void OnIsDraggableChanged(
                   DependencyObject d,
                   DependencyPropertyChangedEventArgs e)
               {
                   if (d is not UIElement element)
                   {
                       return;
                   }
           
                   if ((bool)e.NewValue)
                   {
                       element.MouseDown += OnMouseDown;
                       element.MouseMove += OnMouseMove;
                       element.MouseUp += OnMouseUp;
                   }
                   else
                   {
                       element.MouseDown -= OnMouseDown;
                       element.MouseMove -= OnMouseMove;
                       element.MouseUp -= OnMouseUp;
                   }
               }
           
               private static void OnMouseDown(
                   object sender,
                   MouseButtonEventArgs e)
               {
                   if (sender is not UIElement element)
                   {
                       return;
                   }
           
                   isDragging = true;
                   startPoint = e.GetPosition(element);
                   element.CaptureMouse();
               }
           
               private static void OnMouseMove(
                   object sender,
                   MouseEventArgs e)
               {
                   if (!isDragging || sender is not UIElement element)
                   {
                       return;
                   }
           
                   var currentPosition = e.GetPosition(element);
                   var offset = currentPosition - startPoint;
                   var translateTransform = element.RenderTransform as TranslateTransform ?? new TranslateTransform();
                   translateTransform.X += offset.X;
                   translateTransform.Y += offset.Y;
                   element.RenderTransform = translateTransform;
               }
           
               private static void OnMouseUp(
                   object sender,
                   MouseButtonEventArgs e)
               {
                   isDragging = false;
                   if (sender is UIElement element)
                   {
                       element.ReleaseMouseCapture();
                   }
               }
           }
           """;
}