namespace Atc.Wpf.Sample.SamplesWpf.Threading;

public sealed partial class DebounceViewModel : ViewModelBase
{
    private readonly Collection<string> totalItems;
    private CancellationTokenSource? searchCts;
    private string status;
    private string filter;

    [PropertyDisplay("Delay (ms)", "Behavior", 1)]
    [PropertyRange(100, 3000, 100)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int debounceDelayMs = 300;

    public DebounceViewModel()
    {
        totalItems =
        [
            "John Doe",
            "Jane Doe",
            "Len Kaden",
            "Travis Echo",
            "Malik Lenore",
            "Jasper Blair",
            "Harrison Leilani",
            "Cruz Melodie",
            "Hakeem Rose",
            "Rafael Wanda"
        ];
        FoundItems = [];
        status = string.Empty;
        filter = string.Empty;
    }

    public string Status
    {
        get => status;
        set
        {
            status = value;
            RaisePropertyChanged();
        }
    }

    public string Filter
    {
        get => filter;
        set
        {
            filter = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<string> FoundItems { get; }

    public async Task Search(string searchQuery)
    {
        CancelPreviousSearch();

        var cts = new CancellationTokenSource();
        searchCts = cts;

        Filter = searchQuery;
        FoundItems.Clear();

        if (string.IsNullOrEmpty(searchQuery))
        {
            Status = "Clear result";
            return;
        }

        Status = "Searching...";

        try
        {
            await Task.Delay(1000, cts.Token).ConfigureAwait(true);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (cts.Token.IsCancellationRequested)
        {
            return;
        }

        foreach (var item in totalItems
                     .Where(item => item.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
        {
            FoundItems.Add(item);
        }

        Status = $"Done - found {FoundItems.Count}";
    }

    private void CancelPreviousSearch()
    {
        if (searchCts is null)
        {
            return;
        }

        searchCts.Cancel();
        searchCts.Dispose();
        searchCts = null;
    }
}