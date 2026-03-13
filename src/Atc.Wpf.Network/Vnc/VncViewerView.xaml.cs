namespace Atc.Wpf.Network.Vnc;

public partial class VncViewerView
{
    private VncViewerViewModel? viewModel;

    public VncViewerView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
        Unloaded += OnUnloaded;

        VncImage.MouseLeftButtonDown += OnVncImageMouseButtonDown;
        VncImage.MouseLeftButtonUp += OnVncImageMouseButtonUp;
        VncImage.MouseRightButtonDown += OnVncImageMouseButtonDown;
        VncImage.MouseRightButtonUp += OnVncImageMouseButtonUp;
        VncImage.MouseMove += OnVncImageMouseMove;

        PreviewKeyDown += OnPreviewKeyDown;
        PreviewKeyUp += OnPreviewKeyUp;
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

        var buttonMask = GetButtonMask(e, pressed: true);
        await viewModel.SendPointerEventAsync(buttonMask, x, y).ConfigureAwait(false);
    }

    private async void OnVncImageMouseButtonUp(
        object sender,
        MouseButtonEventArgs e)
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

        var buttonMask = GetButtonMask(e, pressed: false);
        await viewModel.SendPointerEventAsync(buttonMask, x, y).ConfigureAwait(false);
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

        byte buttonMask = 0;
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            buttonMask |= 1;
        }

        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            buttonMask |= 2;
        }

        if (e.RightButton == MouseButtonState.Pressed)
        {
            buttonMask |= 4;
        }

        await viewModel.SendPointerEventAsync(buttonMask, x, y).ConfigureAwait(false);
    }

    private async void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (KeySymMapper.TryGetKeySym(key, out var keySym))
        {
            e.Handled = true;
            await viewModel.SendKeyEventAsync(keySym, pressed: true).ConfigureAwait(false);
        }
    }

    private async void OnPreviewKeyUp(
        object sender,
        KeyEventArgs e)
    {
        if (viewModel is null || !viewModel.IsConnected)
        {
            return;
        }

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (KeySymMapper.TryGetKeySym(key, out var keySym))
        {
            e.Handled = true;
            await viewModel.SendKeyEventAsync(keySym, pressed: false).ConfigureAwait(false);
        }
    }

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
    {
        if (!pressed)
        {
            return 0;
        }

        return e.ChangedButton switch
        {
            MouseButton.Left => 1,
            MouseButton.Middle => 2,
            MouseButton.Right => 4,
            _ => 0,
        };
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        VncImage.MouseLeftButtonDown -= OnVncImageMouseButtonDown;
        VncImage.MouseLeftButtonUp -= OnVncImageMouseButtonUp;
        VncImage.MouseRightButtonDown -= OnVncImageMouseButtonDown;
        VncImage.MouseRightButtonUp -= OnVncImageMouseButtonUp;
        VncImage.MouseMove -= OnVncImageMouseMove;

        PreviewKeyDown -= OnPreviewKeyDown;
        PreviewKeyUp -= OnPreviewKeyUp;

        if (viewModel is not null)
        {
            viewModel.FramebufferUpdated -= OnFramebufferUpdated;
        }
    }
}