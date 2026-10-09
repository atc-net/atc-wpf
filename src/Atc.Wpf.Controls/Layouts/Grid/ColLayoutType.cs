namespace Atc.Wpf.Controls.Layouts.Grid;

/// <summary>Specifies a responsive layout breakpoint used by <see cref="Row"/> and <see cref="Col"/>.</summary>
public enum ColLayoutType
{
    /// <summary>Extra small: widths below 768.</summary>
    Xs,

    /// <summary>Small: widths from 768 to 991.</summary>
    Sm,

    /// <summary>Medium: widths from 992 to 1199.</summary>
    Md,

    /// <summary>Large: widths from 1200 to 1919.</summary>
    Lg,

    /// <summary>Extra large: widths from 1920 to 2559.</summary>
    Xl,

    /// <summary>Extra extra large: widths of 2560 and above.</summary>
    Xxl,

    /// <summary>No breakpoint-specific span; the column keeps its own span.</summary>
    Auto,
}