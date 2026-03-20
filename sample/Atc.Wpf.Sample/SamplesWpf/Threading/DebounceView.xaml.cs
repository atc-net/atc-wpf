namespace Atc.Wpf.Sample.SamplesWpf.Threading;

public partial class DebounceView
{
    private readonly DebounceDispatcher debounceTimer = new();

    public DebounceView()
    {
        InitializeComponent();
    }

    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "Fire-and-forget in debounce callback")]
    private void SearchTextBoxOnKeyup(
        object sender,
        KeyEventArgs e)
    {
        if (DataContext is not DebounceViewModel vm)
        {
            return;
        }

#pragma warning disable CS4014 // Because this call is not awaited
        debounceTimer.Debounce(
            vm.DebounceDelayMs,
            _ => ExecuteSearchAsync());
#pragma warning restore CS4014
    }

    private async Task ExecuteSearchAsync()
    {
        if (DataContext is not DebounceViewModel vm)
        {
            return;
        }

        if (LcSearch.Content is TextBox searchBox)
        {
            await vm
                .Search(searchBox.Text)
                .ConfigureAwait(true);
        }
    }
}