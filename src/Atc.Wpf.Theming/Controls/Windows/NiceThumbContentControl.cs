namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// A content control that behaves like a thumb, raising drag events when the user drags it
/// with the mouse or touch.
/// </summary>
public sealed class NiceThumbContentControl : ContentControlEx, INiceThumb
{
    private TouchDevice? currentDevice;
    private Point startDragPoint;
    private Point startDragScreenPoint;
    private Point oldDragScreenPoint;

    static NiceThumbContentControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NiceThumbContentControl),
            new FrameworkPropertyMetadata(typeof(NiceThumbContentControl)));
        FocusableProperty.OverrideMetadata(
            typeof(NiceThumbContentControl),
            new FrameworkPropertyMetadata(default(bool)));
        EventManager.RegisterClassHandler(
            typeof(NiceThumbContentControl),
            Mouse.LostMouseCaptureEvent,
            new MouseEventHandler(OnLostMouseCapture));
    }

    /// <summary>Identifies the <see cref="DragStarted"/> routed event.</summary>
    public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent(
        nameof(DragStarted),
        RoutingStrategy.Bubble,
        typeof(DragStartedEventHandler),
        typeof(NiceThumbContentControl));

    /// <summary>Identifies the <see cref="DragDelta"/> routed event.</summary>
    public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent(
        nameof(DragDelta),
        RoutingStrategy.Bubble,
        typeof(DragDeltaEventHandler),
        typeof(NiceThumbContentControl));

    /// <summary>Identifies the <see cref="DragCompleted"/> routed event.</summary>
    public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent(
        nameof(DragCompleted),
        RoutingStrategy.Bubble,
        typeof(DragCompletedEventHandler),
        typeof(NiceThumbContentControl));

    /// <summary>
    /// Adds or remove a DragStartedEvent handler
    /// </summary>
    public event DragStartedEventHandler DragStarted
    {
        add => AddHandler(DragStartedEvent, value);
        remove => RemoveHandler(DragStartedEvent, value);
    }

    /// <summary>
    /// Adds or remove a DragDeltaEvent handler
    /// </summary>
    public event DragDeltaEventHandler DragDelta
    {
        add => AddHandler(DragDeltaEvent, value);
        remove => RemoveHandler(DragDeltaEvent, value);
    }

    /// <summary>
    /// Adds or remove a DragCompletedEvent handler
    /// </summary>
    public event DragCompletedEventHandler DragCompleted
    {
        add => AddHandler(DragCompletedEvent, value);
        remove => RemoveHandler(DragCompletedEvent, value);
    }

    private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsDragging),
        typeof(bool),
        typeof(NiceThumbContentControl),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Identifies the <see cref="IsDragging"/> dependency property.</summary>
    public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the control is currently being dragged.
    /// </summary>
    public bool IsDragging
    {
        get => (bool)GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingPropertyKey, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Cancels the current drag operation, if any, and raises <see cref="DragCompleted"/> with the canceled flag set.
    /// </summary>
    public void CancelDragAction()
    {
        if (!IsDragging)
        {
            return;
        }

        if (IsMouseCaptured)
        {
            ReleaseMouseCapture();
        }

        ClearValue(IsDraggingPropertyKey);
        var horizontalChange = oldDragScreenPoint.X - startDragScreenPoint.X;
        var verticalChange = oldDragScreenPoint.Y - startDragScreenPoint.Y;
        RaiseEvent(new NiceThumbContentControlDragCompletedEventArgs(horizontalChange, verticalChange, canceled: true));
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!IsDragging)
        {
            e.Handled = true;
            try
            {
                Focus();
                CaptureMouse();
                SetValue(IsDraggingPropertyKey, BooleanBoxes.TrueBox);
                startDragPoint = e.GetPosition(this);
                oldDragScreenPoint = startDragScreenPoint = PointToScreen(startDragPoint);
                RaiseEvent(new NiceThumbContentControlDragStartedEventArgs(startDragPoint.X, startDragPoint.Y));
            }
            catch (Exception exception)
            {
                Trace.TraceError($"{this}: Something went wrong here: {exception} {Environment.NewLine} {exception.StackTrace}");
                CancelDragAction();
            }
        }

        base.OnMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (IsMouseCaptured && IsDragging)
        {
            e.Handled = true;
            ClearValue(IsDraggingPropertyKey);
            ReleaseMouseCapture();

            var currentMouseScreenPoint = PointToScreen(e.MouseDevice.GetPosition(this));
            var horizontalChange = currentMouseScreenPoint.X - startDragScreenPoint.X;
            var verticalChange = currentMouseScreenPoint.Y - startDragScreenPoint.Y;
            RaiseEvent(new NiceThumbContentControlDragCompletedEventArgs(horizontalChange, verticalChange, false));
        }

        base.OnMouseLeftButtonUp(e);
    }

    private static void OnLostMouseCapture(
        object sender,
        MouseEventArgs e)
    {
        var thumb = (NiceThumbContentControl)sender;
        if (!ReferenceEquals(Mouse.Captured, thumb))
        {
            thumb.CancelDragAction();
        }
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (!IsDragging)
        {
            return;
        }

        if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
        {
            var currentDragPoint = e.GetPosition(this);
            var currentDragScreenPoint = PointToScreen(currentDragPoint);
            if (currentDragScreenPoint == oldDragScreenPoint)
            {
                return;
            }

            oldDragScreenPoint = currentDragScreenPoint;
            e.Handled = true;
            var horizontalChange = currentDragPoint.X - startDragPoint.X;
            var verticalChange = currentDragPoint.Y - startDragPoint.Y;
            RaiseEvent(new DragDeltaEventArgs(horizontalChange, verticalChange) { RoutedEvent = DragDeltaEvent });
        }
        else
        {
            if (ReferenceEquals(e.MouseDevice.Captured, this))
            {
                ReleaseMouseCapture();
            }

            ClearValue(IsDraggingPropertyKey);
            startDragPoint.X = 0;
            startDragPoint.Y = 0;
        }
    }

    /// <inheritdoc />
    protected override void OnPreviewTouchDown(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        ReleaseCurrentDevice();
        CaptureCurrentDevice(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewTouchUp(TouchEventArgs e)
        => ReleaseCurrentDevice();

    /// <inheritdoc />
    protected override void OnLostTouchCapture(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (currentDevice is null)
        {
            return;
        }

        CaptureCurrentDevice(e);
    }

    private void ReleaseCurrentDevice()
    {
        if (currentDevice is null)
        {
            return;
        }

        var temp = currentDevice;
        currentDevice = null;
        ReleaseTouchCapture(temp);
    }

    private void CaptureCurrentDevice(TouchEventArgs e)
    {
        var gotTouch = CaptureTouch(e.TouchDevice);
        if (gotTouch)
        {
            currentDevice = e.TouchDevice;
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new NiceThumbContentControlAutomationPeer(this);
}