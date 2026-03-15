namespace Atc.Wpf.Sample.SamplesWpf.Threading;

public partial class DebounceView
{
    private readonly DebounceDispatcher debounceTimer = new();

    public DebounceView()
    {
        InitializeComponent();
    }

    private void SearchTextBoxOnKeyup(
        object sender,
        KeyEventArgs e)
    {
        if (DataContext is not DebounceViewModel vm)
        {
            return;
        }

        debounceTimer.Debounce(
            vm.DebounceDelayMs,
            _ => ExecuteSearch());
    }

    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "OK")]
    private async void ExecuteSearch()
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