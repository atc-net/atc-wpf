namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how children are aligned along the cross axis in a FlexPanel.
/// </summary>
public enum FlexAlign
{
    /// <summary>
    /// Uses the parent's AlignItems value (for AlignSelf only).
    /// </summary>
    Auto,

    /// <summary>
    /// Children are stretched to fill the cross axis.
    /// </summary>
    Stretch,

    /// <summary>
    /// Children are aligned at the start of the cross axis.
    /// </summary>
    Start,

    /// <summary>
    /// Children are aligned at the end of the cross axis.
    /// </summary>
    End,

    /// <summary>
    /// Children are centered along the cross axis.
    /// </summary>
    Center,

    /// <summary>
    /// Children are aligned at their baselines.
    /// </summary>
    Baseline,
}