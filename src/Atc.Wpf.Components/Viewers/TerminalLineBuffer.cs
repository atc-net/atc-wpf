namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// The lines shown by the <see cref="TerminalViewer"/>, with the search filter and match count kept up to date as
/// lines arrive and are trimmed.
/// </summary>
/// <remarks>
/// The lines live in an observable collection bound as the list's items source, so its collection view filters each
/// new line as it is added. The filter is only set again when the search changes; setting it per batch would reset
/// the list (regenerating every row and losing the scroll position) and rescan every line. The match count is
/// adjusted by the added and trimmed lines instead of being recounted.
/// </remarks>
internal sealed class TerminalLineBuffer
{
    /// <summary>
    /// Above this many lines to trim at once, and when the trim replaces most of the list (a resume after a pause,
    /// or a lowered cap), the oldest lines are removed in one pass with a single reset notification instead of one
    /// removal notification (and one shift of the whole list) per line. A steady stream at the cap trims each batch
    /// line by line, so it never resets the list.
    /// </summary>
    private const int BulkTrimThreshold = 256;

    private readonly TerminalLineCollection lines = [];
    private readonly ICollectionView view;
    private readonly Predicate<object> filterPredicate;

    private TerminalLineItem? lastEvaluatedLine;
    private bool lastEvaluatedResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLineBuffer"/> class.
    /// </summary>
    public TerminalLineBuffer()
    {
        view = CollectionViewSource.GetDefaultView(lines);
        filterPredicate = item => item is not TerminalLineItem line || Matches(line);
    }

    /// <summary>
    /// Gets all lines, including the ones the search filter hides. Bind it as the list's items source.
    /// </summary>
    public ObservableCollection<TerminalLineItem> Lines
        => lines;

    /// <summary>
    /// Gets the search matcher.
    /// </summary>
    public TerminalSearchMatcher Matcher { get; } = new();

    /// <summary>
    /// Gets the number of lines that match the active search, or 0 when no search is active.
    /// </summary>
    public int MatchCount { get; private set; }

    /// <summary>
    /// Applies the search: filters the list when <paramref name="hideNonMatching"/> is set, and recounts the matches.
    /// </summary>
    public void ApplySearch(
        string? pattern,
        bool useRegex,
        bool hideNonMatching)
    {
        var patternChanged = Matcher.Update(pattern, useRegex);
        lastEvaluatedLine = null;

        var filterEnabled = hideNonMatching && Matcher.IsActive;
        if (!filterEnabled)
        {
            if (view.Filter is not null)
            {
                view.Filter = null;
            }
        }
        else if (view.Filter is null)
        {
            view.Filter = filterPredicate;
        }
        else if (patternChanged)
        {
            view.Refresh();
        }

        Recount();
    }

    /// <summary>
    /// Appends <paramref name="newLines"/>, then drops the oldest unpinned lines above <paramref name="maxLines"/>
    /// (0 or less disables the cap).
    /// </summary>
    public void Add(
        IEnumerable<TerminalLineItem> newLines,
        int maxLines)
    {
        ArgumentNullException.ThrowIfNull(newLines);

        var timedOutBefore = Matcher.HasTimedOut;
        foreach (var line in newLines)
        {
            if (Matcher.IsActive && Matches(line))
            {
                MatchCount++;
            }

            lines.Add(line);
        }

        Trim(maxLines);

        if (!timedOutBefore && Matcher.HasTimedOut)
        {
            OnSearchTimedOut();
        }
    }

    /// <summary>
    /// Removes every line.
    /// </summary>
    public void Clear()
    {
        lines.Clear();
        lastEvaluatedLine = null;
        MatchCount = 0;
    }

    /// <summary>
    /// Removes every line that is not pinned.
    /// </summary>
    public void ClearUnpinned()
    {
        lines.RemoveOldestUnpinned(int.MaxValue, onRemoved: null);
        lastEvaluatedLine = null;
        Recount();
    }

    private void Trim(int maxLines)
    {
        var excess = lines.Count - maxLines;
        if (maxLines <= 0 || excess <= 0)
        {
            return;
        }

        if (excess > BulkTrimThreshold && excess > lines.Count / 2)
        {
            lines.RemoveOldestUnpinned(excess, OnLineRemoved);
        }
        else
        {
            TerminalLineTrimmer.TrimToCap(lines, maxLines, item => OnLineRemoved((TerminalLineItem)item!));
        }
    }

    private void OnLineRemoved(TerminalLineItem line)
    {
        if (Matcher.IsActive && MatchCount > 0 && Matches(line))
        {
            MatchCount--;
        }
    }

    private void OnSearchTimedOut()
    {
        // The pattern now matches nothing: drop the count and hide the lines that matched before the timeout.
        MatchCount = 0;
        lastEvaluatedLine = null;
        if (view.Filter is not null)
        {
            view.Refresh();
        }
    }

    private void Recount()
    {
        var count = 0;
        if (Matcher.IsActive)
        {
            foreach (var line in lines)
            {
                if (Matcher.IsMatch(line.Text))
                {
                    count++;
                }
            }
        }

        MatchCount = Matcher.HasTimedOut ? 0 : count;
    }

    /// <summary>
    /// Matches a line, reusing the result for the line just counted, so the collection view's filter does not run
    /// the search a second time when the line is added.
    /// </summary>
    private bool Matches(TerminalLineItem line)
    {
        if (ReferenceEquals(line, lastEvaluatedLine))
        {
            return lastEvaluatedResult;
        }

        lastEvaluatedResult = Matcher.IsMatch(line.Text);
        lastEvaluatedLine = line;
        return lastEvaluatedResult;
    }

    /// <summary>
    /// The line collection, with a single-pass removal of the oldest lines.
    /// </summary>
    private sealed class TerminalLineCollection : ObservableCollection<TerminalLineItem>
    {
        public void RemoveOldestUnpinned(
            int count,
            Action<TerminalLineItem>? onRemoved)
        {
            CheckReentrancy();

            var remaining = count;
            var removed = ((List<TerminalLineItem>)Items).RemoveAll(line =>
            {
                if (remaining <= 0 || line.IsPinned)
                {
                    return false;
                }

                remaining--;
                onRemoved?.Invoke(line);
                return true;
            });

            if (removed == 0)
            {
                return;
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}