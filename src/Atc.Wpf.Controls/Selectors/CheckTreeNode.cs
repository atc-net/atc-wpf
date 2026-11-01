namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A node in a tri-state check tree (<see cref="CheckTreeView"/>, <see cref="TreeSelectComboBox"/>).
/// </summary>
/// <remarks>
/// The check state (<see cref="IsChecked"/>) is kept by the control from its <c>SelectedKeys</c>, so a view model only
/// binds keys. Add children to <see cref="Children"/>; for lazy loading set <see cref="HasUnloadedChildren"/> and give
/// the control a <c>LoadChildrenAsync</c> callback.
/// </remarks>
public sealed partial class CheckTreeNode : ObservableObject
{
    /// <summary>
    /// The display text of the node (also its UI Automation name and what the filter matches).
    /// </summary>
    [ObservableProperty]
    private string text = string.Empty;

    /// <summary>
    /// An optional icon shown before the text (any content, e.g. an image or a font icon).
    /// </summary>
    [ObservableProperty]
    private object? icon;

    /// <summary>
    /// Whether the node can be ticked or unticked. A disabled node keeps its state when its parent is ticked.
    /// </summary>
    [ObservableProperty]
    private bool isEnabled = true;

    /// <summary>
    /// Whether the node is expanded (two-way with the tree).
    /// </summary>
    [ObservableProperty]
    private bool isExpanded;

    /// <summary>
    /// Whether the node has children that are not loaded yet; the control calls its <c>LoadChildrenAsync</c> callback
    /// on the first expand.
    /// </summary>
    [ObservableProperty]
    private bool hasUnloadedChildren;

    /// <summary>
    /// Any value the application wants to keep with the node.
    /// </summary>
    [ObservableProperty]
    private object? tag;

    private bool? isChecked = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckTreeNode"/> class.
    /// </summary>
    public CheckTreeNode()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckTreeNode"/> class with a key and a text.
    /// </summary>
    /// <param name="key">The selection key.</param>
    /// <param name="text">The display text.</param>
    public CheckTreeNode(
        string key,
        string text)
    {
        Key = key;
        this.text = text;
    }

    /// <summary>
    /// Gets the selection key of the node; unique in the tree (adding another node with the same key throws an <see cref="InvalidOperationException"/>).
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public ObservableCollection<CheckTreeNode> Children { get; } = [];

    /// <summary>
    /// Gets the check state, kept by the control: <see langword="true"/> (ticked), <see langword="false"/> (not ticked)
    /// or <see langword="null"/> (only some of the children are ticked).
    /// </summary>
    public bool? IsChecked
    {
        get => isChecked;
        internal set
        {
            if (isChecked == value)
            {
                return;
            }

            isChecked = value;
            RaisePropertyChanged(nameof(IsChecked));
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => Text;
}