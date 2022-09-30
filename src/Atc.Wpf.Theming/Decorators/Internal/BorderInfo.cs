namespace Atc.Wpf.Theming.Decorators.Internal;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[StructLayout(LayoutKind.Auto)]
internal struct BorderInfo
{
    internal readonly double LeftTop;
    internal readonly double TopLeft;
    internal readonly double TopRight;
    internal readonly double RightTop;
    internal readonly double RightBottom;
    internal readonly double BottomRight;
    internal readonly double BottomLeft;
    internal readonly double LeftBottom;

    /// <summary>
    /// Encapsulates the details of each of the core points of the border which is calculated
    /// based on the given CornerRadius, BorderThickness, Padding and a flag to indicate whether
    /// the inner or outer border is to be calculated.
    /// </summary>
    /// <param name="corners">CornerRadius</param>
    /// <param name="borders">BorderThickness</param>
    /// <param name="padding">Padding</param>
    /// <param name="isOuterBorder">Flag to indicate whether outer or inner border needs to be calculated</param>
    internal BorderInfo(CornerRadius corners, Thickness borders, Thickness padding, bool isOuterBorder)
    {
        var left = (0.5 * borders.Left) + padding.Left;
        var top = (0.5 * borders.Top) + padding.Top;
        var right = (0.5 * borders.Right) + padding.Right;
        var bottom = (0.5 * borders.Bottom) + padding.Bottom;

        if (isOuterBorder)
        {
            if (corners.TopLeft.IsZero())
            {
                LeftTop = TopLeft = 0.0;
            }
            else
            {
                LeftTop = corners.TopLeft + left;
                TopLeft = corners.TopLeft + top;
            }

            if (corners.TopRight.IsZero())
            {
                TopRight = RightTop = 0.0;
            }
            else
            {
                TopRight = corners.TopRight + top;
                RightTop = corners.TopRight + right;
            }

            if (corners.BottomRight.IsZero())
            {
                RightBottom = BottomRight = 0.0;
            }
            else
            {
                RightBottom = corners.BottomRight + right;
                BottomRight = corners.BottomRight + bottom;
            }

            if (corners.BottomLeft.IsZero())
            {
                BottomLeft = LeftBottom = 0.0;
            }
            else
            {
                BottomLeft = corners.BottomLeft + bottom;
                LeftBottom = corners.BottomLeft + left;
            }
        }
        else
        {
            LeftTop = System.Math.Max(0.0, corners.TopLeft - left);
            TopLeft = System.Math.Max(0.0, corners.TopLeft - top);
            TopRight = System.Math.Max(0.0, corners.TopRight - top);
            RightTop = System.Math.Max(0.0, corners.TopRight - right);
            RightBottom = System.Math.Max(0.0, corners.BottomRight - right);
            BottomRight = System.Math.Max(0.0, corners.BottomRight - bottom);
            BottomLeft = System.Math.Max(0.0, corners.BottomLeft - bottom);
            LeftBottom = System.Math.Max(0.0, corners.BottomLeft - left);
        }
    }
}