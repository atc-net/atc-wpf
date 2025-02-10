namespace Atc.Wpf.Sample.SamplesWpf.Threading;

public sealed class DebounceViewModel : ViewModelBase
{
    private readonly Collection<string> totalItems;
    private string status;
    private string filter;

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
        Filter = searchQuery;
        FoundItems.Clear();

        if (string.IsNullOrEmpty(searchQuery))
        {
            Status = "Clear result";
            return;
        }

        Status = "Searching...";

        await Task.Delay(1000).ConfigureAwait(true);

        foreach (var item in totalItems
                     .Where(item => item.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
        {
            FoundItems.Add(item);
        }

        Status = $"Done - found {FoundItems.Count}";
    }
}