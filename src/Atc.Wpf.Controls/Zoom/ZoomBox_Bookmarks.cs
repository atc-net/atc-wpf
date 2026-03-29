namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    private readonly ObservableCollection<ViewBookmark> viewBookmarks = [];
    private RelayCommand<string>? saveViewBookmarkCommand;
    private RelayCommand<ViewBookmark>? restoreViewBookmarkCommand;
    private RelayCommand<ViewBookmark>? removeViewBookmarkCommand;

    /// <summary>
    /// Gets the collection of saved view bookmarks.
    /// </summary>
    public IReadOnlyList<ViewBookmark> ViewBookmarks => viewBookmarks;

    /// <summary>
    /// Command to save the current viewport as a named bookmark.
    /// The command parameter is the bookmark name (string).
    /// </summary>
    public ICommand SaveViewBookmarkCommand
        => saveViewBookmarkCommand ??= new RelayCommand<string>(
            name =>
            {
                var existing = viewBookmarks.FirstOrDefault(
                    b => string.Equals(b.Name, name, StringComparison.Ordinal));
                if (existing is not null)
                {
                    var index = viewBookmarks.IndexOf(existing);
                    viewBookmarks[index] = new ViewBookmark(name, CurrentViewportState);
                }
                else
                {
                    viewBookmarks.Add(new ViewBookmark(name, CurrentViewportState));
                }
            },
            name => !string.IsNullOrWhiteSpace(name));

    /// <summary>
    /// Command to restore a previously saved bookmark.
    /// The command parameter is the <see cref="ViewBookmark"/> to restore.
    /// </summary>
    public ICommand RestoreViewBookmarkCommand
        => restoreViewBookmarkCommand ??= new RelayCommand<ViewBookmark>(
            bookmark =>
            {
                SaveZoom();
                RestoreViewportState(bookmark.State);
            },
            bookmark => bookmark is not null);

    /// <summary>
    /// Command to remove a bookmark from the collection.
    /// </summary>
    public ICommand RemoveViewBookmarkCommand
        => removeViewBookmarkCommand ??= new RelayCommand<ViewBookmark>(
            bookmark => viewBookmarks.Remove(bookmark),
            bookmark => bookmark is not null);

    /// <summary>
    /// Saves the current viewport state as a named bookmark.
    /// If a bookmark with the same name exists, it is updated.
    /// </summary>
    public ViewBookmark SaveViewBookmark(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var bookmark = new ViewBookmark(name, CurrentViewportState);

        var existing = viewBookmarks.FirstOrDefault(
            b => string.Equals(b.Name, name, StringComparison.Ordinal));
        if (existing is not null)
        {
            var index = viewBookmarks.IndexOf(existing);
            viewBookmarks[index] = bookmark;
        }
        else
        {
            viewBookmarks.Add(bookmark);
        }

        return bookmark;
    }

    /// <summary>
    /// Removes all saved bookmarks.
    /// </summary>
    public void ClearViewBookmarks()
        => viewBookmarks.Clear();
}