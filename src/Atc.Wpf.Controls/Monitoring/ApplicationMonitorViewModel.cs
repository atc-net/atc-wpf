// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
namespace Atc.Wpf.Controls.Monitoring;

public sealed class ApplicationMonitorViewModel : ViewModelBase
{
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
        filter = new ApplicationFilterViewModel();
        BindingOperations.EnableCollectionSynchronization(Entries, SyncLock);
        view = CollectionViewSource.GetDefaultView(Entries);
        view.SortDescriptions.Clear();
        AutoScroll = true;
        SortDirection = ListSortDirection.Ascending;
        ShowColumnArea = true;

        FilterChangeCommandHandler();

        MessengerInstance.Register<ApplicationEventEntry>(this, OnApplicationEventEntryHandler);

        if (IsInDesignMode)
        {
            Entries.AddRange(DesignModeHelper.CreateApplicationEventEntryList());
        }
    }

    public IRelayCommand ClearCommand => new RelayCommand(ClearCommandHandler);

    public IRelayCommand FilterChangeCommand => new RelayCommand(FilterChangeCommandHandler);

    public ApplicationFilterViewModel Filter
    {
        get => filter;
        set
        {
            if (Equals(value, filter))
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
            if (Equals(value, selectedEntry))
            {
                return;
            }

            selectedEntry = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollectionEx<ApplicationEventEntry> Entries { get; }

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

    public bool IsScrollingUp => AutoScroll && SortDirection == ListSortDirection.Ascending;

    public bool IsScrollingDown => AutoScroll && SortDirection == ListSortDirection.Descending;

    public string MatchOnText
    {
        get => filter.MatchOnTextInData;

        set
        {
            filter.MatchOnTextInData = value;
            RaisePropertyChanged();

            FilterChangeCommandHandler();
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
                MessengerInstance.Register<ToastNotificationMessage>(this, OnToastNotificationMessageHandler);
            }
            else
            {
                MessengerInstance.UnRegister<ToastNotificationMessage>(this, OnToastNotificationMessageHandler);
            }
        }
    }

    public void AddEntry(
        ApplicationEventEntry entry)
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

    private void OnApplicationEventEntryHandler(
        ApplicationEventEntry entry)
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

    private void ClearCommandHandler()
    {
        lock (SyncLock)
        {
            SelectedEntry = null;
            Entries.Clear();
        }
    }

    private void FilterChangeCommandHandler()
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
                       entry.Area.Contains(filter.MatchOnTextInData, StringComparison.OrdinalIgnoreCase) ||
                       entry.Message.Contains(filter.MatchOnTextInData, StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrEmpty(filter.MatchOnTextInData) ||
                   entry.Message.Contains(filter.MatchOnTextInData, StringComparison.OrdinalIgnoreCase);
        };
    }
}