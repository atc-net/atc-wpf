// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    private Border? partDragZoomBorder;
    private Canvas? partDragZoomCanvas;
    private MouseHandlingModeType mouseHandlingMode;
    private Point zoomControlMouseDownPoint;
    private Point contentMouseDownPoint;
    private MouseButton mouseButtonDown;
    private bool isSpacebarHeld;
    private Cursor? cursorBeforeSpacebar;

    private RelayCommand? zoomFillCommand;
    private RelayCommand? zoomFitCommand;
    private RelayCommand<double>? zoomPercentCommand;
    private RelayCommand<double>? zoomRatioFromMinimumCommand;
    private RelayCommand? zoomOutCommand;
    private RelayCommand? zoomInCommand;
    private RelayCommand<Rect>? zoomToSelectionCommand;
    private RelayCommand? zoomToNextPresetCommand;
    private RelayCommand? zoomToPreviousPresetCommand;

    /// <summary>
    /// Command to implement the zoom to fill.
    /// </summary>
    public ICommand ZoomFillCommand => zoomFillCommand ??= new RelayCommand(
        () =>
        {
            SaveZoom();
            AnimatedZoomToCentered(FillZoomValue);
            RaiseCanExecuteChanged();
        },
        () => !InternalViewportZoom.IsWithinOnePercent(FillZoomValue) && FillZoomValue >= MinimumZoomClamped);

    /// <summary>
    /// Command to implement the zoom to fit.
    /// </summary>
    public ICommand ZoomFitCommand => zoomFitCommand ??= new RelayCommand(
        () =>
        {
            SaveZoom();
            AnimatedZoomTo(FitZoomValue);
            RaiseCanExecuteChanged();
        },
        () => !InternalViewportZoom.IsWithinOnePercent(FitZoomValue) && FitZoomValue >= MinimumZoomClamped);

    /// <summary>
    /// Command to implement the zoom to a percentage.
    /// </summary>
    public ICommand ZoomPercentCommand
        => zoomPercentCommand ??= new RelayCommand<double>(
        value =>
        {
            SaveZoom();
            var adjustedValue = value.IsZero()
                ? 1
                : value / 100;

            AnimatedZoomTo(adjustedValue);
            RaiseCanExecuteChanged();
        },
        value =>
        {
            var adjustedValue = value.IsZero()
                ? 1
                : value / 100;

            return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
        });

    /// <summary>
    /// Command to implement the zoom ratio where 1 is the specified minimum.
    /// </summary>
    public ICommand ZoomRatioFromMinimumCommand
        => zoomRatioFromMinimumCommand ??= new RelayCommand<double>(
        value =>
        {
            SaveZoom();
            var adjustedValue = (value.IsZero() ? 2 : value) * MinimumZoomClamped;
            AnimatedZoomTo(adjustedValue);
            RaiseCanExecuteChanged();
        },
        value =>
        {
            var adjustedValue = (value.IsZero() ? 2 : value) * MinimumZoomClamped;
            return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
        });

    /// <summary>
    /// Command to implement the zoom out by 110%.
    /// </summary>
    public ICommand ZoomOutCommand
        => zoomOutCommand ??= new RelayCommand(
        () =>
        {
            DelayedSaveZoom1500MilliSeconds();
            ZoomOut(new Point(ContentZoomFocusX, ContentZoomFocusY));
        },
        () => InternalViewportZoom > MinimumZoomClamped);

    /// <summary>
    /// Command to implement the zoom in by 90%.
    /// </summary>
    public ICommand ZoomInCommand
        => zoomInCommand ??= new RelayCommand(
        () =>
        {
            DelayedSaveZoom1500MilliSeconds();
            ZoomIn(new Point(ContentZoomFocusX, ContentZoomFocusY));
        },
        () => InternalViewportZoom < MaximumZoom);

    /// <summary>
    /// Command to zoom to a specified selection rectangle (in content coordinates).
    /// Animates the viewport to frame the rectangle with a small padding margin.
    /// </summary>
    public ICommand ZoomToSelectionCommand
        => zoomToSelectionCommand ??= new RelayCommand<Rect>(
        rect =>
        {
            SaveZoom();
            AnimatedZoomTo(rect);
            RaiseCanExecuteChanged();
        },
        rect => rect is { Width: > 0, Height: > 0 });

    /// <summary>
    /// Command to zoom to the next higher preset level.
    /// </summary>
    public ICommand ZoomToNextPresetCommand
        => zoomToNextPresetCommand ??= new RelayCommand(
        () =>
        {
            var presets = EffectiveZoomPresets;
            var next = presets.FirstOrDefault(p => p > InternalViewportZoom + 0.001);
            if (next > 0)
            {
                SaveZoom();
                AnimatedZoomTo(next);
                RaiseCanExecuteChanged();
            }
        },
        () => EffectiveZoomPresets.Any(p => p > InternalViewportZoom + 0.001));

    /// <summary>
    /// Command to zoom to the next lower preset level.
    /// </summary>
    public ICommand ZoomToPreviousPresetCommand
        => zoomToPreviousPresetCommand ??= new RelayCommand(
        () =>
        {
            var presets = EffectiveZoomPresets;
            var prev = presets.LastOrDefault(p => p < InternalViewportZoom - 0.001);
            if (prev > 0)
            {
                SaveZoom();
                AnimatedZoomTo(prev);
                RaiseCanExecuteChanged();
            }
        },
        () => EffectiveZoomPresets.Any(p => p < InternalViewportZoom - 0.001));

    /// <summary>
    /// Handles keyboard shortcuts internally. Uses PreviewKeyDown to catch Alt-modified keys
    /// before they are swallowed as system key events.
    /// </summary>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);
        TryHandleKeyDown(e);
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyUp(e);
        TryHandleKeyUp(e);
    }

    /// <summary>
    /// Attempts to handle key down events and execute corresponding zoom commands.
    /// Also handles spacebar for temporary pan mode.
    /// </summary>
    public bool TryHandleKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (IsSpacebarPanEnabled && e.Key == Key.Space && !isSpacebarHeld)
        {
            isSpacebarHeld = true;
            cursorBeforeSpacebar = Cursor;
            Cursor = Cursors.Hand;
            return e.Handled = true;
        }

        // When Alt is held, WPF reports Key.System; the actual key is in SystemKey.
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        var modifiers = Keyboard.Modifiers;

        foreach (var binding in EffectiveZoomKeyBindings)
        {
            if (key != binding.Key || modifiers != binding.Modifiers)
            {
                continue;
            }

            var command = GetCommandForAction(binding.Action);
            if (command is null)
            {
                continue;
            }

            var parameter = binding.Action == ZoomActionType.Zoom100Percent
                ? (object)100.0
                : null;

            if (!command.CanExecute(parameter))
            {
                return false;
            }

            command.Execute(parameter);
            return e.Handled = true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to handle key up events. Releases spacebar pan mode.
    /// </summary>
    public bool TryHandleKeyUp(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (isSpacebarHeld && e.Key == Key.Space)
        {
            isSpacebarHeld = false;
            Cursor = cursorBeforeSpacebar;
            cursorBeforeSpacebar = null;

            if (mouseHandlingMode == MouseHandlingModeType.Panning)
            {
                ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingModeType.None;
            }

            return e.Handled = true;
        }

        return false;
    }

    private ICommand? GetCommandForAction(ZoomActionType action)
        => action switch
        {
            ZoomActionType.ZoomIn => ZoomInCommand,
            ZoomActionType.ZoomOut => ZoomOutCommand,
            ZoomActionType.ZoomFill => ZoomFillCommand,
            ZoomActionType.ZoomFit => ZoomFitCommand,
            ZoomActionType.Zoom100Percent => ZoomPercentCommand,
            ZoomActionType.ZoomToNextPreset => ZoomToNextPresetCommand,
            ZoomActionType.ZoomToPreviousPreset => ZoomToPreviousPresetCommand,
            ZoomActionType.UndoZoom => UndoZoomCommand,
            ZoomActionType.RedoZoom => RedoZoomCommand,
            _ => null,
        };

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseDown(e);

        if (content is null)
        {
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None ||
            (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        {
            SaveZoom();
        }

        Focus();
        Keyboard.Focus(this);

        mouseButtonDown = e.ChangedButton;
        zoomControlMouseDownPoint = e.GetPosition(this);
        contentMouseDownPoint = e.GetPosition(content);

        if (isSpacebarHeld && mouseButtonDown == MouseButton.Left)
        {
            mouseHandlingMode = MouseHandlingModeType.Panning;
        }
        else if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None &&
            (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None &&
            e.ChangedButton is MouseButton.Left or MouseButton.Right)
        {
            mouseHandlingMode = MouseHandlingModeType.Zooming;
        }
        else if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None &&
                 mouseButtonDown == MouseButton.Left)
        {
            mouseHandlingMode = MouseHandlingModeType.Panning;
        }

        if (mouseHandlingMode != MouseHandlingModeType.None)
        {
            if (mouseHandlingMode == MouseHandlingModeType.Panning)
            {
                Cursor = Cursors.Hand;
            }
            else if (mouseHandlingMode == MouseHandlingModeType.Zooming)
            {
                Cursor = Cursors.Cross;
            }

            CaptureMouse();
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseUp(e);

        if (mouseHandlingMode == MouseHandlingModeType.None)
        {
            return;
        }

        if (mouseHandlingMode == MouseHandlingModeType.Zooming)
        {
            switch (mouseButtonDown)
            {
                case MouseButton.Left:
                    ZoomIn(contentMouseDownPoint);
                    break;
                case MouseButton.Right:
                    ZoomOut(contentMouseDownPoint);
                    break;
            }
        }
        else if (mouseHandlingMode == MouseHandlingModeType.DragZooming)
        {
            var finalContentMousePoint = e.GetPosition(content);
            ApplyDragZoomRect(finalContentMousePoint);
        }

        ReleaseMouseCapture();
        mouseHandlingMode = MouseHandlingModeType.None;

        if (!isSpacebarHeld)
        {
            Cursor = null;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (content is null)
        {
            return;
        }

        var curContentMousePoint = e.GetPosition(content);
        var clampedPoint = curContentMousePoint.FilterClamp(content.ActualWidth - 1, content.ActualHeight - 1);

        var oldContentMousePoint = MousePosition;
        MousePosition = clampedPoint;
        OnPropertyChanged(new DependencyPropertyChangedEventArgs(MousePositionProperty, oldContentMousePoint, clampedPoint));

        if (mouseHandlingMode == MouseHandlingModeType.Panning)
        {
            var dragOffset = curContentMousePoint - contentMouseDownPoint;

            ContentOffsetX -= dragOffset.X;
            ContentOffsetY -= dragOffset.Y;

            e.Handled = true;
        }
        else if (mouseHandlingMode == MouseHandlingModeType.Zooming)
        {
            var curZoomControlMousePoint = e.GetPosition(this);
            var dragOffset = curZoomControlMousePoint - zoomControlMouseDownPoint;
            if (mouseButtonDown == MouseButton.Left &&
                (System.Math.Abs(dragOffset.X) > DragZoomThreshold ||
                 System.Math.Abs(dragOffset.Y) > DragZoomThreshold))
            {
                mouseHandlingMode = MouseHandlingModeType.DragZooming;
                InitDragZoomRect(contentMouseDownPoint, curContentMousePoint);
            }
        }
        else if (mouseHandlingMode == MouseHandlingModeType.DragZooming)
        {
            curContentMousePoint = e.GetPosition(this);
            SetDragZoomRect(zoomControlMouseDownPoint, curContentMousePoint);
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseWheel(e);

        // Shift+Scroll → horizontal pan
        if (IsShiftScrollHorizontalPanEnabled &&
            (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None &&
            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
        {
            var panAmount = ContentViewportWidth / 10;
            ContentOffsetX += e.Delta > 0 ? -panAmount : panAmount;
            e.Handled = true;
            return;
        }

        // Ctrl+Scroll → zoom
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
        {
            return;
        }

        DelayedSaveZoom750MilliSeconds();
        e.Handled = true;

        switch (e.Delta)
        {
            case > 0:
                ZoomIn(e.GetPosition(content));
                break;
            case < 0:
                ZoomOut(e.GetPosition(content));
                break;
        }
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseDoubleClick(e);

        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
        {
            SaveZoom();
            AnimatedSnapTo(e.GetPosition(content));
        }
    }

    protected override void OnContentChanged(
        object oldContent,
        object newContent)
    {
        ArgumentNullException.ThrowIfNull(newContent);

        base.OnContentChanged(oldContent, newContent);
        if (oldContent != null)
        {
            ((FrameworkElement)oldContent).SizeChanged -= SetZoomInitialPosition;
        }

        ((FrameworkElement)newContent).SizeChanged += SetZoomInitialPosition;
    }

    /// <summary>
    /// Handles touch manipulation for pinch-to-zoom and two-finger pan.
    /// </summary>
    protected override void OnManipulationStarting(
        ManipulationStartingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!IsTouchEnabled)
        {
            return;
        }

        base.OnManipulationStarting(e);
        e.ManipulationContainer = this;
        e.Mode = ManipulationModes.Scale | ManipulationModes.Translate;
        e.Handled = true;
    }

    /// <summary>
    /// Handles touch manipulation delta for pinch-to-zoom and two-finger pan.
    /// </summary>
    protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!IsTouchEnabled)
        {
            return;
        }

        base.OnManipulationDelta(e);

        var scale = e.DeltaManipulation.Scale;
        if (System.Math.Abs(scale.X - 1.0) > 0.001 || System.Math.Abs(scale.Y - 1.0) > 0.001)
        {
            var zoomFactor = (scale.X + scale.Y) / 2.0;
            var origin = e.ManipulationOrigin;
            var contentPoint = TranslatePoint(origin, content);
            if (content is not null && contentPoint.X >= 0 && contentPoint.Y >= 0)
            {
                ZoomAboutPoint(InternalViewportZoom * zoomFactor, contentPoint);
            }
        }

        var translation = e.DeltaManipulation.Translation;
        if (System.Math.Abs(translation.X) > 0.5 || System.Math.Abs(translation.Y) > 0.5)
        {
            ContentOffsetX -= translation.X / InternalViewportZoom;
            ContentOffsetY -= translation.Y / InternalViewportZoom;
        }

        e.Handled = true;
    }

    /// <summary>
    /// Handles touch manipulation completed.
    /// </summary>
    protected override void OnManipulationCompleted(
        ManipulationCompletedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!IsTouchEnabled)
        {
            return;
        }

        base.OnManipulationCompleted(e);
        SaveZoom();
        e.Handled = true;
    }

    private void OnZoomCommandMessageHandler(ZoomCommandMessage obj)
    {
        switch (obj.CommandType)
        {
            case ZoomCommandMessageType.ZoomToFill:
            {
                zoomFillCommand?.Execute(parameter: null);
                break;
            }

            case ZoomCommandMessageType.ZoomToFit:
            {
                zoomFitCommand?.Execute(parameter: null);
                break;
            }

            case ZoomCommandMessageType.ZoomToPercentage:
            {
                if (obj.Percentage is > 0 and <= 1000)
                {
                    zoomPercentCommand?.Execute(obj.Percentage);
                }

                break;
            }

            default:
                throw new SwitchCaseDefaultException(obj.CommandType);
        }
    }

    private void SetZoomInitialPosition(
        object sender,
        SizeChangedEventArgs e)
    {
        if (content is null)
        {
            return;
        }

        switch (ZoomInitialPosition)
        {
            case ZoomInitialPositionType.Default:
                break;
            case ZoomInitialPositionType.FitScreen:
                InternalViewportZoom = FitZoomValue;
                break;
            case ZoomInitialPositionType.FillScreen:
                InternalViewportZoom = FillZoomValue;
                ContentOffsetX = (content.ActualWidth - (ViewportWidth / InternalViewportZoom)) / 2;
                ContentOffsetY = (content.ActualHeight - (ViewportHeight / InternalViewportZoom)) / 2;
                break;
            case ZoomInitialPositionType.OneHundredPercentCentered:
                InternalViewportZoom = 1.0;
                ContentOffsetX = (content.ActualWidth - ViewportWidth) / 2;
                ContentOffsetY = (content.ActualHeight - ViewportHeight) / 2;
                break;
            default:
                throw new SwitchCaseDefaultException(ZoomInitialPosition);
        }
    }

    private void ZoomOut(Point contentZoomCenter)
        => ZoomAboutPoint(InternalViewportZoom * 0.90909090909, contentZoomCenter);

    private void ZoomIn(Point contentZoomCenter)
        => ZoomAboutPoint(InternalViewportZoom * 1.1, contentZoomCenter);

    private void InitDragZoomRect(
        Point pt1,
        Point pt2)
    {
        if (partDragZoomCanvas is null ||
            partDragZoomBorder is null)
        {
            return;
        }

        partDragZoomCanvas.Visibility = Visibility.Visible;
        partDragZoomBorder.Opacity = 1;
        SetDragZoomRect(pt1, pt2);
    }

    private void SetDragZoomRect(
        Point pt1,
        Point pt2)
    {
        if (partDragZoomBorder is null ||
            partDragZoomCanvas is null)
        {
            return;
        }

        var rect = ViewportHelpers.Clip(
            pt1,
            pt2,
            new Point(0, 0),
            new Point(partDragZoomCanvas.ActualWidth, partDragZoomCanvas.ActualHeight));

        ViewportHelpers.PositionBorderOnCanvas(partDragZoomBorder, rect);
    }

    private void ApplyDragZoomRect(Point finalContentMousePoint)
    {
        if (partDragZoomCanvas is null)
        {
            return;
        }

        var rect = ViewportHelpers.Clip(
            finalContentMousePoint,
            contentMouseDownPoint,
            new Point(0, 0),
            new Point(partDragZoomCanvas.ActualWidth, partDragZoomCanvas.ActualHeight));

        AnimatedZoomTo(rect);
        FadeOutDragZoomRect();
    }

    private void FadeOutDragZoomRect()
    {
        if (partDragZoomBorder is null ||
            partDragZoomCanvas is null)
        {
            return;
        }

        AnimationHelper.StartAnimation(
            partDragZoomBorder,
            OpacityProperty,
            0.0,
            0.1,
            (_, _) =>
            {
                partDragZoomCanvas.Visibility = Visibility.Collapsed;
            },
            UseAnimations);
    }

    private void RaiseCanExecuteChanged()
    {
        zoomPercentCommand?.RaiseCanExecuteChanged();
        zoomOutCommand?.RaiseCanExecuteChanged();
        zoomInCommand?.RaiseCanExecuteChanged();
        zoomFitCommand?.RaiseCanExecuteChanged();
        zoomFillCommand?.RaiseCanExecuteChanged();
        zoomToNextPresetCommand?.RaiseCanExecuteChanged();
        zoomToPreviousPresetCommand?.RaiseCanExecuteChanged();
    }
}