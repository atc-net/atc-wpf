namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// The seven segments (and their combinations) of a <see cref="SevenSegment"/> digit.
/// </summary>
[Flags]
public enum SevenSegmentParts
{
    /// <summary>No segment is lit.</summary>
    None = 0,

    /// <summary>The top horizontal segment (a).</summary>
    Top = 1,

    /// <summary>The top-right vertical segment (b).</summary>
    TopRight = 2,

    /// <summary>The bottom-right vertical segment (c).</summary>
    BottomRight = 4,

    /// <summary>The bottom horizontal segment (d).</summary>
    Bottom = 8,

    /// <summary>The bottom-left vertical segment (e).</summary>
    BottomLeft = 16,

    /// <summary>The top-left vertical segment (f).</summary>
    TopLeft = 32,

    /// <summary>The middle horizontal segment (g).</summary>
    Middle = 64,

    /// <summary>All seven segments.</summary>
    All = Top | TopRight | BottomRight | Bottom | BottomLeft | TopLeft | Middle,
}