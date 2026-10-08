namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// <c>Shell_NotifyIconW</c> and its data structure.
/// </summary>
/// <remarks>
/// Declared by hand because CsWin32 generates <c>NOTIFYICONDATAW</c> per CPU architecture (the x86 header packs it
/// to 1 byte). Every field is 4-byte aligned on x86, so the natural layout below matches on x86, x64 and ARM64.
/// </remarks>
[SuppressMessage("Security", "S6640:Make sure that using \"unsafe\" is safe here", Justification = "OK - By design.")]
internal static partial class NotifyIconNativeMethods
{
    public const uint NimAdd = 0x0;
    public const uint NimModify = 0x1;
    public const uint NimDelete = 0x2;
    public const uint NimSetVersion = 0x4;

    public const uint NifMessage = 0x1;
    public const uint NifIcon = 0x2;
    public const uint NifTip = 0x4;
    public const uint NifShowTip = 0x80;

    public const uint NotifyIconVersion4 = 4;

    [LibraryImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShellNotifyIcon(
        uint message,
        ref NotifyIconData data);

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - Nested interop struct.")]
    public unsafe struct NotifyIconData
    {
        public uint Size;
        public nint WindowHandle;
        public uint Id;
        public uint Flags;
        public uint CallbackMessage;
        public nint IconHandle;
        public fixed char ToolTip[128];
        public uint State;
        public uint StateMask;
        public fixed char Info[256];
        public uint VersionOrTimeout;
        public fixed char InfoTitle[64];
        public uint InfoFlags;
        public Guid ItemGuid;
        public nint BalloonIconHandle;

        public void SetToolTip(string text)
        {
            var length = System.Math.Min(text.Length, 127);
            fixed (char* destination = ToolTip)
            {
                var span = new Span<char>(destination, 128);
                text.AsSpan(0, length).CopyTo(span);
                span[length] = '\0';
            }
        }
    }
}