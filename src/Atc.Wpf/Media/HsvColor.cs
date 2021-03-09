using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Atc.Wpf.Media
{
    /// <summary>
    /// Hsv Color.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "OK.")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
    [StructLayout(LayoutKind.Auto)]
    public struct HsvColor
    {
        /// <summary>
        /// Value H.
        /// </summary>
        public double H;

        /// <summary>
        /// Value S.
        /// </summary>
        public double S;

        /// <summary>
        /// Value V.
        /// </summary>
        public double V;

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvColor" /> struct.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="s">The s.</param>
        /// <param name="v">The v.</param>
        public HsvColor(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }
    }
}