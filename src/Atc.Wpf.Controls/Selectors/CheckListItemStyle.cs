namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies how the rows of a <see cref="CheckListBox"/> show whether they are ticked.
/// </summary>
public enum CheckListItemStyle
{
    /// <summary>A check box before the text.</summary>
    CheckBox,

    /// <summary>No box: ticked rows get the accent background and a trailing check mark.</summary>
    Highlight,

    /// <summary>A toggle switch at the end of the row, for settings pages.</summary>
    Switch,
}