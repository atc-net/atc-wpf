namespace Atc.Wpf.WindowsNative.Structs;

/// <summary>
/// The Win32 <c>WINDOWPLACEMENT</c> structure, which contains the show state and the restored, minimized and maximized positions of a window.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "OK.")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
public struct WINDOWPLACEMENT
{
    /// <summary>The size of the structure, in bytes.</summary>
    public int length;

    /// <summary>The flags that control the position of the minimized window and the restore method.</summary>
    public int flags;

    /// <summary>The current show state of the window (a <c>SW_*</c> value).</summary>
    public int showCmd;

    /// <summary>The upper-left corner of the window when it is minimized.</summary>
    public POINT minPosition;

    /// <summary>The upper-left corner of the window when it is maximized.</summary>
    public POINT maxPosition;

    /// <summary>The window's coordinates when it is in the restored position.</summary>
    public RECT normalPosition;
}