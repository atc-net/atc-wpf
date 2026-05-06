// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
namespace Atc.Wpf.Components.Monitoring;

public sealed partial class ApplicationMonitorViewModel : ViewModelBase, IDisposable
{
    private const string ClipboardFieldSeparator = " | ";

    private static readonly object SyncLock = new();
    private readonly ICollectionView view;
    private ApplicationFilterViewModel filter;
    private bool autoScroll;
    private ApplicationEventEntry? selectedEntry;
    private bool showColumnArea;
    private ListSortDirection sortDirection;
    private bool listenOnToastNotificationMessage;
    private int maxEntries = 10000;
    private bool isPaused;

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

    public ApplicationMonitorViewModel()
    {
        Entries = [];
        SelectedEntries = [];
        filter = new ApplicationFilterViewModel();
        BindingOperations.EnableCollectionSynchronization(
            Entries,
            SyncLock);
        view = CollectionViewSource.GetDefaultView(Entries);
        view.SortDescriptions.Clear();
        AutoScroll = true;
        SortDirection = ListSortDirection.Ascending;
        ShowColumnArea = true;
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

    public ApplicationFilterViewModel Filter
    {
        get => filter;
        set
        {
            if (Equals(
                    value,
                    filter))
            {
                return;
            }

            filter = value;
            RaisePropertyChanged();
        }
    }

    public bool AutoScroll
    {
        get => autoScroll;
        set
        {
            if (value == autoScroll)
            {
                return;
            }

            autoScroll = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// When <c>true</c>, the drain loop stops dispatching incoming entries to
    /// <see cref="Entries"/> while continuing to buffer them in the channel.
    /// <see cref="BufferedCount"/> reflects the held-but-not-shown count.
    /// </summary>
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            if (value == isPaused)
            {
                return;
            }

            isPaused = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(BufferedCount));
        }
    }

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
    /// insertion order) are dropped. <c>0</c> disables the cap.
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

    public ApplicationEventEntry? SelectedEntry
    {
        get => selectedEntry;
        set
        {
            if (Equals(
                    value,
                    selectedEntry))
            {
                return;
            }

            selectedEntry = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollectionEx<ApplicationEventEntry> Entries { get; }

    public ObservableCollectionEx<ApplicationEventEntry> SelectedEntries { get; }

    public bool ShowColumnArea
    {
        get => showColumnArea;
        set
        {
            if (value == showColumnArea)
            {
                return;
            }

            showColumnArea = value;
            RaisePropertyChanged();
        }
    }

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

    public bool ListenOnToastNotificationMessage
    {
        get => listenOnToastNotificationMessage;
        set
        {
            if (value == listenOnToastNotificationMessage)
            {
                return;
            }

            listenOnToastNotificationMessage = value;
            RaisePropertyChanged();

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
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Drain loop must survive transient dispatch failures during shutdown.")]
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

                if (isPaused)
                {
                    continue;
                }

                var batch = new List<ApplicationEventEntry>(capacity: 32);

                while (reader.TryRead(out var entry))
                {
                    batch.Add(entry);
                }

                if (batch.Count == 0)
                {
                    continue;
                }

                var app = Application.Current;
                if (app is null)
                {
                    return;
                }

                try
                {
                    await app.Dispatcher.InvokeAsync(
                        () =>
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
                        },
                        DispatcherPriority.Background,
                        token);

                    NotifyBufferedCountChanged();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception)
                {
                    // Application is tearing down; give up cleanly rather than spinning the loop.
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
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
    /// (CSV / JSON / TXT). No-op when there are no entries.
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
            Title = "Export log",
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
            System.Windows.MessageBox.Show(
                "Could not export log:" + Environment.NewLine + ex.Message,
                "Export failed",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
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