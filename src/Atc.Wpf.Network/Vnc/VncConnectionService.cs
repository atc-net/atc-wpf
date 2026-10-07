namespace Atc.Wpf.Network.Vnc;

public sealed class VncConnectionService : IDisposable
{
    private readonly Func<string, int, IVncClient> clientFactory;
    private readonly Dispatcher dispatcher;
    private IVncClient? vnc;
    private WriteableBitmap? framebuffer;

    public VncConnectionService()
        : this((hostname, port) => new VncClient(hostname, port, new VncClientConfig()))
    {
    }

    internal VncConnectionService(Func<string, int, IVncClient> clientFactory)
    {
        this.clientFactory = clientFactory;
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
    }

    public bool IsConnected { get; private set; }

    public int FramebufferWidth => framebuffer?.PixelWidth ?? 0;

    public int FramebufferHeight => framebuffer?.PixelHeight ?? 0;

    public WriteableBitmap? CurrentFrame => framebuffer;

    public event EventHandler? Connected;

    public event EventHandler? Disconnected;

    public event EventHandler<VncConnectionFailedEventArgs>? ConnectionFailed;

    public event EventHandler? FramebufferUpdated;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "VNC connection errors should be reported, not thrown.")]
    public async Task ConnectAsync(
        string hostname,
        int port,
        string? password)
    {
        Cleanup();

        try
        {
            vnc = clientFactory(hostname, port);
            vnc.ConnectionLost += OnConnectionLost;
            vnc.FramebufferUpdated += OnVncFramebufferUpdated;

            var connected = await vnc.Connect().ConfigureAwait(false);
            if (!connected)
            {
                Cleanup();
                ConnectionFailed?.Invoke(this, new VncConnectionFailedEventArgs(Resources.Resources.VncConnectionFailed));
                return;
            }

            var authenticated = await vnc.Authenticate(password ?? string.Empty).ConfigureAwait(false);
            if (!authenticated)
            {
                Cleanup();
                ConnectionFailed?.Invoke(this, new VncConnectionFailedEventArgs(Resources.Resources.VncAuthenticationFailed));
                return;
            }

            await vnc.Initialize().ConfigureAwait(false);

            var fb = vnc.Framebuffer!;

            await dispatcher.InvokeAsync(() =>
            {
                framebuffer = new WriteableBitmap(
                    fb.Width,
                    fb.Height,
                    96,
                    96,
                    System.Windows.Media.PixelFormats.Bgra32,
                    palette: null);
            });

            await vnc.StartUpdates().ConfigureAwait(false);

            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Cleanup();
            var msg = ex.InnerException?.Message ?? ex.Message;
            ConnectionFailed?.Invoke(
                this,
                new VncConnectionFailedEventArgs(
                    string.Format(CultureInfo.CurrentCulture, Resources.Resources.VncConnectionFailedFormat1, msg)));
        }
    }

    public async Task DisconnectAsync()
    {
        if (vnc is not null)
        {
            await vnc.Disconnect().ConfigureAwait(false);
        }

        Cleanup();
        IsConnected = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failed send is reported as a lost connection.")]
    public async Task SendPointerEventAsync(
        byte buttonMask,
        int x,
        int y)
    {
        if (vnc is null || !IsConnected)
        {
            return;
        }

        try
        {
            await vnc.SendPointerEvent(buttonMask, x, y).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Input is sent from fire-and-forget UI handlers; a dropped socket must surface as a
            // disconnect, not as an exception that reaches the dispatcher unhandled.
            OnConnectionLost();
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failed send is reported as a lost connection.")]
    public async Task SendKeyEventAsync(
        uint keySym,
        bool pressed)
    {
        if (vnc is null || !IsConnected)
        {
            return;
        }

        try
        {
            await vnc.SendKeyEvent(keySym, pressed).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Input is sent from fire-and-forget UI handlers; a dropped socket must surface as a
            // disconnect, not as an exception that reaches the dispatcher unhandled.
            OnConnectionLost();
        }
    }

    public void Dispose()
        => Cleanup();

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Rendering errors must not crash the application.")]
    private void OnVncFramebufferUpdated(
        object? sender,
        VncFramebufferUpdateEventArgs e)
    {
        if (framebuffer is null)
        {
            return;
        }

        try
        {
            var fb = e.Framebuffer;
            var rect = e.Rectangle;

            _ = dispatcher.BeginInvoke(() =>
            {
                try
                {
                    framebuffer.Lock();

                    var sourceData = fb.PixelData;
                    var stride = framebuffer.BackBufferStride;
                    var bytesPerPixel = 4;

                    for (var y = rect.Y; y < rect.Y + rect.Height && y < fb.Height; y++)
                    {
                        var destOffset = (y * stride) + (rect.X * bytesPerPixel);
                        var srcOffset = (y * fb.Width) + rect.X;
                        var pixelCount = System.Math.Min(rect.Width, fb.Width - rect.X);

                        Marshal.Copy(sourceData, srcOffset, framebuffer.BackBuffer + destOffset, pixelCount);
                    }

                    framebuffer.AddDirtyRect(new Int32Rect(
                        rect.X,
                        rect.Y,
                        System.Math.Min(rect.Width, fb.Width - rect.X),
                        System.Math.Min(rect.Height, fb.Height - rect.Y)));

                    framebuffer.Unlock();
                }
                catch
                {
                    // Ignore rendering errors
                }

                FramebufferUpdated?.Invoke(this, EventArgs.Empty);
            });
        }
        catch
        {
            // Ignore rendering errors from background thread
        }
    }

    private void OnConnectionLost()
    {
        // Both a failed send and the client's own ConnectionLost event end up here; report the loss once.
        if (!IsConnected)
        {
            return;
        }

        IsConnected = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    private void Cleanup()
    {
        if (vnc is not null)
        {
            vnc.FramebufferUpdated -= OnVncFramebufferUpdated;
            vnc.ConnectionLost -= OnConnectionLost;
            vnc.Dispose();
            vnc = null;
        }

        framebuffer = null;
    }
}