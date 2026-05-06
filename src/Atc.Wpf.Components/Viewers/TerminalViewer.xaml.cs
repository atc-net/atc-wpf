namespace Atc.Wpf.Components.Viewers;

public sealed partial class TerminalViewer : IDisposable
{
    private const double TailDetectionThresholdPixels = 2d;

    private const double MinFontSize = 6d;
    private const double MaxFontSize = 48d;
    private const double FontSizeStep = 1d;
    private const double DefaultFontSizeValue = 12d;

    private Regex? compiledSearchRegex;
    private int nextLineNumber = 1;
    private AnsiSgrState ansiState = AnsiSgrState.Default;

    // Snapshot fields read by the background drain loop. Reading DP-backed
    // properties from a non-UI thread throws InvalidOperationException
    // ("calling thread cannot access this DispatcherObject"), which would
    // kill the drain task and silently strand all subsequent input. The
    // PropertyChangedCallbacks below keep these snapshots in sync.
    private volatile bool isPausedSnapshot;
    private volatile bool autoScrollSnapshot = true;
    private volatile bool enableAnsiParsingSnapshot = true;

    private readonly Channel<TerminalReceivedDataEventArgs> receivedDataChannel =
        Channel.CreateUnbounded<TerminalReceivedDataEventArgs>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
            });

    private readonly CancellationTokenSource cts = new();
    private readonly Task? queueProcessingTask;
    private ScrollViewer? cachedScrollViewer;

    // True when the user has manually scrolled away from the tail. Only a
    // user-driven scroll (ScrollChangedEventArgs.VerticalChange != 0) flips
    // this — layout-induced events (content overflowed the viewport but the
    // user didn't move) leave it alone.
    private volatile bool isUserDetachedSnapshot;

    // True while we're driving the scroller ourselves (ScrollIntoView /
    // ScrollToBottom). Without this guard, our own programmatic scroll fires
    // ScrollChanged with VerticalChange != 0; under virtualization the
    // post-scroll position is NOT exactly at ScrollableHeight (extent grows
    // as more items realize), the at-tail check returns false, and the
    // handler wrongly latches isUserDetachedSnapshot = true — silently
    // disabling auto-scroll for the rest of the session.
    private bool isPerformingProgrammaticScroll;

    public TerminalViewer()
    {
        InitializeComponent();

        // Intentionally NOT setting DataContext = this. Doing so would
        // override the inherited DataContext on the <atc:TerminalViewer>
        // element itself, which silently breaks any consumer-side binding
        // like <atc:TerminalViewer AutoScroll="{Binding ViewModelProp}" />.
        // All internal bindings inside the XAML reference the control via
        // RelativeSource AncestorType=UserControl (or the ListView's Tag for
        // the ContextMenu), so they don't depend on DataContext being self.

        Messenger.Default.Register<TerminalReceivedDataEventArgs>(
            this,
            TerminalReceivedDataHandle);
        Messenger.Default.Register<TerminalClearEventArgs>(
            this,
            TerminalClearEventArgsHandle);

        queueProcessingTask = Task.Run(
            () => ProcessQueueContinuously(cts.Token),
            cts.Token);
    }

    // ---------------------------------------------------------------------
    // Appearance DPs
    // ---------------------------------------------------------------------

    [DependencyProperty(DefaultValue = "Black")]
    private Brush terminalBackground;

    [DependencyProperty(DefaultValue = "Consolas")]
    private FontFamily terminalFontFamily;

    [DependencyProperty(DefaultValue = "12.0")]
    private double terminalFontSize;

    [DependencyProperty(DefaultValue = "Teal")]
    private Brush defaultTextColor;

    [DependencyProperty(DefaultValue = "Red")]
    private Brush errorTextColor;

    [DependencyProperty]
    private IList<string>? termsError;

    [DependencyProperty(DefaultValue = "LimeGreen")]
    private Brush successfulTextColor;

    [DependencyProperty]
    private IList<string>? termsSuccessful;

    [DependencyProperty(DefaultValue = "Chocolate")]
    private Brush terms1TextColor;

    [DependencyProperty]
    private IList<string>? terms1;

    [DependencyProperty(DefaultValue = "DarkOrange")]
    private Brush terms2TextColor;

    [DependencyProperty]
    private IList<string>? terms2;

    [DependencyProperty(DefaultValue = "CornflowerBlue")]
    private Brush terms3TextColor;

    [DependencyProperty]
    private IList<string>? terms3;

    // ---------------------------------------------------------------------
    // Toolbar visibility DPs
    // ---------------------------------------------------------------------

    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showCopyInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showAutoScrollInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showPauseInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showSearchInToolbar;

    [DependencyProperty(DefaultValue = false)]
    private bool showExportInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showWrapInToolbar;

    // ---------------------------------------------------------------------
    // Behaviour DPs
    // ---------------------------------------------------------------------

    /// <summary>
    /// When <c>true</c>, new lines auto-scroll into view as long as the user
    /// is at the tail of the list. Detected via <see cref="ScrollViewer.ScrollChanged"/>.
    /// Default <c>true</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnAutoScrollChanged))]
    private bool autoScroll;

    /// <summary>
    /// When <c>true</c>, the drain loop stops dispatching incoming lines while
    /// the channel keeps capturing them. The Pause toolbar toggle shows the
    /// live <see cref="BufferedCount"/> while paused. Default <c>false</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnIsPausedChanged))]
    private bool isPaused;

    /// <summary>
    /// Maximum number of lines kept in the visible buffer. When new lines push
    /// the count past this value, the oldest lines are dropped. <c>0</c>
    /// disables the cap (only safe for short-running sessions). Default <c>10000</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 10000,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private int maxLines;

    /// <summary>
    /// Search pattern applied to incoming lines. Empty disables the filter.
    /// When <see cref="UseRegex"/> is <c>true</c> this is treated as a .NET
    /// regex (case-insensitive); otherwise as a substring (case-insensitive).
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSearchInputChanged))]
    private string searchText;

    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSearchInputChanged))]
    private bool useRegex;

    /// <summary>
    /// When <c>true</c>, lines that don't match <see cref="SearchText"/> are
    /// hidden via the underlying <see cref="ItemCollection.Filter"/>. When
    /// <c>false</c>, all lines remain visible — matches are still highlighted
    /// in-line and counted in <see cref="MatchCount"/>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSearchInputChanged))]
    private bool hideNonMatching;

    /// <summary>
    /// Prepend an <c>HH:mm:ss.fff</c> timestamp to each line. Default <c>false</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private bool showTimestamps;

    /// <summary>
    /// Prepend a 5-digit line number to each line. Default <c>false</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private bool showLineNumbers;

    /// <summary>
    /// Wrap long lines instead of horizontally scrolling. Default <c>true</c>.
    /// Toggle from the toolbar Wrap button or set programmatically.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private bool wordWrap;

    /// <summary>
    /// When <c>true</c>, incoming lines are scanned for ANSI escape sequences
    /// (CSI <c>[ ... m</c>) and rendered with their original colours / bold /
    /// italic / underline. Strips non-SGR sequences (cursor movement, screen
    /// clear, etc.) silently. Default <c>true</c>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnEnableAnsiParsingChanged))]
    private bool enableAnsiParsing;

    private static void OnAutoScrollChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TerminalViewer tv)
        {
            tv.autoScrollSnapshot = (bool)e.NewValue;
        }
    }

    private static void OnIsPausedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TerminalViewer tv)
        {
            tv.isPausedSnapshot = (bool)e.NewValue;
        }
    }

    private static void OnEnableAnsiParsingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TerminalViewer tv)
        {
            tv.enableAnsiParsingSnapshot = (bool)e.NewValue;
        }
    }

    // ---------------------------------------------------------------------
    // Read-only state DPs surfaced for the Jump-to-live overlay
    // ---------------------------------------------------------------------

    [DependencyProperty(DefaultValue = false)]
    private bool isDetachedFromTail;

    [DependencyProperty(DefaultValue = 0)]
    private int newSinceDetached;

    [DependencyProperty(DefaultValue = 0)]
    private int bufferedCount;

    [DependencyProperty(DefaultValue = 0)]
    private int matchCount;

    public void Dispose()
    {
        Messenger.Default.UnRegister<TerminalReceivedDataEventArgs>(this, TerminalReceivedDataHandle);
        Messenger.Default.UnRegister<TerminalClearEventArgs>(this, TerminalClearEventArgsHandle);

        receivedDataChannel.Writer.TryComplete();
        cts.Cancel();

        // Dispose the CTS only after the background loop has actually exited,
        // to avoid blocking the UI thread while the loop is mid-Dispatcher.InvokeAsync.
        if (queueProcessingTask is null)
        {
            cts.Dispose();
            return;
        }

        _ = queueProcessingTask.ContinueWith(
            static (_, state) => ((CancellationTokenSource)state!).Dispose(),
            cts,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Programmatically scrolls to the tail of the list and resets the
    /// detached state. Safe to call when there are no entries.
    /// </summary>
    public void JumpToLive()
    {
        if (ListViewTerminal.Items.Count <= 0)
        {
            return;
        }

        ScrollToTail();
        IsDetachedFromTail = false;
        NewSinceDetached = 0;
        isUserDetachedSnapshot = false;
    }

    // ---------------------------------------------------------------------
    // Search / filter / match navigation
    // ---------------------------------------------------------------------

    private static void OnSearchInputChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TerminalViewer tv)
        {
            tv.RebuildSearchRegex();
            tv.ApplyFilterAndCountMatches();
        }
    }

    private void RebuildSearchRegex()
    {
        compiledSearchRegex = null;
        if (string.IsNullOrEmpty(SearchText) || !UseRegex)
        {
            return;
        }

        try
        {
            compiledSearchRegex = new Regex(
                SearchText,
                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(50));
        }
        catch (ArgumentException)
        {
            // Invalid pattern — leave compiledSearchRegex null; LineMatches treats this as "no match".
        }
    }

    private bool LineMatches(string text)
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            return true;
        }

        if (UseRegex)
        {
            return compiledSearchRegex is not null && compiledSearchRegex.IsMatch(text);
        }

        return text.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyFilterAndCountMatches()
    {
        var filterEnabled = HideNonMatching && !string.IsNullOrEmpty(SearchText);
        ListViewTerminal.Items.Filter = filterEnabled
            ? FilterPredicate
            : null;

        if (string.IsNullOrEmpty(SearchText))
        {
            MatchCount = 0;
            return;
        }

        var count = 0;
        foreach (var item in ListViewTerminal.Items.SourceCollection)
        {
            if (item is TerminalLineItem line && LineMatches(line.Text))
            {
                count++;
            }
        }

        MatchCount = count;
    }

    private bool FilterPredicate(object item)
        => item is not TerminalLineItem line || LineMatches(line.Text);

    [RelayCommand]
    private void FocusSearch()
    {
        if (PartSearchTextBox is null)
        {
            return;
        }

        PartSearchTextBox.Focus();
        PartSearchTextBox.SelectAll();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    private bool CanExecuteHasMatches()
        => MatchCount > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteHasMatches))]
    private void NextMatch()
        => MoveToMatch(forward: true);

    [RelayCommand(CanExecute = nameof(CanExecuteHasMatches))]
    private void PreviousMatch()
        => MoveToMatch(forward: false);

    private void MoveToMatch(bool forward)
    {
        if (ListViewTerminal.Items.Count == 0)
        {
            return;
        }

        var startIndex = ListViewTerminal.SelectedIndex;
        var count = ListViewTerminal.Items.Count;
        for (var step = 1; step <= count; step++)
        {
            var probe = forward
                ? (startIndex + step) % count
                : ((startIndex - step) % count + count) % count;

            if (ListViewTerminal.Items[probe] is TerminalLineItem line && LineMatches(line.Text))
            {
                ListViewTerminal.SelectedIndex = probe;
                ListViewTerminal.ScrollIntoView(ListViewTerminal.Items[probe]!);
                return;
            }
        }
    }

    [RelayCommand]
    private void TogglePause()
        => IsPaused = !IsPaused;

    [RelayCommand]
    private void JumpToLiveCommandHandler()
        => JumpToLive();

    // ---------------------------------------------------------------------
    // Export
    // ---------------------------------------------------------------------

    private bool CanExport()
        => ListViewTerminal.Items.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExport))]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Surface failures to the user instead of crashing the viewer.")]
    private void Export()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Text (*.txt)|*.txt|Log (*.log)|*.log",
            FilterIndex = 1,
            FileName = "terminal-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", GlobalizationConstants.EnglishCultureInfo) + ".txt",
            AddExtension = true,
            Title = Miscellaneous.ExportTerminalOutput,
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(dialog.FileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var sb = new StringBuilder();
            foreach (var item in ListViewTerminal.Items)
            {
                if (item is TerminalLineItem line)
                {
                    sb.AppendLine(line.Text);
                }
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            ShowExportFailureDialog(ex);
        }
    }

    private static void ShowExportFailureDialog(Exception ex)
    {
        var owner = Application.Current?.MainWindow;
        if (owner is null)
        {
            return;
        }

        var settings = new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Error)
        {
            TitleBarText = Miscellaneous.ExportFailed,
        };

        var message = string.Format(
            CultureInfo.CurrentCulture,
            Miscellaneous.CouldNotExportTerminalOutputFormat1,
            ex.Message);

        var dialog = new InfoDialogBox(owner, settings, message);
        dialog.ShowDialog();
    }

    // ---------------------------------------------------------------------
    // Existing copy/clear commands
    // ---------------------------------------------------------------------

    private bool CanExecuteHasItems()
        => ListViewTerminal.Items.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteHasItems))]
    private void CopyToClipboard()
    {
        var sb = new StringBuilder();
        foreach (var item in ListViewTerminal.Items)
        {
            if (item is TerminalLineItem terminalLineItem)
            {
                sb.AppendLine(terminalLineItem.Text);
            }
        }

        if (sb.Length > 0)
        {
            System.Windows.Clipboard.SetText(sb.ToString());
        }
    }

    private bool CanCopySelected()
        => ListViewTerminal.SelectedItems.Count > 0;

    [RelayCommand(CanExecute = nameof(CanCopySelected))]
    private void CopySelectedToClipboard()
    {
        var sb = new StringBuilder();
        foreach (var item in ListViewTerminal.SelectedItems)
        {
            if (item is TerminalLineItem terminalLineItem)
            {
                sb.AppendLine(terminalLineItem.Text);
            }
        }

        if (sb.Length > 0)
        {
            System.Windows.Clipboard.SetText(sb.ToString());
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteHasItems))]
    private void ClearScreen()
    {
        var pinned = ListViewTerminal.Items
            .OfType<TerminalLineItem>()
            .Where(line => line.IsPinned)
            .ToList();

        ListViewTerminal.Items.Clear();

        // Reset the line-number counter so the next received line starts fresh.
        // Pinned lines keep their original numbers (purely informational).
        nextLineNumber = 1;

        foreach (var p in pinned)
        {
            ListViewTerminal.Items.Add(p);
        }

        ResetTailState();
        ApplyFilterAndCountMatches();
    }

    // ---------------------------------------------------------------------
    // Pin / unpin
    // ---------------------------------------------------------------------

    [RelayCommand]
    private void TogglePinSelected()
    {
        var indices = new List<int>();
        foreach (var item in ListViewTerminal.SelectedItems)
        {
            var idx = ListViewTerminal.Items.IndexOf(item);
            if (idx >= 0)
            {
                indices.Add(idx);
            }
        }

        // Replace each selected item with a pin-toggled copy. Done in a
        // separate pass since the indices reference the live ItemCollection.
        foreach (var idx in indices)
        {
            if (ListViewTerminal.Items[idx] is TerminalLineItem line)
            {
                ListViewTerminal.Items[idx] = line with { IsPinned = !line.IsPinned };
            }
        }
    }

    // ---------------------------------------------------------------------
    // Zoom (font-size ±)
    // ---------------------------------------------------------------------

    [RelayCommand]
    private void ZoomIn()
        => TerminalFontSize = global::System.Math.Min(MaxFontSize, TerminalFontSize + FontSizeStep);

    [RelayCommand]
    private void ZoomOut()
        => TerminalFontSize = global::System.Math.Max(MinFontSize, TerminalFontSize - FontSizeStep);

    [RelayCommand]
    private void ZoomReset()
        => TerminalFontSize = DefaultFontSizeValue;

    private void TerminalReceivedDataHandle(TerminalReceivedDataEventArgs obj)
        => receivedDataChannel.Writer.TryWrite(obj);

    private void TerminalClearEventArgsHandle(TerminalClearEventArgs obj)
    {
        while (receivedDataChannel.Reader.TryRead(out _))
        {
            // Drain whatever was buffered so the cleared screen stays clear.
        }

        var dispatcher = ListViewTerminal.Dispatcher;
        if (dispatcher.CheckAccess())
        {
            ListViewTerminal.Items.Clear();
            ResetTailState();
        }
        else
        {
            _ = dispatcher.InvokeAsync(
                () =>
                {
                    ListViewTerminal.Items.Clear();
                    ResetTailState();
                },
                DispatcherPriority.Render);
        }
    }

    private void ResetTailState()
    {
        isUserDetachedSnapshot = false;
        IsDetachedFromTail = false;
        NewSinceDetached = 0;
        UpdateBufferedCount();
        if (ListViewTerminal.Items.Count == 0)
        {
            nextLineNumber = 1;
            ansiState = AnsiSgrState.Default;
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Drain loop must survive transient dispatch failures during shutdown.")]
    private async Task ProcessQueueContinuously(CancellationToken token)
    {
        var reader = receivedDataChannel.Reader;

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (isPausedSnapshot)
                {
                    await Task.Delay(100, token).ConfigureAwait(false);
                    UpdateBufferedCount();
                    continue;
                }

                if (!await reader.WaitToReadAsync(token).ConfigureAwait(false))
                {
                    return;
                }

                if (isPausedSnapshot)
                {
                    continue;
                }

                var batch = new List<TerminalReceivedDataEventArgs>(capacity: 16);

                while (reader.TryRead(out var data))
                {
                    batch.Add(data);
                }

                if (batch.Count == 0)
                {
                    continue;
                }

                try
                {
                    await ListViewTerminal.Dispatcher.InvokeAsync(
                        () =>
                        {
                            var addedCount = 0;
                            foreach (var line in batch.SelectMany(e => e.Lines))
                            {
                                IReadOnlyList<TerminalRun>? runs = null;
                                if (enableAnsiParsingSnapshot && AnsiSequenceParser.ContainsEscapeSequence(line))
                                {
                                    var (parsedRuns, newState) = AnsiSequenceParser.Parse(line, ansiState);
                                    ansiState = newState;
                                    runs = parsedRuns;
                                }

                                var item = new TerminalLineItem(
                                    line,
                                    GetColorForLine(line))
                                {
                                    Timestamp = DateTimeOffset.Now,
                                    LineNumber = nextLineNumber++,
                                    Runs = runs,
                                };
                                ListViewTerminal.Items.Add(item);
                                addedCount++;
                            }

                            TrimToCap();

                            if (isUserDetachedSnapshot && addedCount > 0)
                            {
                                NewSinceDetached += addedCount;
                            }

                            // Recount matches whenever new lines arrive (or
                            // get trimmed) so the toolbar badge stays live.
                            if (!string.IsNullOrEmpty(SearchText))
                            {
                                ApplyFilterAndCountMatches();
                            }

                            CopyToClipboardCommand.RaiseCanExecuteChanged();
                            ClearScreenCommand.RaiseCanExecuteChanged();
                            ExportCommand.RaiseCanExecuteChanged();
                            NextMatchCommand.RaiseCanExecuteChanged();
                            PreviousMatchCommand.RaiseCanExecuteChanged();

                            ListViewTerminal.UpdateLayout();
                        },
                        DispatcherPriority.Render,
                        token);

                    UpdateBufferedCount();

                    if (autoScrollSnapshot && !isUserDetachedSnapshot)
                    {
                        await ListViewTerminal.Dispatcher.InvokeAsync(
                            ScrollToTail,
                            DispatcherPriority.Background,
                            token);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception)
                {
                    // App tearing down; give up cleanly.
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }
    }

    private void TrimToCap()
    {
        if (MaxLines <= 0)
        {
            return;
        }

        while (ListViewTerminal.Items.Count > MaxLines)
        {
            ListViewTerminal.Items.RemoveAt(0);
        }
    }

    private void UpdateBufferedCount()
    {
        var count = receivedDataChannel.Reader.CanCount
            ? receivedDataChannel.Reader.Count
            : 0;

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return;
        }

        if (dispatcher.CheckAccess())
        {
            BufferedCount = count;
        }
        else
        {
            _ = dispatcher.BeginInvoke(
                () => BufferedCount = count,
                DispatcherPriority.Background);
        }
    }

    private void ScrollToTail()
    {
        var count = ListViewTerminal.Items.Count;
        if (count <= 0)
        {
            return;
        }

        isPerformingProgrammaticScroll = true;
        try
        {
            // ScrollIntoView is the reliable path under WPF virtualization —
            // it forces the panel to realize the target item, then scrolls to
            // it. ScrollToBottom alone is unreliable because ScrollableHeight
            // is estimated from the realized-item subset.
            ListViewTerminal.ScrollIntoView(ListViewTerminal.Items[count - 1]!);

            // Belt and braces: also nudge the inner ScrollViewer to the
            // bottom in case any pixel-rounding / wrapping leaves us a row
            // short.
            var scrollViewer = cachedScrollViewer ??=
                VisualTreeHelperEx.FindChild<ScrollViewer>(ListViewTerminal);
            scrollViewer?.ScrollToBottom();
        }
        finally
        {
            // Reset the guard *after* the dispatcher has processed any
            // ScrollChanged events we just provoked. ScrollChanged fires at
            // ContextIdle when the layout pass completes; resetting at
            // ApplicationIdle guarantees the handler has already returned.
            Dispatcher.BeginInvoke(
                new Action(() => isPerformingProgrammaticScroll = false),
                DispatcherPriority.ApplicationIdle);
        }
    }

    /// <summary>
    /// Tracks whether the user has manually scrolled away from the tail.
    /// We only react to user-driven scrolls (<see cref="ScrollChangedEventArgs.VerticalChange"/>
    /// != 0) — layout-induced events that change <c>ScrollableHeight</c> but
    /// not <c>VerticalOffset</c> (content overflowed the viewport while the
    /// user sat still) must not flip the detached state, otherwise auto-scroll
    /// would never engage on the very first overflow.
    /// </summary>
    private void OnListViewScrollChanged(
        object sender,
        ScrollChangedEventArgs e)
    {
        if (e.OriginalSource is not ScrollViewer sv)
        {
            return;
        }

        if (e.VerticalChange == 0)
        {
            // Pure layout change (extent grew / shrank, viewport resized).
            // Leave the detached state alone — a user who hasn't moved has
            // not "detached".
            return;
        }

        if (isPerformingProgrammaticScroll)
        {
            // Our own ScrollIntoView / ScrollToBottom is the source. Don't
            // misread it as a user-driven detach.
            return;
        }

        var atTail = sv.VerticalOffset >= sv.ScrollableHeight - TailDetectionThresholdPixels;

        if (atTail)
        {
            isUserDetachedSnapshot = false;
            if (IsDetachedFromTail)
            {
                IsDetachedFromTail = false;
                NewSinceDetached = 0;
            }
        }
        else
        {
            isUserDetachedSnapshot = true;
            if (!IsDetachedFromTail)
            {
                IsDetachedFromTail = true;
            }
        }
    }

    private void OnJumpToLiveClick(
        object sender,
        RoutedEventArgs e)
        => JumpToLive();

    private void OnTerminalSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        => CopySelectedToClipboardCommand.RaiseCanExecuteChanged();

    private Brush GetColorForLine(string line)
    {
        var lineSpan = line.AsSpan();

        var checks = new (
            IEnumerable<string>? Terms,
            Brush Colour,
            StringComparison Comparison)[]
            {
                (TermsError,
                    ErrorTextColor,
                    StringComparison.OrdinalIgnoreCase),
                (TermsSuccessful,
                    SuccessfulTextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms1,
                    Terms1TextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms2,
                    Terms2TextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms3,
                    Terms3TextColor,
                    StringComparison.OrdinalIgnoreCase),
            };

        foreach (var (terms, colour, cmp) in checks)
        {
            if (terms is null)
            {
                continue;
            }

            foreach (var term in terms)
            {
                if (ContainsTermAsWord(
                        lineSpan,
                        term.AsSpan(),
                        cmp))
                {
                    return colour;
                }
            }
        }

        return DefaultTextColor;
    }

    private static bool ContainsTermAsWord(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> term,
        StringComparison comparison)
    {
        int index;
        while ((index = line.IndexOf(
                   term,
                   comparison)) >= 0)
        {
            var atStart = index == 0 || char.IsWhiteSpace(line[index - 1]);
            var end = index + term.Length;
            var atEnd = end == line.Length || (end < line.Length && char.IsWhiteSpace(line[end]));

            if (atStart && atEnd)
            {
                return true;
            }

            line = line[(index + 1)..];
        }

        return false;
    }
}
