using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Atc.Wpf.WindowsNative.Structs
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "OK.")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "OK.")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Width => Right - Left;

        public int Height => Bottom - Top;

        public void Offset(int dx, int dy)
        {
            Left += dx;
            Top += dy;
            Right += dx;
            Bottom += dy;
        }

        public bool IsEmpty => Left >= Right || Top >= Bottom;
    }
}