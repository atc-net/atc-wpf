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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "OK.")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }
}