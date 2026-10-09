namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Manages a VNC client connection: connecting and authenticating, rendering framebuffer
/// updates into a <see cref="WriteableBitmap"/>, and forwarding pointer and key input.
/// </summary>
public sealed class VncConnectionService : IDisposable
{
    private readonly Func<string, int, IVncClient> clientFactory;
    private readonly Dispatcher dispatcher;
    private IVncClient? vnc;
    private WriteableBitmap? framebuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="VncConnectionService"/> class.
    /// </summary>
    public VncConnectionService()
        : this((hostname, port) => new VncClient(hostname, port, new VncClientConfig()))
    {
    }

    internal VncConnectionService(Func<string, int, IVncClient> clientFactory)
    {
        this.clientFactory = clientFactory;
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
    }

    /// <summary>
    /// Gets a value indicating whether a VNC session is currently connected.
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Gets the width, in pixels, of the remote framebuffer, or 0 when not connected.
    /// </summary>
    public int FramebufferWidth => framebuffer?.PixelWidth ?? 0;

    /// <summary>
    /// Gets the height, in pixels, of the remote framebuffer, or 0 when not connected.
    /// </summary>
    public int FramebufferHeight => framebuffer?.PixelHeight ?? 0;

    /// <summary>
    /// Gets the bitmap holding the current remote screen image, or <see langword="null"/> when not connected.
    /// </summary>
    public WriteableBitmap? CurrentFrame => framebuffer;

    /// <summary>
    /// Occurs when the connection has been established and screen updates have started.
    /// </summary>
    public event EventHandler? Connected;

    /// <summary>
    /// Occurs when the connection is closed or lost.
    /// </summary>
    public event EventHandler? Disconnected;

    /// <summary>
    /// Occurs when connecting or authenticating fails.
    /// </summary>
    public event EventHandler<VncConnectionFailedEventArgs>? ConnectionFailed;

    /// <summary>
    /// Occurs when a region of <see cref="CurrentFrame"/> has been updated.
    /// </summary>
    public event EventHandler? FramebufferUpdated;

    /// <summary>
    /// Connects to and authenticates with a VNC server, then starts receiving screen updates.
    /// Failures are reported through <see cref="ConnectionFailed"/> rather than thrown.
    /// </summary>
    /// <param name="hostname">The host name or IP address of the VNC server.</param>
    /// <param name="port">The TCP port of the VNC server.</param>
    /// <param name="password">The optional password used for authentication.</param>
    /// <returns>A task that completes when the connection attempt has finished.</returns>
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

    /// <summary>
    /// Disconnects from the VNC server and raises <see cref="Disconnected"/>.
    /// </summary>
    /// <returns>A task that completes when the connection has been closed.</returns>
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

    /// <summary>
    /// Sends a pointer (mouse) event to the VNC server; does nothing when not connected.
    /// A failed send is reported as a lost connection.
    /// </summary>
    /// <param name="buttonMask">The pressed-buttons bit mask (1 = left, 2 = middle, 4 = right).</param>
    /// <param name="x">The horizontal framebuffer coordinate.</param>
    /// <param name="y">The vertical framebuffer coordinate.</param>
    /// <returns>A task that completes when the event has been sent.</returns>
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

    /// <summary>
    /// Sends a key press or release event to the VNC server; does nothing when not connected.
    /// A failed send is reported as a lost connection.
    /// </summary>
    /// <param name="keySym">The X11 keysym of the key.</param>
    /// <param name="pressed"><see langword="true"/> for a key press; <see langword="false"/> for a key release.</param>
    /// <returns>A task that completes when the event has been sent.</returns>
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

    /// <inheritdoc />
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