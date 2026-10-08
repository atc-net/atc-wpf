namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// <see cref="ITrayIconHost"/> on top of <c>Shell_NotifyIcon</c>, with a hidden top-level window for the callbacks.
/// </summary>
/// <remarks>
/// The window is a top-level window (never shown) rather than a message-only window, because message-only
/// windows don't receive the broadcast <c>TaskbarCreated</c> message that Explorer sends after a restart.
/// The icon is identified by window and id, not by GUID: a GUID is bound to the executable path.
/// </remarks>
[SuppressMessage("Security", "S6640:Make sure that using \"unsafe\" is safe here", Justification = "OK - By design.")]
internal sealed class ShellTrayIconHost : ITrayIconHost
{
    private const uint CallbackMessage = PInvoke.WM_APP + 1;

    private static int lastId;

    private readonly uint id = (uint)Interlocked.Increment(ref lastId);
    private readonly uint taskbarCreatedMessage;
    private readonly HwndSource window;
    private HICON icon;
    private bool isAdded;

    public ShellTrayIconHost()
    {
        taskbarCreatedMessage = PInvoke.RegisterWindowMessage("TaskbarCreated");
        window = new HwndSource(new HwndSourceParameters("AtcTrayIcon")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0,
        });
        window.AddHook(WndProc);
    }

    public Action<TrayIconAction>? ActionReceived { get; set; }

    public Action? TaskbarCreated { get; set; }

    public bool Add(
        ImageSource? icon,
        string toolTip)
    {
        // Add runs again after an Explorer restart, so release the icon of the previous add.
        DestroyIcon(ReplaceIcon(icon));

        var data = CreateData(toolTip);
        isAdded = NotifyIconNativeMethods.ShellNotifyIcon(NotifyIconNativeMethods.NimAdd, ref data);
        if (!isAdded)
        {
            return false;
        }

        data.VersionOrTimeout = NotifyIconNativeMethods.NotifyIconVersion4;
        NotifyIconNativeMethods.ShellNotifyIcon(NotifyIconNativeMethods.NimSetVersion, ref data);
        return true;
    }

    public void Update(
        ImageSource? icon,
        string toolTip)
    {
        if (!isAdded)
        {
            return;
        }

        var previousIcon = ReplaceIcon(icon);
        var data = CreateData(toolTip);
        NotifyIconNativeMethods.ShellNotifyIcon(NotifyIconNativeMethods.NimModify, ref data);
        DestroyIcon(previousIcon);
    }

    public void SetForegroundWindow(nint handle)
        => PInvoke.SetForegroundWindow(new HWND(handle));

    public void Dispose()
    {
        if (isAdded)
        {
            var data = new NotifyIconNativeMethods.NotifyIconData
            {
                Size = (uint)Marshal.SizeOf<NotifyIconNativeMethods.NotifyIconData>(),
                WindowHandle = window.Handle,
                Id = id,
            };
            NotifyIconNativeMethods.ShellNotifyIcon(NotifyIconNativeMethods.NimDelete, ref data);
            isAdded = false;
        }

        window.RemoveHook(WndProc);
        window.Dispose();
        DestroyIcon(icon);
        icon = HICON.Null;
    }

    private NotifyIconNativeMethods.NotifyIconData CreateData(string toolTip)
    {
        var data = new NotifyIconNativeMethods.NotifyIconData
        {
            Size = (uint)Marshal.SizeOf<NotifyIconNativeMethods.NotifyIconData>(),
            WindowHandle = window.Handle,
            Id = id,
            Flags = NotifyIconNativeMethods.NifMessage | NotifyIconNativeMethods.NifIcon | NotifyIconNativeMethods.NifTip | NotifyIconNativeMethods.NifShowTip,
            CallbackMessage = CallbackMessage,
            IconHandle = icon.IsNull
                ? PInvoke.LoadIcon(HINSTANCE.Null, PInvoke.IDI_APPLICATION)
                : icon,
        };
        data.SetToolTip(TrayIconMessageDecoder.TruncateToolTip(toolTip));
        return data;
    }

    // Returns the previous icon, which the caller destroys once the shell no longer uses it.
    private HICON ReplaceIcon(ImageSource? source)
    {
        var previousIcon = icon;
        icon = source is null
            ? HICON.Null
            : CreateIcon(source);
        return previousIcon;
    }

    private static void DestroyIcon(HICON handle)
    {
        if (!handle.IsNull)
        {
            PInvoke.DestroyIcon(handle);
        }
    }

    private static unsafe HICON CreateIcon(ImageSource source)
    {
        var size = PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON);
        var pixels = TrayIconImageRenderer.RenderBgra(source, size);

        // 1 bpp mask rows are padded to 16 bits; all zero = opaque, the color bitmap's alpha does the rest.
        var mask = new byte[((size + 15) / 16) * 2 * size];

        fixed (byte* pixelsPointer = pixels)
        {
            fixed (byte* maskPointer = mask)
            {
                var colorBitmap = PInvoke.CreateBitmap(size, size, 1, 32, pixelsPointer);
                var maskBitmap = PInvoke.CreateBitmap(size, size, 1, 1, maskPointer);
                try
                {
                    var iconInfo = new ICONINFO
                    {
                        fIcon = true,
                        hbmColor = colorBitmap,
                        hbmMask = maskBitmap,
                    };
                    return PInvoke.CreateIconIndirect(&iconInfo);
                }
                finally
                {
                    PInvoke.DeleteObject(colorBitmap);
                    PInvoke.DeleteObject(maskBitmap);
                }
            }
        }
    }

    private IntPtr WndProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg == CallbackMessage)
        {
            var action = TrayIconMessageDecoder.Decode(lParam.ToInt64());
            if (action != TrayIconAction.None)
            {
                ActionReceived?.Invoke(action);
            }

            handled = true;
        }
        else if (msg == taskbarCreatedMessage)
        {
            isAdded = false;
            TaskbarCreated?.Invoke();
        }

        return IntPtr.Zero;
    }
}