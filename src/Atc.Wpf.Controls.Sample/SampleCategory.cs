namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// A category of samples in a <see cref="SampleExplorer"/>: a tab with a badge, and the sample tree it shows.
/// </summary>
public sealed class SampleCategory
{
    /// <summary>
    /// Gets or sets the key of the category, e.g. <c>WpfControls</c>. It names the automation IDs of the tab
    /// (<c>Tab{Key}</c>) and the tree (<c>StvSamples{Key}</c>); the keys of an explorer are unique.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text of the category's tab, e.g. <c>Wpf.Controls</c>.
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the category's sample tree: a <see cref="TreeView"/> subclass with a public
    /// parameterless constructor, usually <c>Samples{Key}TreeView</c>. The explorer creates it.
    /// </summary>
    public Type? TreeType { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Key)}: {Key}, {nameof(Header)}: {Header}, {nameof(TreeType)}: {TreeType?.Name}";
}