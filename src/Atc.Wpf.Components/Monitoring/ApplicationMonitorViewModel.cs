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

        Entries.CollectionChanged += (_, _) => CopyAllCommand.RaiseCanExecuteChanged();

        FilterChange();

        MessengerInstance.Register<ApplicationEventEntry>(
            this,
            OnApplicationEventEntryHandler);

        if (XamlToolkit.Helpers.DesignModeHelper.IsInDesignMode)
        {
            Entries.AddRange(DesignModeHelper.CreateApplicationEventEntryList());
        }
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

    public void AddEntry(ApplicationEventEntry entry)
    {
        lock (SyncLock)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Entries.Add(entry);

                if (AutoScroll)
                {
                    MessengerInstance.Send(new ApplicationMonitorScrollEvent(SortDirection));
                }
            }
            else
            {
                _ = Application.Current.Dispatcher.BeginInvoke(() => Entries.Add(entry));

                if (AutoScroll)
                {
                    MessengerInstance.Send(new ApplicationMonitorScrollEvent(SortDirection));
                }
            }
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