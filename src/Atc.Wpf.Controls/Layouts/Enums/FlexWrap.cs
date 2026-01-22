namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies whether children should wrap when they overflow the main axis in a FlexPanel.
/// </summary>
public enum FlexWrap
{
    /// <summary>
    /// Children are laid out in a single line and may overflow.
    /// </summary>
    NoWrap,

    /// <summary>
    /// Children wrap onto multiple lines from top to bottom.
    /// </summary>
    Wrap,

    /// <summary>
    /// Children wrap onto multiple lines from bottom to top.
    /// </summary>
    WrapReverse,
}
