namespace Atc.Wpf.Network.Vnc;

public partial class VncViewerView
{
    private readonly VncKeyboardTranslator keyboardTranslator = new();
    private VncViewerViewModel? viewModel;
    private int lastPointerX;
    private int lastPointerY;
    private byte lastButtonMask;

    /// <summary>
    /// Initializes a new instance of the <see cref="VncViewerView"/> class.
    /// </summary>
    public VncViewerView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        VncImage.MouseDown += OnVncImageMouseButtonDown;
        VncImage.MouseUp += OnVncImageMouseButtonUp;
        VncImage.MouseMove += OnVncImageMouseMove;
        VncImage.MouseWheel += OnVncImageMouseWheel;
        VncImage.LostMouseCapture += OnVncImageLostMouseCapture;

        PreviewKeyDown += OnPreviewKeyDown;
        PreviewKeyUp += OnPreviewKeyUp;
        PreviewTextInput += OnPreviewTextInput;
        LostKeyboardFocus += OnLostKeyboardFocus;
    }

    private void OnDataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (viewModel is not null)
        {
            viewModel.FramebufferUpdated -= OnFramebufferUpdated;
        }

        viewModel = e.NewValue as VncViewerViewModel;

        if (viewModel is not null)
        {
            viewModel.FramebufferUpdated += OnFramebufferUpdated;
        }
    }

    private void OnFramebufferUpdated(
        object? sender,
        EventArgs e)
    {
        _ = Dispatcher.BeginInvoke(RefreshImage);
    }

    private void RefreshImage()
    {
        var frame = viewModel?.CurrentFrame;
        if (frame is not null && VncImage.Source != frame)
        {
            VncImage.Source = frame;
        }
    }

    private async void OnVncImageMouseButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        VncImage.Focus();

        var (x, y) = TranslateToVnc(e);
        if (x < 0)
        {
            return;
        }

        // Keep receiving the move and release while the pointer is dragged outside the image,
        // so the server never keeps a button down.
        if (!VncImage.IsMouseCaptured)
        {
            VncImage.CaptureMouse();
        }

        var buttonMask = GetButtonMask(e, pressed: true);
        await SendPointerAsync(buttonMask, x, y).ConfigureAwait(false);
    }

    private async void OnVncImageMouseButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            ReleaseCaptureIfNoButtonHeld(e);
            return;
        }

        var (x, y) = TranslateToVnc(e);
        var buttonMask = GetButtonMask(e, pressed: false);
        if (buttonMask == 0)
        {
            // Released before the capture is lost, so the release below is the one the server sees.
            lastButtonMask = 0;
            ReleaseCaptureIfNoButtonHeld(e);
        }

        if (x < 0)
        {
            return;
        }

        await SendPointerAsync(buttonMask, x, y).ConfigureAwait(false);
    }

    private async void OnVncImageMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var (x, y) = TranslateToVnc(e);
        if (x < 0)
        {
            return;
        }

        var buttonMask = VncPointerMask.FromButtons(
            e.LeftButton == MouseButtonState.Pressed,
            e.MiddleButton == MouseButtonState.Pressed,
            e.RightButton == MouseButtonState.Pressed);

        await SendPointerAsync(buttonMask, x, y).ConfigureAwait(false);
    }

    private async void OnVncImageMouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var (x, y) = TranslateToVnc(e);
        if (x < 0)
        {
            return;
        }

        e.Handled = true;

        var heldMask = VncPointerMask.FromButtons(
            e.LeftButton == MouseButtonState.Pressed,
            e.MiddleButton == MouseButtonState.Pressed,
            e.RightButton == MouseButtonState.Pressed);

        // RFB has no wheel delta: each notch is a press and release of button 4 (up) or 5 (down).
        var (wheelMask, clicks) = VncPointerMask.GetWheel(e.Delta, heldMask);
        var vm = viewModel;
        for (var i = 0; i < clicks; i++)
        {
            await vm.SendPointerEventAsync(wheelMask, x, y).ConfigureAwait(true);
            await vm.SendPointerEventAsync(heldMask, x, y).ConfigureAwait(true);
        }
    }

    private async void OnVncImageLostMouseCapture(
        object sender,
        MouseEventArgs e)
    {
        // Capture taken away mid-drag (Alt+Tab, a pop-up): release the buttons on the server.
        if (lastButtonMask == 0 ||
            viewModel is null ||
            !viewModel.IsConnected)
        {
            return;
        }

        await SendPointerAsync(0, lastPointerX, lastPointerY).ConfigureAwait(false);
    }

    private void ReleaseCaptureIfNoButtonHeld(MouseEventArgs e)
    {
        if (VncImage.IsMouseCaptured &&
            e.LeftButton == MouseButtonState.Released &&
            e.MiddleButton == MouseButtonState.Released &&
            e.RightButton == MouseButtonState.Released)
        {
            VncImage.ReleaseMouseCapture();
        }
    }

    private Task SendPointerAsync(
        byte buttonMask,
        int x,
        int y)
    {
        lastButtonMask = buttonMask;
        lastPointerX = x;
        lastPointerY = y;
        return viewModel?.SendPointerEventAsync(buttonMask, x, y) ?? Task.CompletedTask;
    }

    private async void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        // Printable keys give no events here; they are sent from the text they type.
        var events = keyboardTranslator.KeyDown(GetRealKey(e));
        if (events.Count == 0)
        {
            return;
        }

        e.Handled = true;
        await SendKeyEventsAsync(events).ConfigureAwait(false);
    }

    private async void OnPreviewKeyUp(
        object sender,
        KeyEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var events = keyboardTranslator.KeyUp(GetRealKey(e));
        if (events.Count == 0)
        {
            return;
        }

        e.Handled = true;
        await SendKeyEventsAsync(events).ConfigureAwait(false);
    }

    private async void OnPreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var events = keyboardTranslator.TextInput(e.Text);
        if (events.Count == 0)
        {
            return;
        }

        e.Handled = true;
        await SendKeyEventsAsync(events).ConfigureAwait(false);
    }

    private async void OnLostKeyboardFocus(
        object sender,
        KeyboardFocusChangedEventArgs e)
    {
        if (IsKeyboardFocusWithin)
        {
            return;
        }

        // Keys released while another window has focus never reach us: release them on the server.
        var events = keyboardTranslator.ReleaseAll();
        if (events.Count == 0 ||
            viewModel is null ||
            !viewModel.IsConnected)
        {
            return;
        }

        await SendKeyEventsAsync(events).ConfigureAwait(false);
    }

    private async Task SendKeyEventsAsync(IReadOnlyList<VncKeyEvent> events)
    {
        var vm = viewModel;
        if (vm is null)
        {
            return;
        }

        foreach (var keyEvent in events)
        {
            await vm.SendKeyEventAsync(keyEvent.KeySym, keyEvent.Pressed).ConfigureAwait(false);
        }
    }

    private static Key GetRealKey(KeyEventArgs e)
        => e.Key switch
        {
            Key.System => e.SystemKey,
            Key.ImeProcessed => e.ImeProcessedKey,
            Key.DeadCharProcessed => e.DeadCharProcessedKey,
            _ => e.Key,
        };

    private (int X, int Y) TranslateToVnc(MouseEventArgs e)
    {
        if (viewModel is null)
        {
            return (-1, -1);
        }

        var fbWidth = viewModel.FramebufferWidth;
        var fbHeight = viewModel.FramebufferHeight;
        if (fbWidth == 0 || fbHeight == 0)
        {
            return (-1, -1);
        }

        var pos = e.GetPosition(VncImage);
        var actualWidth = VncImage.ActualWidth;
        var actualHeight = VncImage.ActualHeight;

        var scale = System.Math.Min(actualWidth / fbWidth, actualHeight / fbHeight);
        var renderedW = fbWidth * scale;
        var renderedH = fbHeight * scale;
        var offsetX = (actualWidth - renderedW) / 2;
        var offsetY = (actualHeight - renderedH) / 2;

        var vncX = (int)((pos.X - offsetX) / scale);
        var vncY = (int)((pos.Y - offsetY) / scale);

        vncX = System.Math.Clamp(vncX, 0, fbWidth - 1);
        vncY = System.Math.Clamp(vncY, 0, fbHeight - 1);

        return (vncX, vncY);
    }

    private static byte GetButtonMask(
        MouseButtonEventArgs e,
        bool pressed)
        => VncPointerMask.ForButtonChange(
            e.LeftButton == MouseButtonState.Pressed,
            e.MiddleButton == MouseButtonState.Pressed,
            e.RightButton == MouseButtonState.Pressed,
            e.ChangedButton,
            pressed);

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (viewModel is null)
        {
            return;
        }

        // Unloaded detaches the framebuffer handler; a view that is loaded again (tab switch, re-parenting)
        // has to pick the live session up again. The input handlers on our own children stay attached.
        viewModel.FramebufferUpdated -= OnFramebufferUpdated;
        viewModel.FramebufferUpdated += OnFramebufferUpdated;
        RefreshImage();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (viewModel is not null)
        {
            viewModel.FramebufferUpdated -= OnFramebufferUpdated;
        }
    }
}