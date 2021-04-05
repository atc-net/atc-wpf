using System;
using System.Runtime.InteropServices;
using Atc.Wpf.WindowsNative.Structs;

// ReSharper disable IdentifierTypo
namespace Atc.Wpf.WindowsNative.User32
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
    }
}