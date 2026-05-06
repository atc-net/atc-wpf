namespace Atc.Wpf.Components.Monitoring;

public sealed partial class ApplicationMonitorViewModel : ViewModelBase, IDisposable
{
    private const string ClipboardFieldSeparator = " | ";

    private static readonly object SyncLock = new();

    private readonly ICollectionView view;
    private readonly Channel<ApplicationEventEntry> entryChannel =
        Channel.CreateUnbounded<ApplicationEventEntry>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
            });

    private readonly CancellationTokenSource cts = new();
    private readonly Task? queueProcessingTask;

    private ListSortDirection sortDirection;
    private int maxEntries = 10000;

    [ObservableProperty]
    private ApplicationFilterViewModel filter = new();

    [ObservableProperty(DependentPropertyNames = [nameof(IsScrollingUp), nameof(IsScrollingDown)])]
    private bool autoScroll = true;

    [ObservableProperty]
    private ApplicationEventEntry? selectedEntry;

    [ObservableProperty]
    private bool showColumnArea = true;

    /// <summary>
    /// When <c>true</c>, the drain loop stops dispatching incoming entries to
    /// <see cref="Entries"/> while continuing to buffer them in the channel.
    /// <see cref="BufferedCount"/> reflects the held-but-not-shown count.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(BufferedCount)])]
    private bool isPaused;

    [ObservableProperty(AfterChangedCallback = nameof(OnListenOnToastNotificationMessageChanged))]
    private bool listenOnToastNotificationMessage;

    public ApplicationMonitorViewModel()
    {
        Entries = [];
        SelectedEntries = [];
        BindingOperations.EnableCollectionSynchronization(
            Entries,
            SyncLock);
        view = CollectionViewSource.GetDefaultView(Entries);
        view.SortDescriptions.Clear();
        SortDirection = ListSortDirection.Ascending;

        // Field default is false; this assignment transitions to true and so
        // fires the AfterChangedCallback that registers the messenger handler.
        ListenOnToastNotificationMessage = true;

        Entries.CollectionChanged += (_, _) =>
        {
            CopyAllCommand.RaiseCanExecuteChanged();
            ExportCommand.RaiseCanExecuteChanged();
        };

        FilterChange();

        MessengerInstance.Register<ApplicationEventEntry>(
            this,
            OnApplicationEventEntryHandler);

        if (XamlToolkit.Helpers.DesignModeHelper.IsInDesignMode)
        {
            Entries.AddRange(DesignModeHelper.CreateApplicationEventEntryList());
            return;
        }

        queueProcessingTask = Task.Run(
            () => ProcessQueueContinuouslyAsync(cts.Token),
            cts.Token);
    }

    public ObservableCollectionEx<ApplicationEventEntry> Entries { get; }

    public ObservableCollectionEx<ApplicationEventEntry> SelectedEntries { get; }

    /// <summary>
    /// Number of entries currently sitting in the ingest channel waiting to be
    /// dispatched to <see cref="Entries"/>. Non-zero only while
    /// <see cref="IsPaused"/> is <c>true</c> or under sustained burst load.
    /// </summary>
    public int BufferedCount
        => entryChannel.Reader.CanCount ? entryChannel.Reader.Count : 0;

    /// <summary>
    /// Caps the number of entries kept in <see cref="Entries"/>. When new
    /// entries push the count past this value, the oldest entries (by
    /// insertion order) are dropped. <c>0</c> disables the cap. Negative
    /// values are clamped to <c>0</c>, which is why this stays a hand-rolled
    /// setter rather than an <c>[ObservableProperty]</c>.
    /// </summary>
    public int MaxEntries
    {
        get => maxEntries;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value == maxEntries)
            {
                return;
            }

            maxEntries = value;
            RaisePropertyChanged();
            lock (SyncLock)
            {
                TrimToCap();
            }
        }
    }

    /// <summary>
    /// Sort direction applied to the underlying <see cref="ICollectionView"/>.
    /// Hand-rolled because the setter has to rebuild
    /// <see cref="ICollectionView.SortDescriptions"/> and refresh the view —
    /// not a pure value-store.
    /// </summary>
    public ListSortDirection SortDirection
    {
        get => sortDirection;

        set
        {
            sortDirection = value;
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(
                new SortDescription(
                    nameof(ApplicationEventEntry.Timestamp),
                    sortDirection));
            view.Refresh();
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsScrollingUp));
            RaisePropertyChanged(nameof(IsScrollingDown));
        }
    }

    public bool IsScrollingUp
        => AutoScroll && SortDirection == ListSortDirection.Ascending;

    public bool IsScrollingDown
        => AutoScroll && SortDirection == ListSortDirection.Descending;

    /// <summary>
    /// Free-text filter applied to the visible entries. The getter / setter
    /// delegate to <see cref="ApplicationFilterViewModel.MatchOnTextInData"/>
    /// (no backing field of our own), so this can't be an
    /// <c>[ObservableProperty]</c>.
    /// </summary>
    public string MatchOnText
    {
        get => filter.MatchOnTextInData;

        set
        {
            filter.MatchOnTextInData = value;
            RaisePropertyChanged();

            FilterChange();
        }
    }

    private void OnListenOnToastNotificationMessageChanged()
    {
        if (listenOnToastNotificationMessage)
        {
            MessengerInstance.Register<ToastNotificationMessage>(
                this,
                OnToastNotificationMessageHandler);
        }
        else
        {
            MessengerInstance.UnRegister<ToastNotificationMessage>(
                this,
                OnToastNotificationMessageHandler);
        }
    }

    /// <summary>
    /// Enqueues <paramref name="entry"/> for batched UI dispatch. Thread-safe;
    /// callers can produce from any thread without taking <see cref="SyncLock"/>.
    /// In design mode the entry is added directly to <see cref="Entries"/>
    /// since the drain loop is not started.
    /// </summary>
    public void AddEntry(ApplicationEventEntry entry)
    {
        if (XamlToolkit.Helpers.DesignModeHelper.IsInDesignMode)
        {
            lock (SyncLock)
            {
                Entries.Add(entry);
                TrimToCap();
            }

            return;
        }

        entryChannel.Writer.TryWrite(entry);
    }

    /// <summary>
    /// Drains queued entries in batches, dispatches each batch to the UI thread
    /// in a single <see cref="Application.Current"/>.<see cref="Dispatcher.InvokeAsync(Action)"/> call,
    /// and emits one <see cref="ApplicationMonitorScrollEvent"/> per batch when
    /// <see cref="AutoScroll"/> is on. Loops until the channel completes or
    /// <paramref name="token"/> is cancelled.
    /// </summary>
    private async Task ProcessQueueContinuouslyAsync(CancellationToken token)
    {
        var reader = entryChannel.Reader;

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (isPaused)
                {
                    await Task.Delay(100, token).ConfigureAwait(false);
                    NotifyBufferedCountChanged();
                    continue;
                }

                if (!await reader.WaitToReadAsync(token).ConfigureAwait(false))
                {
                    return;
                }

                if (!await DrainAndDispatchBatchAsync(reader, token).ConfigureAwait(false))
                {
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }
    }

    /// <summary>
    /// Drains everything currently in the channel, dispatches it as a single
    /// batch on the UI thread, and emits one scroll event for the batch when
    /// auto-scroll is on. Returns <c>false</c> when the loop should exit
    /// (Application is tearing down or token was cancelled mid-dispatch).
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Drain loop must survive transient dispatch failures during shutdown.")]
    private async Task<bool> DrainAndDispatchBatchAsync(
        ChannelReader<ApplicationEventEntry> reader,
        CancellationToken token)
    {
        if (isPaused)
        {
            return true;
        }

        var batch = new List<ApplicationEventEntry>(capacity: 32);
        while (reader.TryRead(out var entry))
        {
            batch.Add(entry);
        }

        if (batch.Count == 0)
        {
            return true;
        }

        var app = Application.Current;
        if (app is null)
        {
            return false;
        }

        try
        {
            await app.Dispatcher.InvokeAsync(
                () => CommitBatchOnUiThread(batch),
                DispatcherPriority.Background,
                token);

            NotifyBufferedCountChanged();
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception)
        {
            // Application is tearing down; give up cleanly rather than spinning the loop.
            return false;
        }
    }

    private void CommitBatchOnUiThread(List<ApplicationEventEntry> batch)
    {
        lock (SyncLock)
        {
            foreach (var entry in batch)
            {
                Entries.Add(entry);
            }

            TrimToCap();
        }

        if (AutoScroll)
        {
            MessengerInstance.Send(new ApplicationMonitorScrollEvent(SortDirection));
        }
    }

    private void NotifyBufferedCountChanged()
    {
        var app = Application.Current;
        if (app is null)
        {
            return;
        }

        if (app.Dispatcher.CheckAccess())
        {
            RaisePropertyChanged(nameof(BufferedCount));
        }
        else
        {
            _ = app.Dispatcher.BeginInvoke(
                () => RaisePropertyChanged(nameof(BufferedCount)),
                DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Drops the oldest entries (by insertion order) until <see cref="Entries"/>
    /// is at or below <see cref="MaxEntries"/>. No-op when the cap is <c>0</c>
    /// or the count is already within the cap. Caller must hold
    /// <see cref="SyncLock"/>.
    /// </summary>
    private void TrimToCap()
    {
        if (maxEntries <= 0)
        {
            return;
        }

        while (Entries.Count > maxEntries)
        {
            Entries.RemoveAt(0);
        }
    }

    /// <summary>
    /// Replaces <see cref="SelectedEntries"/> with the supplied entries and
    /// re-evaluates the per-selection copy commands' CanExecute. Call this from
    /// the host control whenever its multi-selection state changes (WPF's
    /// ListView.SelectedItems is not directly bindable to a collection property).
    /// </summary>
    public void SetSelectedEntries(IEnumerable<ApplicationEventEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        SelectedEntries.SuppressOnChangedNotification = true;
        SelectedEntries.Clear();
        foreach (var entry in entries)
        {
            SelectedEntries.Add(entry);
        }

        SelectedEntries.SuppressOnChangedNotification = false;

        CopySelectedCommand.RaiseCanExecuteChanged();
        CopyAllCommand.RaiseCanExecuteChanged();
    }

    public void Dispose()
    {
        MessengerInstance.UnRegister<ApplicationEventEntry>(
            this,
            OnApplicationEventEntryHandler);
        MessengerInstance.UnRegister<ToastNotificationMessage>(
            this,
            OnToastNotificationMessageHandler);

        entryChannel.Writer.TryComplete();
        cts.Cancel();

        // Dispose the CTS only after the drain loop has actually exited, to avoid
        // racing the loop's mid-dispatch InvokeAsync against CTS disposal.
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

    private void OnApplicationEventEntryHandler(ApplicationEventEntry entry)
        => AddEntry(entry);

    private void OnToastNotificationMessageHandler(
        ToastNotificationMessage message)
    {
        if (!ListenOnToastNotificationMessage)
        {
            return;
        }

        AddEntry(message.ToApplicationEventEntry());
    }

    private bool CanExport()
        => Entries.Count > 0;

    /// <summary>
    /// Opens a Save File dialog and writes the currently visible (filtered)
    /// entries to the chosen file in the format inferred from its extension
    /// (CSV / JSON / TXT). Disabled (button greyed out) while there are no
    /// entries to export.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExport))]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Export failures must surface to the user, not crash the picker.")]
    private void Export()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv|JSON (*.json)|*.json|Text (*.txt)|*.txt",
            FilterIndex = 1,
            FileName = "log-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", GlobalizationConstants.EnglishCultureInfo) + ".csv",
            AddExtension = true,
            Title = Miscellaneous.ExportLog,
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var format = Internal.ApplicationMonitorExportService.InferFormat(dialog.FileName);
            Internal.ApplicationMonitorExportService.Export(
                view.OfType<ApplicationEventEntry>(),
                dialog.FileName,
                format);
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
            Miscellaneous.CouldNotExportLogFormat1,
            ex.Message);

        var dialog = new InfoDialogBox(owner, settings, message);
        dialog.ShowDialog();
    }

    [RelayCommand]
    private void Clear()
    {
        lock (SyncLock)
        {
            SelectedEntry = null;
            SelectedEntries.Clear();
            Entries.Clear();
        }

        CopySelectedCommand.RaiseCanExecuteChanged();
        CopyAllCommand.RaiseCanExecuteChanged();
    }

    private bool CanCopySelected()
        => SelectedEntries.Count > 0;

    [RelayCommand(CanExecute = nameof(CanCopySelected))]
    private void CopySelected()
        => CopyEntriesToClipboard(SelectedEntries);

    private bool CanCopyAll()
        => Entries.Count > 0 &&
           Entries.Count > SelectedEntries.Count;

    [RelayCommand(CanExecute = nameof(CanCopyAll))]
    private void CopyAll()
        => CopyEntriesToClipboard(view.OfType<ApplicationEventEntry>());

    private static void CopyEntriesToClipboard(
        IEnumerable<ApplicationEventEntry> entries)
    {
        var sb = new StringBuilder();
        foreach (var entry in entries)
        {
            sb.Append(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", GlobalizationConstants.EnglishCultureInfo));
            sb.Append(ClipboardFieldSeparator);
            sb.Append(entry.LogCategoryType.ToShortNameBracketed());
            sb.Append(ClipboardFieldSeparator);
            sb.Append(entry.Area);
            sb.Append(ClipboardFieldSeparator);
            sb.AppendLine(entry.Message);
        }

        if (sb.Length > 0)
        {
            System.Windows.Clipboard.SetText(sb.ToString());
        }
    }

    [RelayCommand]
    private void FilterChange()
    {
        view.Filter = null;
        view.Filter = o =>
        {
            if (o is not ApplicationEventEntry entry)
            {
                return true;
            }

            if (!filter.SeverityInformation && entry.LogCategoryType is
                    LogCategoryType.Information or
                    LogCategoryType.Security or
                    LogCategoryType.Audit or
                    LogCategoryType.Service or
                    LogCategoryType.UI or
                    LogCategoryType.Debug or
                    LogCategoryType.Trace)
            {
                return false;
            }

            if (!filter.SeverityWarning && entry.LogCategoryType == LogCategoryType.Warning)
            {
                return false;
            }

            if (!filter.SeverityError && entry.LogCategoryType is
                    LogCategoryType.Error or
                    LogCategoryType.Critical)
            {
                return false;
            }

            if (ShowColumnArea)
            {
                return string.IsNullOrEmpty(filter.MatchOnTextInData) ||
                       entry.Area
                           .Contains(
                               filter.MatchOnTextInData,
                               StringComparison.OrdinalIgnoreCase) ||
                       entry.Message
                           .Contains(
                               filter.MatchOnTextInData,
                               StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrEmpty(filter.MatchOnTextInData) ||
                   entry.Message
                       .Contains(
                           filter.MatchOnTextInData,
                           StringComparison.OrdinalIgnoreCase);
        };
    }
}